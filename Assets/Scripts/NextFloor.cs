using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NextFloor : MonoBehaviour
{

    [SerializeField]
    private GameObject nextFloor;
    [SerializeField]
    private Vector3 nextFloorPosition;

    [SerializeField]
    private List<Enemy> enemies;
    [SerializeField]
    private List<Enemy> nextFloorEnemies;
    [SerializeField]
    private List<Hostage> hostages;
    [SerializeField]
    private List<Hostage> nextFloorHostages;

    [SerializeField]
    private GameObject arrow;

    private BoxCollider2D bc2D;

    private bool canGoToNextRoom = false;

    // Start is called before the first frame update
    void Start()
    {
        bc2D = GetComponent<BoxCollider2D>();
        bc2D.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canGoToNextRoom && enemies.All(e => !e.alive) && hostages.All(h => h.isSaved))
        {
            arrow.SetActive(true);
            bc2D.enabled = true;
            canGoToNextRoom = true;
        }
        else if (canGoToNextRoom)
        {
            if (Vector2.Distance(Player.instance.transform.position, transform.position) < 4)
            {
                StartCoroutine(Teleport());
            }
        }
    }


    private IEnumerator Teleport()
    {
        yield return new WaitWhile(() => Player.instance.isPerformingAction);

        GameManager.instance.turns = 0;

        foreach (var h in nextFloorHostages) h.ClearTurns();
        foreach (var e in nextFloorEnemies) e.ClearTurns();

        Player.instance.ClearTurns();         
        Player.instance.transform.position = nextFloorPosition;
        Destroy(gameObject);
    }

}
