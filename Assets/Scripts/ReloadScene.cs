using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    public static void Reload()
    { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
}
