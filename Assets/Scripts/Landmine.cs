using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour
{

    [SerializeField]
    private int radius = 1;
    [SerializeField]
    private GameObject mineExplosion;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.landmineSet = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.landmineCountdown == 0)
        {
            GameManager.instance.landmineCountdown = 2;
            GameManager.instance.landmineSet = false;
            Detonate();
        }
        else if (!GameManager.instance.landmineSet) // This is so two grenades can be active on the same time
        {
            GameManager.instance.landmineSet = true;
            GameManager.instance.landmineCountdown--;
        }
    }
    

    public void Detonate()
    {
        int n = 2 * radius + 1;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x == 0 || y == 0)
                {
                    var mExplosion =
                        Instantiate(mineExplosion, transform.position + 2 * new Vector3(x, y), Quaternion.identity).
                        GetComponent<Explosion>();
                    mExplosion.damageFunctionName = "KnockOut";
                }
            }
        }
        Destroy(gameObject);
    }
}
