using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;

public partial class SetRuneUIManager : MonoBehaviour
{
    [Header("메인룬 장비")]
    [SerializeField] private GameObject MainUIRoot;
    [SerializeField] private GameObject ObjMainRuneSlot;
    [SerializeField] private Transform TMainRuneSlotParent;
    [SerializeField] private Text MainRuneNameText;
    [SerializeField] private Text MainRuneExplainText;
    [SerializeField] private Text MainRuneHpText;
    [SerializeField] private Text MainRuneAtkText;
    [SerializeField] private Text MainRuneDefText;
    [SerializeField] private Text EquipmentMsgText;
    [SerializeField] private SimpleScrollSnap MainRuneScrollSnap;
    [Header("서브룬 장비")]
    [SerializeField] private GameObject SubUIRoot;
    [SerializeField] private GameObject ObjSlot;
    [SerializeField] private List<GameObject> ObjListTab;
    [SerializeField] private Transform TEquipedSlotParent;
    [SerializeField] private Transform TInventorySlotParent;
    [SerializeField] private RectTransform TScrollContent;
    [SerializeField] private Sprite EmptySlotImage;
    [SerializeField] private Text MainNameText;
    [Header("캐릭터")]
    [SerializeField] private GameObject ObjMainRuneCharacter;
    [SerializeField] private GameObject ObjSubRuneParent;

    private static List<InventorySlot> EquipedSlot, SubListSlot;
    private StringBuilder sb;
    public Rune SelectMainRune;
    private EnumList.SUBRUNE_VIEW_TYPE CurType;
}
