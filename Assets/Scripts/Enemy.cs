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
        aliveTurns.Add(alive);
        knockedOutTurns.Add(knockedOut);

        if (!render.isVisible || !alive)
        {
            MoveNPC();
            return;
        }

        RaycastHit2D hit = RShoot();

        if (hit.transform == null) MoveNPC();
        else if (hit.transform.CompareTag("Player"))
        {
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

        if (nextMovementIndex == movement.Count) nextMovementIndex = 0;
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
        if (turn < pastMovements.Count)
        {
            transform.position = pastMovements[turn];
            transform.localEulerAngles = pastOrientations[turn];
        }
        else
        {
            Rewind(turn - 1);
            return;
        }

        // Movement
        if (turn != GameManager.instance.turns) nextMovementIndex = pastMovementIndexes[turn];
        else if (turn != 0) nextMovementIndex = pastMovementIndexes[turn-1];

        // Check if enemy is alive
        if (!alive && aliveTurns[turn]) Resurrect();
        else if (alive && !aliveTurns[turn] && !knockedOutTurns[turn]) TakeDamage();
        else if (alive && !aliveTurns[turn] && knockedOutTurns[turn]) KnockOut();
    }


    public override void EraseTurns(int turn)
    {
        base.EraseTurns(turn);

        aliveTurns.RemoveRange(turn + 1, aliveTurns.Count - turn - 1);
        knockedOutTurns.RemoveRange(turn + 1, knockedOutTurns.Count - turn - 1);
        pastMovementIndexes.RemoveRange(turn, pastMovementIndexes.Count - turn);

        if (turn > 0) nextMovementIndex = pastMovementIndexes[turn - 1] + 1;
        else nextMovementIndex = 0;
    }


    public void Resurrect()
    {
        animator.enabled = true;
        animator.SetBool("Death", false);
        animator.SetBool("TaserKnockOut", false);
        alive = true;
        knockedOut = false;
    }


    /// <summary>
    /// Checks if there is someting to shoot.
    /// </summary>
    /// <returns></returns>
    private RaycastHit2D RShoot()
    {
        bc2D.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, Mathf.Infinity,
            Player.instance.highlightMask, -Mathf.Infinity, Mathf.Infinity);
        bc2D.enabled = true;
        
        return hit;
    }


    protected override IEnumerator CShoot()
    {
        yield return StartCoroutine(base.CShoot());
        if (alive) Player.instance.SendMessage("TakeDamage");
    }


    protected override IEnumerator CTakeDamage()
    {
        yield return StartCoroutine(base.CTakeDamage());
        GameManager.instance.AllHostagesSaved();
    }


    protected override IEnumerator CTaserKnockOut()
    {
        yield return StartCoroutine(base.CTaserKnockOut());
        GameManager.instance.AllHostagesSaved();
    }


    public override void ClearTurns()
    {
        pastMovementIndexes.Clear();
        nextMovementIndex = 0;

        base.ClearTurns();
    }

}
