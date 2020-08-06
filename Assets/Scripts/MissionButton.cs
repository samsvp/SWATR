using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionButton : MonoBehaviour
{

    public void StartMission()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PointerEnter()
    {
        transform.localScale = new Vector3(1.3f, 1.3f);
    }

    public void PointerExit()
    {
        transform.localScale = Vector3.one;
    }
}
