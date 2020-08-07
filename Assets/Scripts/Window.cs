using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{

    [SerializeField]
    private Sprite breakedSprite;

    private bool isBreaked = false;
    
    public void Break()
    {
        if (isBreaked) return;

        GetComponent<AudioSource>().Play();
        GetComponent<SpriteRenderer>().sprite = breakedSprite;
        isBreaked = true;
    }
}
