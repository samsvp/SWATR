﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    // Set GameManager so it is unique
    public static GameManager instance = null;

    public float levelStartDelay = 2f;
    public float turnDelay = .1f;

    // Don't show on the editor
    [HideInInspector]
    public bool playersTurn = true;

    private bool wait = false;

    // If hidden enemies can't find player
    [HideInInspector]
    public bool hidden = false;

    private List<NPC> NPCs;
    private bool NPCsMoving;

    // How many turns have passed
    public int turns = 0;
    [SerializeField]
    public Text turnsText;

    public bool Wait
    {
        get
        {
            return wait;
        }

        set
        {
            wait = value;
        }
    }

    void Awake()
    {

        // Always use this code to ensure that there is no GameManager duplicate
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject); // Destroy the GameManager if we end up with two instances of it

        NPCs = new List<NPC>();
    }
    

    // Update is called once per frame
    void Update()
    {
        if (wait || NPCsMoving)
            if (Player.instance.turnButton.activeSelf && Input.GetMouseButtonDown(1)) Player.instance.Rewind();
            else return;
        else if (playersTurn)
            Player.instance.GetInput();
        else
            StartCoroutine(MoveNPCs());
    }

    // Called by each new NPC and familiar created
    public void AddNPCToList(NPC script)
    {
        NPCs.Add(script);
    }


    IEnumerator MoveNPCs()
    {
        turnsText.text = (++turns).ToString();

        NPCsMoving = true;

        for (int i = 0; i < NPCs.Count; i++)
        {
            NPCs[i].MoveNPC();
        }

        yield return null;
        yield return new WaitWhile(() => NPCs.All((npc) => npc.moving));
        
        NPCsMoving = false;
        playersTurn = true;
        // Wait one frame due a bug when entering doors
        wait = true;
        yield return null;
        wait = false;
    }


    /// <summary>
    /// Shoots the player
    /// </summary>
    public void Shoot()
    {
        for (int i = 0; i < NPCs.Count; i++)
        {
            if (NPCs[i] is Enemy) NPCs[i].Shoot();
        }
    }


    /// <summary>
    /// Rewind to the given turn
    /// </summary>
    public void Rewind(int turn)
    {
        turns = turn;
        for (int i = 0; i < NPCs.Count; i++)
        {
            NPCs[i].Rewind(turn);
        }
    }
    

    public void EraseTurns(int turn)
    {
        for (int i = 0; i < NPCs.Count; i++)
        {
            NPCs[i].EraseTurns(turn);
        }
    }


    /// <summary>
    /// Locks player input
    /// </summary>
    public void Lock()
    {
        wait = true;
    }


    /// <summary>
    /// Unlocks player input
    /// </summary>
    public void Unlock()
    {
        wait = false;
    }
}