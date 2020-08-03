using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Continuously rotates a game object
/// </summary>
public class RotateGO : MonoBehaviour
{

    [SerializeField]
    private int maxAngle = 10;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            float step = 0.025f;
            for (float i = 0; i <= maxAngle; i += step)
            {
                transform.localEulerAngles = new Vector3(0, 0, i);
                yield return new WaitForEndOfFrame();
            }
            for (float i = maxAngle; i >= -maxAngle; i -= step)
            {
                transform.localEulerAngles = new Vector3(0, 0, i);
                yield return new WaitForEndOfFrame();
            }
            for (float i = -maxAngle; i < 0; i += step)
            {
                transform.localEulerAngles = new Vector3(0, 0, i);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForFixedUpdate();
        }
    }

}
