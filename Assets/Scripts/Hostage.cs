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
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        turnsLeft = maxTurnsLeft;
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
        if (--turnsLeft == 0) KillHostage();
    }


    public override void Rewind(int turn)
    {
        turnsLeft = maxTurnsLeft - turn;
        if (turnsLeft == 0) KillHostage();
        else if (turnsLeft < 0)
        {
            isDead = true;
            isSaved = false;
        }
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


    public void KillHostage()
    {
        isDead = true;
        isSaved = false;
    }
}
