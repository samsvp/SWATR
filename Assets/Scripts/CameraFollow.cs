using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{


    // Set CameraFollow so it is unique
    public static CameraFollow instance = null;

    private GameObject player;

    [SerializeField]
    private float speed;

    private float offset = 0.5f;

    public bool lockX;
    public bool lockY;


    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject); // Destroy the CameraFollow if we end up with two instances of it
    }


    void Start()
    {
        player = Player.instance.gameObject;
    }


    void FixedUpdate()
    {
        MoveCamera();
    }


    private void MoveCamera()
    {
        float interpolation = speed * Time.deltaTime;

        Vector3 position = transform.position;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(new Vector2(this.transform.position.y, this.transform.position.x), new Vector2(mouseWorldPos.y, mouseWorldPos.x)) > offset)
            {
                position.y = Mathf.Lerp(this.transform.position.y, mouseWorldPos.y, interpolation);
                position.x = Mathf.Lerp(this.transform.position.x, mouseWorldPos.x, interpolation);
            }

            if (Mathf.Abs(player.transform.position.x - position.x) > 16)
                position.x = transform.position.x;
            if (Mathf.Abs(player.transform.position.y - position.y) > 16)
                position.y = transform.position.y;
        }
        else
        {
            if (!lockX)
                position.x = Mathf.Lerp(transform.position.x, player.transform.position.x, interpolation);
            if (!lockY)
                position.y = Mathf.Lerp(transform.position.y, player.transform.position.y, interpolation);

        }

        transform.position = position;
    }


    public void LockCamera()
    {
        lockX = true;
        lockY = true;
    }

    public void UnlockCamera()
    {
        lockX = false;
        lockY = false;
    }
}
