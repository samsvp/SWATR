using System.Collections;
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

    [SerializeField]
    private Enemy boss;

    // How many turns have passed
    public int turns = 0;
    [SerializeField]
    public Text turnsText;

    // Grenade Manager
    [HideInInspector]
    public bool grenadeSet = false;
    [HideInInspector]
    public int grenadeCountdown;
    
    // Checks for level completion
    private bool allHostagesSaved = false;
    private bool allEnemiesSecured = false;

    // GUI
    private GameObject restartGUI;
    private GameObject levelCompletedGUI;

    // Audio
    private AudioSource audioSource;

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
        grenadeCountdown = 2;
    }


    private void Start()
    {
        restartGUI = GameObject.FindGameObjectWithTag("RestartCanvas");
        levelCompletedGUI = GameObject.FindGameObjectWithTag("LevelCompleted");

        restartGUI.SetActive(false);
        levelCompletedGUI.SetActive(false);

        audioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        if (wait || NPCsMoving)
            if (Player.instance.turnButton.activeSelf && (Input.GetMouseButtonDown(1) 
                                                        || Input.GetKeyDown(KeyCode.LeftControl)))
                Player.instance.Rewind();
            else return;
        else if (playersTurn)
            Player.instance.GetInput();
        else
            StartCoroutine(ChooseNPCsAction());
    }


    // Called by each new NPC and familiar created
    public void AddNPCToList(NPC script)
    {
        NPCs.Add(script);
    }


    private IEnumerator ChooseNPCsAction()
    {
        if (grenadeSet) grenadeCountdown--;

        turnsText.text = (++turns).ToString();

        NPCsMoving = true;

        for (int i = 0; i < NPCs.Count; i++)
        {
            NPCs[i].ChooseAction();
        }

        yield return null;
        yield return new WaitWhile(() => NPCs.Any((npc) => npc.isPerformingAction));
        
        NPCsMoving = false;
        playersTurn = true;

        // Wait a for fixed update to check collision
        wait = true;
        yield return new WaitForFixedUpdate();
        wait = false;
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


    public void AllHostagesSaved()
    {
        allHostagesSaved = NPCs.Where(NPC => NPC is Hostage).All(npc => ((Hostage)npc).IsHostageSaved());
        LevelCompleted();
    }


    public void AllEnemiesSecured()
    {
        allEnemiesSecured = NPCs.Where(NPC => NPC is Enemy).All(npc => !npc.isAlive);
        LevelCompleted();
    }


    private void LevelCompleted()
    {
        if (SceneManager.GetActiveScene().name == "Level4" && !boss.isAlive)
        {
            levelCompletedGUI.SetActive(true);

            DisplayLvl4Score();

            SaveManager.Save(SceneManager.GetActiveScene().buildIndex + 1);
            StartCoroutine(GoToNextLevel());
        }
        else if (allEnemiesSecured && allHostagesSaved)
        {
            levelCompletedGUI.SetActive(true);

            DisplayScore();

            SaveManager.Save(SceneManager.GetActiveScene().buildIndex + 1);
            StartCoroutine(GoToNextLevel());
        }
    }


    private void DisplayScore()
    {
        List<Enemy> enemies = NPCs.Where(npc => npc is Enemy).Cast<Enemy>().ToList();
        List<Hostage> hostages = NPCs.Where(npc => npc is Hostage).Cast<Hostage>().ToList();

        int enemiesKnockedOut = enemies.Where(enemy => enemy.isKnockedOut).Count();
        int score = (int)Mathf.Floor(enemiesKnockedOut / (float)enemies.Count * 50) + 50;

        var levelCompletedGUIText = levelCompletedGUI.transform.GetChild(0).gameObject.GetComponent<Text>();
        levelCompletedGUIText.text = "Level completed!\n\nScore: " + score +
            "\nKnocked Out enemies: " + enemiesKnockedOut + "/" + enemies.Count +
            "\nHostages Saved: " + hostages.Count + "/" + hostages.Count +
            "\nTurns: " + turns +
            "\n\nPress space or left mouse to continue!";
    }


    private void DisplayLvl4Score()
    {
        List<Enemy> enemies = NPCs.Where(npc => npc is Enemy).Cast<Enemy>().ToList();
        int enemiesKnockedOut = enemies.Where(enemy => enemy.isKnockedOut).Count();
        int enemiesKilled = enemies.Where(enemy => !enemy.isKnockedOut && !enemy.isAlive).Count();
        
        int score;
        if (enemiesKilled <= 3) score = 100;
        else score = (int)Mathf.Floor(enemiesKnockedOut / (float)enemies.Count * 50) + 50;

        var levelCompletedGUIText = levelCompletedGUI.transform.GetChild(0).gameObject.GetComponent<Text>();
        levelCompletedGUIText.text = "Level completed!\n\nScore: " + score +
            "\nKnocked Out enemies: " + enemiesKnockedOut + "/" + enemies.Count +
            "\nTurns: " + turns +
            "\n\nPress space or left mouse to continue!";
    }
    

    private IEnumerator GoToNextLevel()
    {
        wait = true;

        AsyncOperation asyncLoad;

        while (true)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
            }
            yield return null;
        }
    }
    

    public void PlayClip(AudioClip clip)
    {

    }


    public void EnableRestartGUI()
    {
        restartGUI.SetActive(true);
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
