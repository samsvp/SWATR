using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{

    private int index;
    private Transform nextChild;
    private Transform parent;
    private bool lastSibling = false;

    // Start is called before the first frame update
    void Start()
    {
        index = transform.GetSiblingIndex();
        parent = transform.parent;
        if (index < parent.childCount - 1) nextChild = parent.GetChild(index + 1);
        else lastSibling = true;
    }
    
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!lastSibling) nextChild.gameObject.SetActive(false);
    }


    void OnTriggerExit2D(Collider2D col)
    {
        if (!lastSibling) nextChild.gameObject.SetActive(true);
    }


    public void OnEnable()
    {
        if (!lastSibling) nextChild.gameObject.SetActive(true);
    }


    public void OnDisable()
    {
        if (!lastSibling) nextChild.gameObject.SetActive(false);
    }

}
