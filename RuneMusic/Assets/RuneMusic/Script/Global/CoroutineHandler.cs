using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHandler : MonoBehaviour
{
    private CoroutineHandler() { }

    private static CoroutineHandler instance;
    public static CoroutineHandler Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = GetComponent<CoroutineHandler>();
        DontDestroyOnLoad(this);
    }
}
