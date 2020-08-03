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


    public override void ChooseAction()
    {
        if (!render.isVisible || !alive)
        {
            MoveNPC();
            return;
        }

        RaycastHit2D hit = RShoot();

        if (hit.transform == null) MoveNPC();
        else if (hit.transform.CompareTag("Player"))
        {
            hit.transform.SendMessage("TakeDamage");
            Shoot();
        }
        else MoveNPC();
    }


    public override void MoveNPC()
    {
        if (!alive)
        {
            pastMovements.Add(transform.position);
            pastOrientations.Add(transform.localEulerAngles);
            pastMovementIndexes.Add(nextMovementIndex);
            return;
        }

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
        if (turn != GameManager.instance.turns) nextMovementIndex = pastMovementIndexes[turn];
        else if (turn != 0) nextMovementIndex = pastMovementIndexes[turn-1];
    }


    public override void EraseTurns(int turn)
    {
        base.EraseTurns(turn);
        pastMovementIndexes.RemoveRange(turn, pastMovementIndexes.Count - turn);
        if (turn > 0) nextMovementIndex = pastMovementIndexes[turn - 1] + 1;
        else nextMovementIndex = 0;
    }


    /// <summary>
    /// Checks if there is someting to shoot.
    /// </summary>
    /// <returns></returns>
    private RaycastHit2D RShoot()
    {
        bc2D.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        bc2D.enabled = true;

        return hit;
    }

}
