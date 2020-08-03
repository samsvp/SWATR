using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Continuously rotates a game object
/// </summary>
public class RotateGO : MonoBehaviour
{

    [SerializeField]
    private float relativeMaxAngle = 10;
    [SerializeField]
    private float step = 0.025f;
    private float localMaxAngle;
    private float z;

    // Use this for initialization
    void Start()
    {
        z = transform.localEulerAngles.z;
        localMaxAngle = relativeMaxAngle + z;
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            for (float i = z; i <= localMaxAngle; i += step)
            {
                transform.localEulerAngles = new Vector3(0, 0, i);
                yield return new WaitForEndOfFrame();
            }
            for (float i = localMaxAngle; i >= (z - relativeMaxAngle); i -= step)
            {
                transform.localEulerAngles = new Vector3(0, 0, i);
                yield return new WaitForEndOfFrame();
            }
            for (float i = (z - relativeMaxAngle); i < z; i += step)
            {
                transform.localEulerAngles = new Vector3(0, 0, i);
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForFixedUpdate();
        }
    }

}
