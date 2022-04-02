using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class MainHomeUIManager : MonoBehaviour
{
    private void KeyDown()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null) Debug.Log(hit.collider.tag);
                if (hit.collider.tag == "Player")
                {
                    // idle 윈도우 출력
                    PopupAnimator.SetBool("ShowIdle", true);
                    bShowIdleWindow = true;
                }
                else if (hit.collider.tag == "Stage")
                {
                    StageStar stage = hit.collider.GetComponent<StageStar>();
                    stage.OnClick();
                }
            }
            else
            {
                if (PopupAnimator.GetBool("ShowIdle")) onClickSpentReward();
                PopupAnimator.SetBool("ShowMessages", false);
                PopupAnimator.SetBool("ShowSetting", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }

    public void onClickEquip()
    {
        PlayerPref.Instance.LoadScene(EnumList.SCENE_NAME.SetRuneScene);
    }

    public void onClickStore()
    {
        PlayerPref.Instance.LoadScene(EnumList.SCENE_NAME.StoreScene);
    }

    public void onClickAdventure()
    {
        MenuAnimator.SetBool("IsStage", true);
        StartCoroutine(CameraMoveToStage(1));
    }

    // Idle 보상 받기 버튼
    public void onClickSpentReward()
    {
        PlayerPref.Instance.IdleTimer = DateTime.Now;

        int curRS = ItemPref.instance.Gold;
        ItemPref.instance.Gold += getMoney;
        StartCoroutine(CountingWorth(curRS, curRS + getMoney, 0.5f, 1.0f));
        getMoney = 0;
        bShowIdleWindow = false;
        // idle 윈도우 off
        PopupAnimator.SetBool("ShowIdle", false);
    }

    // 메뉴 버튼
    public void onClickMenu()
    {
        if (MenuAnimator.GetBool("IsStage"))
        {
            if(CurStage != null) CurStage.IsSelected(false);
            PopupAnimator.SetBool("ShowStageExplain", false);
            MenuAnimator.SetBool("IsStage", false);
            StartCoroutine(CameraMoveToStage(0));
        }
        else
        {
            bool isMenuOpen = MenuAnimator.GetBool("IsMenuOpen");
            MenuAnimator.SetBool("IsMenuOpen", !isMenuOpen);
        }
    }

    // 채팅 버튼
    public void onClickChat()
    {
        bool isChatOpen = ToolAnimator.GetBool("OnChat");
        ToolAnimator.SetBool("OnChat", !isChatOpen);
    }

    // 메시지 버튼
    public void onClickMessage()
    {
        bool isMessageOpen = PopupAnimator.GetBool("ShowMessages");
        PopupAnimator.SetBool("ShowMessages", !isMessageOpen);
    }

    // 설정 버튼
    public void onClickSetting()
    {
        bool isSettingOpen = PopupAnimator.GetBool("ShowSetting");
        PopupAnimator.SetBool("ShowSetting", !isSettingOpen);
    }

    public void OnSelectStage(StageStar stage)
    {
        // 현재 선택된 스테이지
        if(CurStage != null) CurStage.IsSelected(false);
        CurStage = stage;

        sb.Length = 0;
        sb.Append(stage.name);
        sb.Append('\n');
        sb.Append("Hp : ");
        sb.Append(stage.addHp);
        PopupAnimator.SetBool("ShowStageExplain", true);
        ExplainText.text = sb.ToString();
    }

    public void OnClickStageStart()
    {
        // 몬스터 큐에 추가 후 스테이지 실행
        MonsterPref monster = new MonsterPref(CurStage.addHp, CurStage.addAtk, CurStage.addDef, CurStage.addGold, CurStage.addExp);
        MonsterPref.monsterQueue.Enqueue(monster);
        PlayerPref.Instance.LoadScene(EnumList.SCENE_NAME.PlayGameScene);
    }

    public void OnClickCloseExplain()
    {
        PopupAnimator.SetBool("ShowStageExplain", false);
        CurStage.IsSelected(false);
    }

    public void OnClickNextStage()
    {
        if (CurStage != null) CurStage.IsSelected(false);
        PopupAnimator.SetBool("ShowStageExplain", false);
        SetStageIndex(1, true);
    }

    public void OnClickPrevStage()
    {
        if (CurStage != null) CurStage.IsSelected(false);
        PopupAnimator.SetBool("ShowStageExplain", false);
        SetStageIndex(-1, true);
    }
}
