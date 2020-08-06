using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public static string path;
    public static int currentLevel;


    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        path = Application.persistentDataPath + "/level.txt";

        if (!System.IO.File.Exists(path)) currentLevel = 1;
        else currentLevel = Int32.Parse(System.IO.File.ReadAllText(path));
    }


    public static void Save(int level)
    {
        System.IO.File.WriteAllText(path, level.ToString());
    }

}
