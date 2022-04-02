using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawStoreEffect : MonoBehaviour
{
    public GameObject keyObj, LockObj, IdleEffect, boxObj;
    public DrawStoreUIController uiController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onKeyAndLockActive()
    {
        boxObj.SetActive(false);
        keyObj.GetComponent<CanvasGroup>().alpha = 1.0f;
        LockObj.GetComponent<CanvasGroup>().alpha = 1.0f;
        IdleEffect.SetActive(true);
        uiController.ActiveDrawButton(true);
    }

    public void offKeyAndLockActive()
    {
        keyObj.GetComponent<CanvasGroup>().alpha = 0.0f;
        LockObj.GetComponent<CanvasGroup>().alpha = 0.0f;
        IdleEffect.SetActive(false);
    }

    public void itemBoxOn()
    {
        boxObj.SetActive(true);
    }
}
