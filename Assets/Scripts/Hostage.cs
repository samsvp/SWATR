using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hostage : NPC
{
    public LayerMask highlightMask;

    private int savedOnTurn = -1;
    private int killedOnTurn = -1;

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

        captiveSprite = render.sprite;

        textMesh = transform.GetChild(0).GetChild(0).GetComponent<TextMesh>();
        outline = transform.GetChild(0).GetComponent<TextMesh>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
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
    }


    public void UpdateGUI()
    {
        if (isSaved)
        {
            render.sprite = savedSprite;
            textMesh.text = "Saved!";
            outline.text = "Saved!";
        }
        if (isAlive)
        {
            textMesh.text = "Help!";
            outline.text = "Help!";
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
        else if (isAlive) killedOnTurn = -1;
        UpdateGUI();
    }


    public override void Rewind(int turn)
    {
        if (isSaved) { if (savedOnTurn > turn) UnSaveHostage(); }
        if (savedOnTurn == turn) SaveHostage();
        else if (killedOnTurn <= turn) KillHostage();
        else if (killedOnTurn > turn) ResurrectHostage();

        UpdateGUI();
    }


    public void ResetTurn(int turn)
    {
        if (!isSaved) KillHostage();
    }


    public bool IsHostageSaved()
    {
        if (enemies.Any(e => e.isAlive) || isDead) return false;
        SaveHostage();
        return true;
    }


    public void SaveHostage()
    {
        if (isDead || isSaved) return;

        isSaved = true;
        isDead = false;

        audioSource.clip = savedClip;
        audioSource.Play();

        savedOnTurn = GameManager.instance.turns;
        UpdateGUI();
    }


    public void UnSaveHostage()
    {
        render.sprite = captiveSprite;
        isSaved = false;
        UpdateGUI();
    }


    public void EraseSave()
    {
        print("hello");
        render.sprite = captiveSprite;
        isSaved = false;
        isDead = false;
        savedOnTurn = -1;
        UpdateGUI();
    }


    public void ResurrectHostage()
    {
        render.sprite = captiveSprite;
        isDead = false;
        isSaved = false;
        isAlive = true;

        // This fixes on collision enter bugs
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up,
            0.1f, highlightMask);
        if (hit.transform != null)
            hit.transform.SendMessage("OnDisable", SendMessageOptions.DontRequireReceiver);

        bc2D.enabled = true;

    }


    public void KillHostage()
    {
        if (isDead) return;

        render.sprite = deadSprite;
        isDead = true;
        isSaved = false;
        isAlive = false;
        bc2D.enabled = false;

        killedOnTurn = GameManager.instance.turns;

        // This fixes on collision enter bugs
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up,
            0.1f, highlightMask);
        if (hit.transform != null)
            hit.transform.SendMessage("OnEnable", SendMessageOptions.DontRequireReceiver);

        UpdateGUI();
    }


    public override void TakeDamage()
    {
        KillHostage();
    }

    
    public override void ClearTurns()
    {
        isDead = false;
        
        UpdateGUI();
        
        base.ClearTurns();
    }
    
}
