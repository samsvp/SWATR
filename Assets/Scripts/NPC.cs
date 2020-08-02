using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : Character
{

    private Transform target;

    // Use this for initialization
    protected override void Start()
    {
        GameManager.instance.AddNPCToList(this); // Add this enemy to the list of enemies
        base.Start();
    }
    

    public abstract void MoveNPC();

    public abstract void Rewind(int turn);
}