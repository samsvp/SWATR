﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour
{

    [SerializeField]
    private int radius = 1;
    [SerializeField]
    private GameObject mineExplosion;
    

    public void Detonate()
    {
        int n = 2 * radius + 1;
        
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Mathf.Abs(x) == Mathf.Abs(y))
                {
                    var mExplosion =
                        Instantiate(mineExplosion, transform.position + 2 * new Vector3(x, y), Quaternion.identity).
                        GetComponent<Explosion>();
                    mExplosion.damageFunctionName = "TakeDamage";
                }
            }
        }
        Destroy(gameObject);
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Vision") && !col.CompareTag("VisionIgnore")) Detonate();
    }
}
