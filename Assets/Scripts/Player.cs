using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    
    // Set Player so it is unique
    public static Player instance = null;
    
    // Movement axis
    // Public because the Door needs
    [HideInInspector]
    public int x, y;

    // Rewind logic
    public int rewindTurns = 5;
    [HideInInspector]
    public int rewindsLeft = 5;
    // How many turns the player can rewind back to
    public int rewindLimit = 5;
    private int rewindBeginTurn = -1; // The turn the player initiated the rewind
    [SerializeField]
    private Text rewindsLeftText;

    // Items logic
    [SerializeField]
    private int gunAmmo = 5;
    [SerializeField]
    private Text gunAmmoText;
    [SerializeField]
    private int taserAmmo = 5;
    [SerializeField]
    private Text taserAmmoText;
    [SerializeField]
    private int grenadeAmmo = 3;
    [SerializeField]
    private Text grenadeAmmoText;

    // Turn button
    public GameObject turnButton;
    private Text turnButtonText;
    [SerializeField]
    private GameObject downButton;
    [SerializeField]
    private GameObject upButton;
    private Image downButtonImage;
    [SerializeField]
    private Sprite buttonEnabled;
    private Image upButtonImage;
    [SerializeField]
    private Sprite buttonDisabled;

    // Sprite Logic
    [SerializeField]
    private Sprite gunSprite;
    [SerializeField]
    private Sprite taserSprite;
    [SerializeField]
    private Sprite grenadeSprite;

    // Weapon Logic
    public enum Weapon
    {
        gun,
        taser,
        grenade
    };

    private Weapon weapon = Weapon.gun;

    // Other game objects
    [SerializeField]
    private GameObject grenade;
    private GameObject highlight;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        turnButtonText = turnButton.transform.GetChild(0).GetComponent<Text>();
        upButtonImage = upButton.GetComponent<Image>();
        downButtonImage = downButton.GetComponent<Image>();

        highlight = transform.GetChild(0).gameObject;

        gunAmmoText.text = gunAmmo.ToString();
        taserAmmoText.text = taserAmmo.ToString();
        grenadeAmmoText.text = grenadeAmmo.ToString();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ReloadScene.Reload();
    }


    public void GetInput()
    {
        if (!alive) return;
        // Check if it's the players turn. If it's not then nothing will run
        if (!GameManager.instance.playersTurn || isPerformingAction) return;

        ChangeWeapon();

        // Store directionm which we will be moving
        x = 0;
        y = 0;

        // Get input from input manager and turn it into int
        x = (int)Input.GetAxisRaw("Horizontal");
        y = (int)Input.GetAxisRaw("Vertical");

        // Make sure that no diagonal move is made
        if (x != 0) y = 0;

        // Pass the horizontal and vertical direction that the player is moving. The <Wall> parameter means that
        // the player might interact with a wall when moving
        if (x != 0 || y != 0) Move(x, y);
        else if (Input.GetMouseButtonDown(0))
        {
            switch (weapon)
            {
                case Weapon.gun:
                    ShootGun();
                    break;
                case Weapon.taser:
                    ShootTaser();
                    break;
                case Weapon.grenade:
                    ShootGrenade();
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetMouseButtonDown(1)) Rewind();
        else
        {
            animator.enabled = false;
            render.sprite = sprite;
        }
    }


    public void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon = Weapon.gun;
            sprite = gunSprite;
            animator.SetBool("Taser", false);
            animator.SetBool("Grenade", false);
            highlight.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon = Weapon.taser;
            sprite = taserSprite;
            animator.SetBool("Taser", true);
            animator.SetBool("Grenade", false);
            highlight.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            weapon = Weapon.grenade;
            sprite = grenadeSprite;
            animator.SetBool("Taser", false);
            animator.SetBool("Grenade", true);
            GrenadePosition();
        }
    }


    private void GrenadePosition()
    {
        if (grenadeAmmo <= 0)
        {
            highlight.SetActive(false);
            return;
        }
        highlight.SetActive(true);
        HighlightPosition();
    }


    private void HighlightPosition()
    {
        bc2D.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        bc2D.enabled = true;
        
        if (hit.transform != null)
        {
            var hitPos = hit.transform.position;

            if (hitPos.x == transform.position.x)
            {
                float y = hitPos.y;
                y = y > transform.position.y ? y - 2 : y + 2;
                highlight.transform.position = new Vector3(highlight.transform.position.x, y);
            }
            else
            {
                float x = hitPos.x;
                x = x > transform.position.x ? x - 2 : x + 2;
                highlight.transform.position = new Vector3(x, highlight.transform.position.y);
            }
            
        }
        else highlight.transform.localPosition = new Vector3(0, 6);
    }


    #region rewind
    public void Rewind()
    {
        if (GameManager.instance.Wait)
        {
            GameManager.instance.Wait = false;
            turnButton.SetActive(false);
            int turn = Int32.Parse(turnButtonText.text);
            EraseTurns(turn);
            GameManager.instance.EraseTurns(turn);
            return;
        }

        if (--rewindsLeft < 0) return;
        rewindBeginTurn = GameManager.instance.turns;
        RewindsLeft();
        turnButton.SetActive(true);
        RefreshArrows(Int32.Parse(turnButtonText.text));
        GameManager.instance.Wait = true;
        //GameManager.instance.Rewind(1);
    }

    
    public void ChangeRewindTurn(int i)
    {
        int turn = Int32.Parse(turnButtonText.text);
        int nextTurn = turn + i;
        
        if (nextTurn >= pastMovements.Count)
        {
            upButtonImage.sprite = buttonDisabled;
            return;
        }
        else downButtonImage.sprite = buttonEnabled;

        if (nextTurn < 0 || rewindBeginTurn - nextTurn > rewindLimit)
        {
            downButtonImage.sprite = buttonDisabled;
            return;
        }
        else upButtonImage.sprite = buttonEnabled;

        turnButtonText.text = nextTurn.ToString();
        GameManager.instance.Rewind(nextTurn);

        RefreshArrows(nextTurn);
    }


    public void RewindsLeft()
    {
        rewindsLeftText.text = rewindsLeft.ToString();
    }


    private void RefreshArrows(int turn)
    {
        if (turn <= 0 || rewindBeginTurn - turn == rewindLimit) downButtonImage.sprite = buttonDisabled;
        else downButtonImage.sprite = buttonEnabled;

        if (turn >= pastMovements.Count - 1) upButtonImage.sprite = buttonDisabled;
        else upButtonImage.sprite = buttonEnabled;  
    }
    #endregion rewind

    protected override bool Move(int xDir, int yDir)
    {
        animator.enabled = true;
        if (weapon == Weapon.grenade) highlight.SetActive(false);
        if (base.Move(xDir, yDir))
        {
            // Set playersTurn as false so that the enemies can move
            GameManager.instance.playersTurn = false;
            return true;
        }
        return false;
    }


    protected override IEnumerator SmoothMovement(Vector3 end)
    {
        yield return StartCoroutine(base.SmoothMovement(end));
        if (weapon == Weapon.grenade)
        {
            highlight.SetActive(true);
            GrenadePosition();
        }
    }

    #region shoot
    public void ShootGun()
    {
        if (gunAmmo < 0) return; // Avoid negative numbers
        if (gunAmmo-- == 0) return;
        gunAmmoText.text = gunAmmo.ToString();

        bc2D.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        bc2D.enabled = true;

        Shoot();

        pastMovements.Add(transform.position);
        pastOrientations.Add(transform.localEulerAngles);

        if (hit.transform == null) return;
        if (hit.transform.CompareTag("Enemy")) hit.transform.SendMessage("TakeDamage");
    }


    protected override IEnumerator CShoot()
    {
        yield return StartCoroutine(base.CShoot());
        GameManager.instance.playersTurn = false;
    }


    public void ShootTaser()
    {
        if (taserAmmo < 0) return; // Avoid negative numbers
        if (taserAmmo-- == 0) return;
        taserAmmoText.text = taserAmmo.ToString();

        bc2D.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        bc2D.enabled = true;

        StartCoroutine(ShootTaser(hit));
    }


    private IEnumerator ShootTaser(RaycastHit2D hit)
    {
        // Make the taser transition
        animator.enabled = true;
        animator.SetBool("Taser", true);
        animator.SetBool("Grenade", false);
        // Wait a frame for the transition to take place
        yield return null;

        Shoot();

        pastMovements.Add(transform.position);
        pastOrientations.Add(transform.localEulerAngles);

        if (hit.transform == null) yield break;
        if (hit.transform.CompareTag("Enemy")) hit.transform.SendMessage("TaserKnockOut");
    }


    public void ShootGrenade()
    {
        if (grenadeAmmo < 0) return; // Avoid negative numbers
        if (grenadeAmmo-- == 0) return;
        grenadeAmmoText.text = grenadeAmmo.ToString();

        bc2D.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        bc2D.enabled = true;

        StartCoroutine(ShootGrenade(hit));
    }


    private IEnumerator ShootGrenade(RaycastHit2D hit)
    {
        // Make the taser transition
        animator.enabled = true;
        animator.SetBool("Taser", false);
        animator.SetBool("Grenade", true);
        // Wait a frame for the transition to take place
        yield return null;

        isPerformingAction = true;
        yield return StartCoroutine(CShoot());

        pastMovements.Add(transform.position);
        pastOrientations.Add(transform.localEulerAngles);

        // Spawn Grenade
        Instantiate(grenade, highlight.transform.position, Quaternion.identity);
        if (grenadeAmmo <= 0) highlight.SetActive(false);
    }
    
    #endregion shoot


    protected override IEnumerator CTakeDamage()
    {
        yield return StartCoroutine(base.CTakeDamage());

        highlight.SetActive(false);

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.R)) ReloadScene.Reload();
            yield return null;
        }
    }
    
}
