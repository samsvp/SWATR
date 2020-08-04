using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Detonate());
    }


    private IEnumerator Detonate()
    {
        var animator = GetComponent<Animator>();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        Destroy(gameObject);
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        col.transform.SendMessage("TaserKnockOut", SendMessageOptions.DontRequireReceiver);
    }
}
