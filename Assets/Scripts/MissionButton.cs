using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionButton : MonoBehaviour
{

    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(SaveManager.currentLevel);
    }

    public void StartMission()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
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
