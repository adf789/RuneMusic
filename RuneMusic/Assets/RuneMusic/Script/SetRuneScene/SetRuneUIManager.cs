using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class SetRuneUIManager : MonoBehaviour
{
    private void Start()
    {
        sb = new StringBuilder();
        InitInventory();
        GameDatas.IsLoadedSubRuneInventory = false;
        GameDatas.IsLoadedMainRuneInventory = false;
        MainNameText.text = ItemPref.instance.curMainRune.name;
    }

    private void Update()
    {
        if (!GameDatas.IsLoadedSubRuneInventory)
        {
            GameDatas.IsLoadedSubRuneInventory = true;
            ReArrangeEquipedSlot();
            ReArrangeInventorySlot();
            InitCharacterEquipModel();
        }
        if (!GameDatas.IsLoadedMainRuneInventory)
        {
            GameDatas.IsLoadedMainRuneInventory = true;
            ReArrangeMainRuneSlot();
        }
    }

    private void InitInventory()
    {
        EquipedSlot = new List<InventorySlot>();
        SubListSlot = new List<InventorySlot>();
        CreateSlot(ItemManager.InvenSize);

        for(int i = 0; i < TEquipedSlotParent.childCount; i++)
        {
            Transform obj = TEquipedSlotParent.GetChild(i);
            InventorySlot slot = obj.GetComponent<InventorySlot>();
            slot.SlotIndex = i;
            EquipedSlot.Add(slot);
        }
        CurType = EnumList.SUBRUNE_VIEW_TYPE.ALL;
    }

    private void CreateSlot(int size)
    {
        // 서브룬 슬롯
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(ObjSlot, TInventorySlotParent);

            InventorySlot slot = obj.GetComponent<InventorySlot>();
            slot.SlotIndex = i;
            SubListSlot.Add(slot);
        }
    }

    // 인벤토리 내 서브룬 슬롯 재정렬
    private void ReArrangeInventorySlot()
    {
        OnTabClick(CurType);
    }

    // 장착된 서브룬 슬롯 재정렬
    private void ReArrangeEquipedSlot()
    {
        List<SubRune> runeList = ItemPref.instance.curMainRune.subRunes;
        for (int i = 0; i < TEquipedSlotParent.childCount; i++)
        {
            bool isExist = i < (int)ItemPref.instance.curMainRune.grade;
            TEquipedSlotParent.GetChild(i).gameObject.SetActive(isExist);
            if (!isExist) continue;

            InventorySlot equipedSlot = TEquipedSlotParent.GetChild(i).GetComponent<InventorySlot>();
            equipedSlot.InitDefault();
            // 슬롯에 해당하는 이미지 삽입
            if (i < runeList.Count)
            {
                equipedSlot.SetRuneData(runeList[i]);
           }
        }

        InitCharacterEquipModel();
    }

    private void ReArrangeMainRuneSlot()
    {
        int count = ItemPref.instance.myRunes.Count > TMainRuneSlotParent.childCount? ItemPref.instance.myRunes.Count:TMainRuneSlotParent.childCount;

        for(int i = 0; i < count; i++)
        {
            GameObject obj = null;
            bool isExist = i < ItemPref.instance.myRunes.Count;
            if (count == ItemPref.instance.myRunes.Count && i >= TMainRuneSlotParent.childCount)
            {
                obj = Instantiate(ObjMainRuneSlot, TMainRuneSlotParent);
            }
            else if(count == TMainRuneSlotParent.childCount && i >= ItemPref.instance.myRunes.Count)
            {
                obj = TMainRuneSlotParent.GetChild(i).gameObject;
            }

            if (obj != null) {
                obj.SetActive(isExist);
                if (isExist)
                {
                    obj.GetComponent<Image>().sprite = ItemPref.instance.myRunes[i].getItemImage();
                }
            }
        }

        MainRuneScrollSnap.GoToPanel(MainRuneScrollSnap.NearestPanel);
        OnChangeScrollSnap();
    }

    private void ChangeMainRune(int selectedIndex)
    {
        if (!ItemManager.SelectRune(EnumList.ITEMMANAGER_TYPE.MAINRUNE, selectedIndex)) Debug.Log("에러 SetRuneUIManager : ChangeMainRune");
    }

    private void InitCharacterEquipModel()
    {
        // 메인룬

        // 서브룬
        for(int i = 0; i < ObjSubRuneParent.transform.childCount; i++)
        {
            bool isExist = i < (int)ItemPref.instance.curMainRune.subRunes.Count;
            ObjSubRuneParent.transform.GetChild(i).gameObject.SetActive(isExist);

            if (!isExist) continue;

        }
    }

    public void EquipSubRune(int index)
    {
        if (!ItemManager.SelectRune(EnumList.ITEMMANAGER_TYPE.SUBRUNE, index)) return;

        // 슬롯 re-paint
        ReArrangeEquipedSlot();
        ReArrangeInventorySlot();
    }

    public void UnEquipSubRune(int index)
    {
        ItemManager.DeSelectSubRune(index);

        // 슬롯 re-paint
        ReArrangeEquipedSlot();
        ReArrangeInventorySlot();
    }

    public InventorySlot IsEqualData(SubRune rune)
    {
        foreach(Transform Tslot in TEquipedSlotParent)
        {
            if (Tslot.GetComponent<InventorySlot>().RuneData == rune) return Tslot.GetComponent<InventorySlot>(); 
        }

        return null;
    }
}
