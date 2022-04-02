using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = new GameManager();
            return instance;
        }
    }

    private GameManager() { }

    public bool IsLocal = true;
    public EnumList.SCENE_NAME PrevScene { get; private set; }
    private DateTime idleTimer;
    public DateTime IdleTimer
    {
        get
        {
            return idleTimer;
        }
        set
        {
            idleTimer = value;
        }
    }

    public static void LoadScene(EnumList.SCENE_NAME scene)
    {
        try
        {
            if (SceneManager.GetActiveScene().buildIndex == (int)EnumList.SCENE_NAME.MainHomeScene)
            {
                Instance.IdleTimer = DateTime.Now;

            }

            Instance.PrevScene = (EnumList.SCENE_NAME)SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene((int)scene, LoadSceneMode.Single);
        }catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public static void LoadPrevScene()
    {
        LoadScene(Instance.PrevScene);
    }
}
