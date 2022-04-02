using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class SetRuneUIManager : MonoBehaviour
{
    public void OnBackClick()
    {
        if (!MainUIRoot.activeSelf)
        {
            PlayerPref.Instance.LoadPrevScene();
            return;
        }
        OnChangeEquipScreen();

    }

    public void OnTabClick(EnumList.SUBRUNE_VIEW_TYPE type)
    {
        CurType = type;

        // 탭 버튼 이미지 변경
        int tabNum = 0, i = 0;
        switch (type)
        {
            case EnumList.SUBRUNE_VIEW_TYPE.ALL:
                tabNum = 0;
                break;
        }
        foreach (GameObject obj in ObjListTab)
        {
            if (i == tabNum)
            {
                ObjListTab[i].GetComponentInChildren<Text>().color = Color.white;
            }
            else
            {
                ObjListTab[i].GetComponentInChildren<Text>().color = Color.grey;
            }
            i++;
        }

        // 현재 타입에 의해 선택된 룬들, 슬롯 정렬
        // EnumList.SUBRUNE_VIEW_TYPE 타입 추가하여 분기 기재 필요
        i = 0;
        foreach (InventorySlot slot in SubListSlot)
        {
            slot.InitDefault();

            if (type == EnumList.SUBRUNE_VIEW_TYPE.ALL)
            {
                List<SubRune> runes = ItemPref.instance.mySubRunes;
                bool isExist = i < runes.Count;
                slot.gameObject.SetActive(isExist);

                if (isExist)
                {
                    slot.SetRuneData(runes[i]);
                }
            }
            i++;
        }
    }

    public void OnChangeEquipScreen()
    {
        // 서브룬 장착 화면
        if(MainUIRoot.activeSelf)
        {
            MainUIRoot.SetActive(false);
            SubUIRoot.SetActive(true);
            OnChangeSubRunePanelScroll();
        }
        // 메인룬 장착 화면
        else
        {
            MainUIRoot.SetActive(true);
            SubUIRoot.SetActive(false);
            Vector3 position = ObjMainRuneCharacter.transform.position;
            position.y = 0f;
            ObjMainRuneCharacter.transform.position = position;
            MainRuneScrollSnap.NearestPanel = ItemPref.instance.myRunes.IndexOf(ItemPref.instance.curMainRune);
            MainRuneScrollSnap.GoToPanel(MainRuneScrollSnap.NearestPanel);
            //OnChangeScrollSnap();
            ResetMainRuneInfoText();
        }
    }

    public void ResetMainRuneInfoText()
    {
        Rune rune = ItemPref.instance.curMainRune;
        MainRuneNameText.text = rune.name;
        MainRuneExplainText.text = rune.explain;
        MainRuneHpText.text = rune.getMainAttr(EnumList.MAIN_RUNE_STAT.HP).ToString();
        MainRuneAtkText.text = rune.getMainAttr(EnumList.MAIN_RUNE_STAT.ATK).ToString();
        MainRuneDefText.text = rune.getMainAttr(EnumList.MAIN_RUNE_STAT.DEF).ToString();
    }

    public void OnChangeScrollSnap()
    {
        SelectMainRune = ItemPref.instance.myRunes[MainRuneScrollSnap.NearestPanel] as Rune;
        MainRuneNameText.text = SelectMainRune.name;
        MainRuneExplainText.text = SelectMainRune.explain;
        MainRuneHpText.text = SelectMainRune.getMainAttr(EnumList.MAIN_RUNE_STAT.HP).ToString();
        MainRuneAtkText.text = SelectMainRune.getMainAttr(EnumList.MAIN_RUNE_STAT.ATK).ToString();
        MainRuneDefText.text = SelectMainRune.getMainAttr(EnumList.MAIN_RUNE_STAT.DEF).ToString();
    }

    public void OnChangeSubRunePanelScroll()
    {
        // 스크롤 변화에 따른 캐릭터 위치 변화
        Vector3 pos = ObjMainRuneCharacter.transform.position;
        pos.y = 3.2f + TScrollContent.anchoredPosition.y * 0.0057f;
        if (pos.y < 3.2f) pos.y = 3.2f;
        ObjMainRuneCharacter.transform.position = pos;
    }

    public void OnClickSetMainRune()
    {
        if (ItemPref.instance.curMainRune.musicIndex == SelectMainRune.musicIndex) return;

        ChangeMainRune(ItemPref.instance.myRunes.FindIndex((x) => x.musicIndex == SelectMainRune.musicIndex));
        MainNameText.text = SelectMainRune.name;
        StartCoroutine(ShowEquipMsg());
    }

    IEnumerator ShowEquipMsg()
    {
        EquipmentMsgText.GetComponent<CanvasGroup>().alpha = 1;
        while (EquipmentMsgText.GetComponent<CanvasGroup>().alpha > 0)
        {
            EquipmentMsgText.GetComponent<CanvasGroup>().alpha -= 0.01f;
            yield return null;
        }
    }
}
