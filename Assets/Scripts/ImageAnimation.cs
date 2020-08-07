using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class ImageAnimation : MonoBehaviour
{

    public List<Sprite> sprites = new List<Sprite>();
    public int spritesPerFrame = 3;
    public bool loop = true;

    private int index = 0;
    private Image image;


    void Awake()
    {
        image = GetComponent<Image>();
    }


    private IEnumerator PlayAnimation()
    {
        image.enabled = true;
        
        for (int i = 0; i < sprites.Count; i++)
        {
            for (int frame = 0; frame < spritesPerFrame; frame++) yield return new WaitForFixedUpdate();
            image.sprite = sprites[i];
        }

        image.enabled = false;

        if (loop) StartCoroutine(PlayAnimation());
    }


    void OnEnable()
    {
        StartCoroutine(PlayAnimation());
    }

}