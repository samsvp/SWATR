using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    [SerializeField]
    private int radius = 1;
    [SerializeField]
    private GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.grenadeSet = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.grenadeCountdown == 0)
        {
            GameManager.instance.grenadeCountdown = 2;
            GameManager.instance.grenadeSet = false;
            Detonate();
        }
        else if (!GameManager.instance.grenadeSet) // This is so two grenades can be active on the same time
        {
            GameManager.instance.grenadeSet = true;
            GameManager.instance.grenadeCountdown--;
        }
    }


    public void Detonate()
    {
        int n = 2 * radius + 1;
        
        for (int x = -radius; x <= radius; x++)
            for (int y = -radius; y <= radius; y++)
                Instantiate(explosion, transform.position + 2 * new Vector3(x, y), Quaternion.identity);

        Destroy(gameObject);
    }
    
}
