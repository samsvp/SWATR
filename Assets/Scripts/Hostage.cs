using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostage : NPC
{
    // Turns left until the hostage is killed
    private int turnsLeft;
    public int maxTurnsLeft = 8;

    private int savedOnTurn = -1;

    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public bool isSaved = false;

    // Once all the enemies in the list are dead the hostage is rescued
    [SerializeField]
    private List<Enemy> enemies = new List<Enemy>();

    // Sprites
    private Sprite aliveSprite;
    [SerializeField]
    private Sprite deadSprite;

    // Text variables
    private TextMesh textMesh;
    private TextMesh outline;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        moving = true;
        turnsLeft = maxTurnsLeft;

        aliveSprite = render.sprite;

        textMesh = transform.GetChild(0).GetChild(0).GetComponent<TextMesh>();
        outline = transform.GetChild(0).GetComponent<TextMesh>();

        UpdateTurnsLeftGUI(maxTurnsLeft);
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }


    /// <summary>
    /// Remove 1 from turn count.
    /// Hostages can't move.
    /// </summary>
    public override void MoveNPC()
    {
        if (!isSaved) IsHostageSaved();
        if (--turnsLeft == -1 && !isSaved) KillHostage();
        UpdateTurnsLeftGUI(turnsLeft);
    }


    public void UpdateTurnsLeftGUI(int turn)
    {
        if (isSaved)
        {
            textMesh.text = "Saved!";
            outline.text = "Saved!";
            return;
        }
        if (turn >= 0)
        {
            var sTurn = turn.ToString();
            textMesh.text = sTurn;
            outline.text = sTurn;
        }
        else
        {
            textMesh.text = "Dead";
            outline.text = "Dead";
        }
    }


    public override void EraseTurns(int turn)
    {
        UpdateTurnsLeftGUI(maxTurnsLeft - turn);
    }


    public override void Rewind(int turn)
    {
        turnsLeft = maxTurnsLeft - turn;
        UpdateTurnsLeftGUI(turnsLeft);

        print("turn " + turn);

        if (isSaved)
        {
            if (savedOnTurn == turn) UnSaveHostage();
            return;
        }
        else if (savedOnTurn == turn)
        {
            SaveHostage();
            //UpdateTurnsLeftGUI(turnsLeft);
            return;
        }

        if (turnsLeft == -1) KillHostage();
        else if (turnsLeft == 0) ResurrectHostage();
    }


    public void ResetTurn(int turn)
    {
        turnsLeft = maxTurnsLeft - turn;
        if (turnsLeft == -1 && !isSaved) KillHostage();
    }


    public void IsHostageSaved()
    {
        if (enemies.TrueForAll(e => e.alive)) return;
        SaveHostage();
    }


    public void SaveHostage()
    {
        print("saved");
        isSaved = true;
        isDead = false;
        savedOnTurn = GameManager.instance.turns;
        UpdateTurnsLeftGUI(savedOnTurn);
        print("savedOnTurn: " + savedOnTurn);
    }


    public void UnSaveHostage()
    {
        isSaved = false;
    }


    public void EraseSave()
    {
        isSaved = true;
        isDead = false;
        savedOnTurn = -1;
        UpdateTurnsLeftGUI(GameManager.instance.turns);
    }


    public void ResurrectHostage()
    {
        render.sprite = aliveSprite;
        isDead = false;
        isSaved = false;
    }


    public void KillHostage()
    {
        render.sprite = deadSprite;
        isDead = true;
        isSaved = false;
    }
}
