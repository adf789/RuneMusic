using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class MainHomeUIManager : MonoBehaviour
{
    [Header("노트"), SerializeField]
    private Transform noteModel;

    [Header("캐릭터 위치"), SerializeField]
    private Transform CharacterT;

    [Header("스테이지"), SerializeField]
    private Transform[] StageTs;
    [SerializeField]
    private float CameraToStageTime;
    [SerializeField]
    private Text curStageText;
    [SerializeField]
    private Text DebugText;

    [Header("팝업창"), SerializeField]
    private Text IdleWindowTimeText;
    [SerializeField]
    private Text IdleWindowMoneyText;
    [SerializeField]
    private Image[] IdleWindowRuneImage;
    [SerializeField]
    private Text ExplainText;
    [SerializeField]
    private Animator PopupAnimator;
    [SerializeField]
    private ScrollRect EquipScroll;

    [Header("메뉴"), SerializeField]
    private Animator MenuAnimator;
    [SerializeField]
    private GameObject menuPane;

    [Header("툴바"), SerializeField]
    private Text MoneyText;
    [SerializeField]
    private Text RuneFragText;
    [SerializeField]
    private Text LevelText;
    [SerializeField]
    private Animator ToolAnimator;
    [SerializeField]
    private Image ExperienceBar;
    [SerializeField]
    private Text ExperiencePercentText;

    private ObjectPulling<Transform> noteModels;
    private Vector3 pos;
    private StringBuilder sb;
    private MainSpaceRenderer spaceRenderer;
    private StageStar CurStage;
    // 현재 시간 저장
    private float CurTime;
    private int getMoney, curCameraOnStageIndex;
    private bool bShowIdleWindow, bFixCamera;
}
