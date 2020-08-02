using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NPC
{
    [SerializeField]
    private List<Vector2> movement = new List<Vector2>();
    private List<int> pastMovementIndexes = new List<int>();
    private int nextMovementIndex = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }


    public override void MoveNPC()
    {
        Vector2 currentMovent = movement[nextMovementIndex];
        Move((int)currentMovent.x, (int)currentMovent.y);
        pastMovementIndexes.Add(nextMovementIndex);
        
        if (++nextMovementIndex >= movement.Count) nextMovementIndex = 0;
    }


    /// <summary>
    /// Changes the enemy position from the one it was at the given turn
    /// </summary>
    /// <param name="turn"></param>
    public override void Rewind(int turn)
    {
        transform.position = pastMovements[turn];
        transform.localEulerAngles = pastOrientations[turn];
        nextMovementIndex = pastMovementIndexes[turn];
    }


    public override void EraseTurns(int turn)
    {
        base.EraseTurns(turn);
        pastMovementIndexes.RemoveRange(turn, pastMovementIndexes.Count - turn);
        nextMovementIndex = pastMovementIndexes[turn - 1] + 1;
    }


    /// <summary>
    /// Shoots the player
    /// </summary>
    public override void Shoot()
    {
        if (!render.isVisible) return; // return if offscreen
        bc2D.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        bc2D.enabled = true;

        if (hit.transform == null) return;
        if (hit.transform.CompareTag("Player")) base.Shoot();
    }
}
