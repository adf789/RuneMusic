using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStar : MonoBehaviour
{
    public int addHp, addAtk, addDef, addGold, addExp;
    public static MainHomeUIManager uiManager;
    [SerializeField]
    private int number;

    private void Start()
    {
        if(uiManager == null)
            uiManager = GameObject.Find("GameManager").GetComponent<MainHomeUIManager>();
    }

    public void OnClick()
    {
        IsSelected(true);
        uiManager.OnSelectStage(this);
    }

    public void IsSelected(bool selected)
    {
        transform.GetChild(0).gameObject.SetActive(selected);
    }

    public int StageNumber
    {
        get
        {
            return number;
        }
    }
}
