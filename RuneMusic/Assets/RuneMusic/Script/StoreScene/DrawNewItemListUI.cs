using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 아이템 목록 추가
// 아이템 목록 삭제
// 목록 활성화 상태 변경
public class DrawNewItemListUI : MonoBehaviour
{
    private ObjectPulling<Transform> ItemList;
    [SerializeField]
    private Transform ItemObj, ItemObjParent;
    [SerializeField]
    private Text WhatCountNewText;
    private Vector3 baseSize = new Vector3(1.0f, 1.0f, 1.0f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddItem(SubRune item)
    {
        RectTransform itemT = ItemList.GetObject(ItemObjParent) as RectTransform;

        // 목록에 출력할 아이템 이미지
        itemT.GetChild(0).GetComponent<Image>().sprite = item.getItemImage();
        // 목록에 출력할 아이템 이름
        itemT.GetComponentInChildren<Text>().text = item.name;
        itemT.localScale = baseSize;
    }

    private void Clear()
    {
        ItemList.UnActive();
    }

    public void SetList(List<SubRune> runeList)
    {
        Clear();
        foreach(SubRune rune in runeList){
            AddItem(rune);
        }
    }

    public void ShowUI(bool isShow)
    {
        if (ItemList == null) ItemList = new ObjectPulling<Transform>(ItemObj, 5);
        if (!isShow) Clear();
        gameObject.SetActive(isShow);
    }

    public void SetNewCount(int count)
    {
        WhatCountNewText.text = count + " New!";
    }

}
