using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveTime = 0.03f;
    [HideInInspector]
    public bool moving;
    
    protected BoxCollider2D bc2D;
    protected Rigidbody2D rb2D;

    // Animator instance
    protected Animator animator;
    private float inverseMoveTime;
    protected SpriteRenderer render;
    protected Sprite sprite;

    // Movement memory
    protected List<Vector2> pastMovements = new List<Vector2>();
    protected List<Vector3> pastOrientations = new List<Vector3>();
    // Movement rotation
    protected static Vector3 left = new Vector3(0, 0, 90);
    protected static Vector3 right = new Vector3(0, 0, -90);
    protected static Vector3 up = new Vector3(0, 0, 0);
    protected static Vector3 down = new Vector3(0, 0, 180);

    // Start is called before the first frame update
    protected virtual void Start()
    {
        bc2D = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        sprite = render.sprite;

        pastMovements.Add(transform.position);
        pastOrientations.Add(transform.localEulerAngles);

        inverseMoveTime = 1f / moveTime;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }


    protected virtual void Move(int xDir, int yDir)
    {
        Vector2 start = transform.position;
        Vector2 end = start + 2 * new Vector2(xDir, yDir);

        // Disable collider so the ray doesn't collide with the object that's emitting it
        bc2D.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(start, end);
        bc2D.enabled = true;

        if (hit.transform == null)
        {
            // Change transform rotation
            if (xDir == 0) transform.localEulerAngles = yDir > 0 ? up : down;
            else transform.localEulerAngles = xDir > 0 ? right : left;

            StartCoroutine(SmoothMovement(end));
            // Add to the list of past movements
            pastMovements.Add(end);
            pastOrientations.Add(transform.localEulerAngles);
        }
    }


    protected IEnumerator SmoothMovement(Vector3 end)
    {
        moving = true;

        Vector3 target = new Vector3((int)end.x, (int)end.y);
        float sqrRemainingDistance = (transform.position - target).sqrMagnitude;

        while (sqrRemainingDistance > 0.01f)
        {
            // Move towards the positon we want
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - target).sqrMagnitude;
            yield return null; // Wait for a frame before continuing the loop
        }

        transform.position = target;
        // Shoot when the player finishes moving
        if (this is Player) GameManager.instance.Shoot();

        moving = false;
    }


    public virtual void EraseTurns(int turn)
    {
        pastMovements.RemoveRange(turn + 1, pastMovements.Count - turn - 1);
        pastOrientations.RemoveRange(turn + 1, pastOrientations.Count - turn - 1);
    }


    public virtual void Shoot()
    {
        Debug.Log("Pew pew");
    }
}
