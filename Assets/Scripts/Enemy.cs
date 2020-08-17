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

    [SerializeField]
    private Hostage hostage;
    [SerializeField]
    private Enemy enemyWithHostage;

    private bool isAlert = false;
    private List<bool> alertTurns = new List<bool>() { false };

    [SerializeField]
    private Sprite knockedOutSprite;

    private Action nextAction;
    private Transform nextActionTransform;
    private SpriteRenderer nextActionRenderer;
    [SerializeField]
    private Sprite[] actionSprites;

    [SerializeField]
    protected AudioClip knockedOutClip;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        nextActionTransform = transform.GetChild(0);
        nextActionRenderer = nextActionTransform.gameObject.GetComponent<SpriteRenderer>();
        SetNextAction(MoveNPC);

        StartCoroutine(MoveNextActionTransform());
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }


    private IEnumerator MoveNextActionTransform()
    {
        while (true)
        {
            float sin = 0.005f * Mathf.Sin(Time.time);
            var offset = new Vector3(0, sin);
            nextActionTransform.localPosition += offset;
            yield return new WaitForFixedUpdate();
        }
    }


    public override void ChooseAction()
    {
        aliveTurns.Add(isAlive);
        alertTurns.Add(isAlert);
        knockedOutTurns.Add(isKnockedOut);

        if (!render.isVisible || !isAlive)
        {
            MoveNPC();
            return;
        }

        nextAction();
        SetNextAction(MoveNPC);
    }


    private void SetNextAction(Action action)
    {
        if (action == MoveNPC)
        {
            int nextIndex = (nextMovementIndex) % movement.Count;
            Vector2 nextMovement = movement[nextIndex];
            Vector3 angles = GetDirectionAngles((int)nextMovement.x, (int)nextMovement.y);
            nextActionTransform.eulerAngles = angles;

            if (nextMovement == Vector2.zero) nextActionRenderer.sprite = actionSprites[4];
            else nextActionRenderer.sprite = actionSprites[0];
            
        }
        else if (action == Shoot)
        {
            nextActionTransform.localEulerAngles = Vector3.zero;
            nextActionRenderer.sprite = actionSprites[1];
        }

        nextAction = action;
    }


    public void SetAlert(bool alert=true)
    {
        isAlert = alert;

        // Add the alert turn so that if the player rewind back to when an enemy is killed
        // the last element of alertTurns is true
        alertTurns.RemoveAt(0);
        alertTurns.Add(isAlert);

        if (isAlert) SetNextAction(Shoot);
        else SetNextAction(MoveNPC);
    }


    public void ShootNextTurn()
    {
        SetNextAction(Shoot);
    }


    public override void MoveNPC()
    {
        if (!isAlive)
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
        if (!isAlive && aliveTurns[turn]) Resurrect();
        else if (isAlive && !aliveTurns[turn] && !knockedOutTurns[turn]) TakeDamage();
        else if (isAlive && !aliveTurns[turn] && knockedOutTurns[turn]) KnockOut();
    }


    public override void EraseTurns(int turn)
    {
        base.EraseTurns(turn);

        aliveTurns.RemoveRange(turn + 1, aliveTurns.Count - turn - 1);
        alertTurns.RemoveRange(turn + 1, alertTurns.Count - turn - 1);
        knockedOutTurns.RemoveRange(turn + 1, knockedOutTurns.Count - turn - 1);
        pastMovementIndexes.RemoveRange(turn, pastMovementIndexes.Count - turn);
        
        SetAlert(alertTurns[alertTurns.Count - 1]);

        if (turn > 0) nextMovementIndex = pastMovementIndexes[turn - 1] + 1;
        else nextMovementIndex = 0;
    }


    public void Resurrect()
    {
        animator.enabled = true;
        animator.SetBool("Death", false);
        animator.SetBool("TaserKnockOut", false);
        isAlive = true;
        isKnockedOut = false;
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


    protected override IEnumerator CShoot(AudioClip clip)
    {
        yield return StartCoroutine(base.CShoot(clip));
        if (isAlive && !isAlert) Player.instance.SendMessage("TakeDamage");
        else if (isAlive && isAlert) hostage.SendMessage("TakeDamage");
    }


    protected override IEnumerator CTakeDamage()
    {
        yield return StartCoroutine(base.CTakeDamage());

        // Kill hostage if enemy is killed by gun shot
        if (enemyWithHostage != null && enemyWithHostage.isAlive)
        {
            enemyWithHostage.SetAlert();
        }

        nextActionTransform.localEulerAngles = Vector3.zero;
        nextActionRenderer.sprite = actionSprites[2];

        GameManager.instance.AllEnemiesSecured();
        GameManager.instance.AllHostagesSaved();
    }


    protected override IEnumerator CTaserKnockOut()
    {
        audioSource.clip = knockedOutClip;
        audioSource.Play();

        yield return StartCoroutine(base.CTaserKnockOut());

        render.sprite = knockedOutSprite;
        nextActionTransform.localEulerAngles = Vector3.zero;
        nextActionRenderer.sprite = actionSprites[3];

        GameManager.instance.AllEnemiesSecured();
        GameManager.instance.AllHostagesSaved();
    }


    public override void ClearTurns()
    {
        pastMovementIndexes.Clear();
        nextMovementIndex = 0;

        alertTurns.Clear();
        isAlert = false;
        alertTurns.Add(isAlert);

        base.ClearTurns();
    }

}
