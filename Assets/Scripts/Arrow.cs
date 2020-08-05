using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    [SerializeField]
    private float speed;
    [SerializeField]
    private float amplitude;

    // Update is called once per frame
    void Update()
    {
        float x = amplitude * Mathf.Sin(speed * Time.deltaTime * Time.time);
        transform.position += x * Vector3.right;
    }
}
