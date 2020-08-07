using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraColor : MonoBehaviour {

    [SerializeField]
    List<Gradient> gradient;
    [SerializeField]
    float duration;
    private float t1, t2;
    private float value;

    // Use this for initialization
    void Start () {
        t1    = 0f;
        t2    = 1f;
        value = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (t1 >= t2)
        {
            value = Mathf.Lerp(1f, 0f, 1-t2);
            t2 -= Time.deltaTime / duration;
            Color color = gradient[0].Evaluate(value);
            Camera.main.backgroundColor = color;
            if (t2 <= 0f)
            {
                t1 = 0f;
                t2 = 1f;
            }
        }
        else
        {
            value = Mathf.Lerp(0f, 1f, t1);
            t1 += Time.deltaTime / duration;
            Color color = gradient[0].Evaluate(value);
            Camera.main.backgroundColor = color;
        }        
    }
}
