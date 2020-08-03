using System;
using System.Collections;
using System.Collections.Generic;
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

    // Weapon Logic
    public enum Weapon
    {
        gun,
        taser,
        granade
    };

    private Weapon weapon = Weapon.gun;

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

        gunAmmoText.text = gunAmmo.ToString();
        taserAmmoText.text = taserAmmo.ToString();
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
                case Weapon.granade:
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
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon = Weapon.taser;
            sprite = taserSprite;
            animator.SetBool("Taser", true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.SetBool("Taser", false);
        }
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

    protected override void Move(int xDir, int yDir)
    {
        animator.enabled = true;
        base.Move(xDir, yDir);
        // Set playersTurn as false so that the enemies can move
        GameManager.instance.playersTurn = false;
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
        animator.enabled = true;
        animator.SetBool("Taser", true);

        yield return null;

        Shoot();

        pastMovements.Add(transform.position);
        pastOrientations.Add(transform.localEulerAngles);

        if (hit.transform == null) yield break;
        if (hit.transform.CompareTag("Enemy")) hit.transform.SendMessage("TaserKnockOut");
    }


    protected override IEnumerator CShoot()
    {
        yield return StartCoroutine(base.CShoot());
        GameManager.instance.playersTurn = false;
    }
    #endregion shoot


    protected override IEnumerator CTakeDamage()
    {
        yield return StartCoroutine(base.CTakeDamage());

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.R)) ReloadScene.Reload();
            yield return null;
        }
    }
    
}
