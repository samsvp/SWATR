using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{

    private int index;
    private Transform nextChild = null;
    private Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        index = transform.GetSiblingIndex();
        parent = transform.parent;
        if (index < parent.childCount - 1) nextChild = parent.GetChild(index + 1);
    }
    

    void OnTriggerEnter2D(Collider2D col)
    {
        if (nextChild != null && !col.CompareTag("Vision") && !col.CompareTag("VisionIgnore"))
            nextChild.gameObject.SetActive(false);
        if (col.CompareTag("Player")) parent.SendMessage("ShootNextTurn");
    }


    void OnTriggerExit2D(Collider2D col)
    {
        if (nextChild != null && !col.CompareTag("Vision")) nextChild.gameObject.SetActive(true);
    }


    public void OnEnable()
    {
        if (nextChild != null) nextChild.gameObject.SetActive(true);
    }


    public void OnDisable()
    {
        if (nextChild != null) nextChild.gameObject.SetActive(false);
    }

}
