using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stinger : MonoBehaviour
{

    [SerializeField]
    private int radius = 1;
    [SerializeField]
    private GameObject stingerExplosion;


    public void Detonate()
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x == 0 || y == 0)
                {
                    var mExplosion =
                        Instantiate(stingerExplosion, transform.position + 2 * new Vector3(x, y), Quaternion.identity).
                        GetComponent<Explosion>();
                    mExplosion.damageFunctionName = "KnockOut";
                }
            }
        }
        Destroy(gameObject);
    }
}
