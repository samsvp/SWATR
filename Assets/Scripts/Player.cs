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
    public int rewindsLeft = 5;
    [SerializeField]
    private Text rewindsLeftText;

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
        else if (Input.GetMouseButtonDown(0)) Shoot();
        else if (Input.GetMouseButtonDown(1)) Rewind();
        else
        {
            animator.enabled = false;
            render.sprite = sprite;
        }
    }


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

        if (nextTurn < 0)
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
        if (turn <= 0) downButtonImage.sprite = buttonDisabled;
        else downButtonImage.sprite = buttonEnabled;

        if (turn >= pastMovements.Count - 1) upButtonImage.sprite = buttonDisabled;
        else upButtonImage.sprite = buttonEnabled;
    }


    protected override void Move(int xDir, int yDir)
    {
        animator.enabled = true;
        base.Move(xDir, yDir);
        // Set playersTurn as false so that the enemies can move
        GameManager.instance.playersTurn = false;
    }


    public override void Shoot()
    {
        bc2D.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        bc2D.enabled = true;

        base.Shoot();

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
