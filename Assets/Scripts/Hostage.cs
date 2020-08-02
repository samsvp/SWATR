using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostage : NPC
{
    // Turns left until the hostage is killed
    private int turnsLeft;
    public int maxTurnsLeft = 8;

    public bool isDead = false;
    public bool isSaved = false;

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
        if (--turnsLeft == -1) KillHostage();
        UpdateTurnsLeftGUI(turnsLeft);
    }


    public void UpdateTurnsLeftGUI(int turn)
    {
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

        if (turnsLeft == -1) KillHostage();
        else if (turnsLeft == 0) ResurrectHostage();
    }


    public void ResetTurn(int turn)
    {
        turnsLeft = maxTurnsLeft - turn;
        if (turnsLeft == 0) KillHostage();
    }


    public void SaveHostage()
    {
        isSaved = true;
        isDead = false;
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
