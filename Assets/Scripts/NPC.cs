using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : Character
{
    private Transform target;

    protected List<bool> aliveTurns = new List<bool>() { true };
    protected List<bool> knockedOutTurns = new List<bool>() { false };

    protected Vector3 initialPosition;
    protected Vector3 initialRotation;

    // Use this for initialization
    protected override void Start()
    {
        GameManager.instance.AddNPCToList(this); // Add this enemy to the list of enemies
        base.Start();

        initialPosition = transform.position;
        initialRotation = transform.localEulerAngles;
    }

    public virtual void KnockOut()
    {
        print("Knocked out");
        TaserKnockOut();
    }


    public virtual void TaserKnockOut()
    {
        bc2D.enabled = false;

        alive = false;
        knockedOut = true;
        StartCoroutine(CTaserKnockOut());
    }


    protected virtual IEnumerator CTaserKnockOut()
    {
        animator.enabled = true;
        animator.SetBool("TaserKnockOut", true);
        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        yield return null;
        animator.enabled = false;
    }


    public override void ClearTurns()
    {
        pastMovements.Clear();
        pastOrientations.Clear();

        pastMovements.Add(initialPosition);
        pastOrientations.Add(initialRotation);

        aliveTurns.Clear();
        aliveTurns.Add(true);

        knockedOutTurns.Clear();
        knockedOutTurns.Add(false);
    }


    public abstract void ChooseAction();

    public abstract void MoveNPC();

    public abstract void Rewind(int turn);
}