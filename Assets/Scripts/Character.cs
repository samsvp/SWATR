using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveTime = 0.03f;
    // Actions
    [HideInInspector]
    public bool isPerformingAction;
    
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

    // Health
    [HideInInspector]
    public bool isAlive = true;
    [HideInInspector]
    public bool isKnockedOut = false;

    [SerializeField]
    protected Sprite mDeadSprite;

    // Audio
    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip shotgunClip;
    [SerializeField]
    protected AudioClip deathClip;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        bc2D = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        sprite = render.sprite;

        pastMovements.Add(transform.position);
        pastOrientations.Add(transform.localEulerAngles);

        inverseMoveTime = 1f / moveTime;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }


    protected virtual bool Move(int xDir, int yDir)
    {
        if (!isAlive)
        {
            pastMovements.Add(transform.position);
            pastOrientations.Add(transform.localEulerAngles);
            return true;
        }
        Vector2 start = transform.position;
        Vector2 end = start + 2 * new Vector2(xDir, yDir);

        // Disable collider so the ray doesn't collide with the object that's emitting it
        bc2D.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(start, end);
        bc2D.enabled = true;

        if (hit.transform == null)
        {
            // Change transform rotation
            transform.localEulerAngles = GetDirectionAngles(xDir, yDir);

            StartCoroutine(SmoothMovement(end));
            // Add to the list of past movements
            pastMovements.Add(end);
            pastOrientations.Add(transform.localEulerAngles);
            return true;
        }
        return false;
    }


    protected Vector3 GetDirectionAngles(int x, int y)
    {
        if (x == 0) return y > 0 ? up : down;
        else return x > 0 ? right : left;
    }


    protected virtual IEnumerator SmoothMovement(Vector3 end)
    {
        isPerformingAction = true;
        bc2D.enabled = false;

        Vector3 target = new Vector3((int)end.x, (int)end.y);
        float sqrRemainingDistance = (transform.position - target).sqrMagnitude;
        while (sqrRemainingDistance > 0.0001f)
        {
            // Move towards the positon we want
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, target, inverseMoveTime * Time.fixedDeltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - target).sqrMagnitude;
            yield return new WaitForFixedUpdate(); // Wait for fixed update before continuing the loop
        }
        yield return null;
        transform.position = target;

        bc2D.enabled = true;
        isPerformingAction = false;
    }


    public virtual void EraseTurns(int turn)
    {
        pastMovements.RemoveRange(turn + 1, pastMovements.Count - turn - 1);
        pastOrientations.RemoveRange(turn + 1, pastOrientations.Count - turn - 1);
    }


    public virtual void Shoot()
    {
        if (!isAlive) return;

        isPerformingAction = true;
        BreakWindow();
        StartCoroutine(CShoot(shotgunClip));
    }


    public virtual void Shoot(AudioClip clip)
    {
        if (!isAlive) return;

        isPerformingAction = true;
        BreakWindow();
        StartCoroutine(CShoot(clip));
    }


    protected virtual IEnumerator CShoot(AudioClip clip)
    {
        animator.enabled = true;
        animator.SetBool("Shoot", true);

        audioSource.clip = clip;
        audioSource.Play();

        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        animator.SetBool("Shoot", false);
        yield return null;
        animator.enabled = false;
        isPerformingAction = false;
    }


    /// <summary>
    /// Breaks a window if it's hit
    /// </summary>
    protected virtual void BreakWindow()
    {
        bc2D.enabled = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up);
        bc2D.enabled = true;

        foreach (var hit in hits)
        {
            if (hit.collider.name.Contains("Window"))
                hit.collider.GetComponent<Window>().Break();
            else break;
        }
                    
    }


    public virtual void TakeDamage()
    {
        // Disable colliders (remember to reactivate it if it comes back to live
        // through a rewind)
        bc2D.enabled = false;
        
        isAlive = false;
        StartCoroutine(CTakeDamage());
    }


    protected virtual IEnumerator CTakeDamage()
    {
        animator.enabled = true;
        animator.SetBool("Death", true);

        audioSource.clip = deathClip;
        audioSource.Play();

        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        yield return null;
        animator.enabled = false;
        render.sprite = mDeadSprite;
    }


    public virtual void ClearTurns()
    {
        pastMovements.Clear();
        pastOrientations.Clear();

        pastMovements.Add(transform.position);
        pastOrientations.Add(transform.localEulerAngles);
    }
}
