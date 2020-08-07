using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Sprite captiveSprite;
    [SerializeField]
    private Sprite deadSprite;
    [SerializeField]
    private Sprite savedSprite;

    // Text variables
    private TextMesh textMesh;
    private TextMesh outline;

    [SerializeField]
    private AudioClip savedClip;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        isPerformingAction = false;
        turnsLeft = maxTurnsLeft;

        captiveSprite = render.sprite;

        textMesh = transform.GetChild(0).GetChild(0).GetComponent<TextMesh>();
        outline = transform.GetChild(0).GetComponent<TextMesh>();

        UpdateTurnsLeftGUI(maxTurnsLeft);
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    void LateUpdate()
    {
        if (turnsLeft == -1) IsHostageSaved();
    }

    public override void ChooseAction()
    {
        MoveNPC();
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
            render.sprite = savedSprite;
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

        if (!isSaved) savedOnTurn = -1;
        UpdateTurnsLeftGUI(maxTurnsLeft - turn);
    }


    public override void Rewind(int turn)
    {
        turnsLeft = maxTurnsLeft - turn;
        UpdateTurnsLeftGUI(turnsLeft);
        
        if (isSaved)
        {
            if (savedOnTurn > turn) UnSaveHostage();
            return;
        }
        else if (savedOnTurn == turn)
        {
            SaveHostage();
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


    public bool IsHostageSaved()
    {
        if (enemies.Any(e => e.alive)) return false;
        SaveHostage();
        return true;
    }


    public void SaveHostage()
    {
        if ((isDead && turnsLeft < -1) || isSaved) return;

        isSaved = true;
        isDead = false;

        audioSource.clip = savedClip;
        audioSource.Play();

        savedOnTurn = GameManager.instance.turns;
        UpdateTurnsLeftGUI(savedOnTurn);
    }


    public void UnSaveHostage()
    {
        render.sprite = captiveSprite;
        isSaved = false;
        UpdateTurnsLeftGUI(maxTurnsLeft - GameManager.instance.turns);
    }


    public void EraseSave()
    {
        render.sprite = captiveSprite;
        isSaved = false;
        isDead = false;
        savedOnTurn = -1;
        UpdateTurnsLeftGUI(GameManager.instance.turns);
    }


    public void ResurrectHostage()
    {
        render.sprite = captiveSprite;
        isDead = false;
        isSaved = false;
    }


    public void KillHostage()
    {
        render.sprite = deadSprite;
        isDead = true;
        isSaved = false;
    }


    public override void ClearTurns()
    {
        isDead = false;

        turnsLeft = maxTurnsLeft;
        UpdateTurnsLeftGUI(maxTurnsLeft);
        
        base.ClearTurns();
    }
}
