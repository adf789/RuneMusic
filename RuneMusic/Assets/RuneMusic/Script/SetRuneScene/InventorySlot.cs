using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class InventorySlot : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private EnumList.SLOT_TYPE type = EnumList.SLOT_TYPE.INVENTORY;
    public EnumList.SLOT_TYPE Type
    {
        get
        {
            return type;
        }
    }
    [SerializeField] private SetRuneUIManager UIManager;

    [Header("슬롯 공통"), SerializeField] private Image RuneImage;
    [SerializeField] private Text RuneNameText;
    [Header("인벤토리 슬롯"), SerializeField] private Image UsingImage;

    [Space(10)]
    private GameObject checker;
    public GameObject Checker
    {
        get
        {
            if (checker == null) checker = transform.Find("Checker").gameObject;
            return checker;
        }
    }
    [HideInInspector] public SubRune RuneData = null;
    [HideInInspector] public int SlotIndex;

    private float pressedTime;

    #region UI 이벤트
    public void OnPointerUp(PointerEventData eventData)
    {
        pressedTime = -1;
        if (type != EnumList.SLOT_TYPE.INVENTORY) return;
        if (RuneData == null) return;
        // 인벤토리일 경우에만
        if(!RuneData.isUsing) UIManager.EquipSubRune(SlotIndex);
        else
        {
            //InventorySlot equipedSlot = UIManager.IsEqualData(RuneData);
            //if (equipedSlot != null) UIManager.UnEquipSubRune(equipedSlot.SlotIndex);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (type != EnumList.SLOT_TYPE.EQUIP) return;

        // 장비 슬롯일 경우에만
        pressedTime = 0;
        StartCoroutine("OnPressDown");
    }
    #endregion

    IEnumerator OnPressDown()
    {
        while(pressedTime >= 0 && pressedTime < 1.5)
        {
            pressedTime += Time.deltaTime;
            yield return null;
        }

        if (pressedTime >= 0)
        {
            UIManager.UnEquipSubRune(SlotIndex);
        }
    }

    private void Start()
    {
        if(type == EnumList.SLOT_TYPE.EQUIP)
        {
            if(RuneData == null) InitDefault();
        }
    }

    public void InitDefault()
    {
        RuneData = null;
        RuneImage.sprite = null;
        RuneImage.enabled = false;
        RuneNameText.text = "";
        if (type == EnumList.SLOT_TYPE.INVENTORY)
        {
            UsingImage.enabled = false;
            GetComponent<Image>().color = Color.gray;
        }
    }

    public void SetRuneData(SubRune rune)
    {
        RuneData = rune;
        RuneImage.sprite = rune.getItemImage();
        RuneImage.enabled = true;
        RuneNameText.text = rune.name;
        if (type == EnumList.SLOT_TYPE.INVENTORY)
        {
            UsingImage.enabled = rune.isUsing;
            if (rune.isUsing)
            {
                GetComponent<Image>().color = Color.black;
            }
        }
    }
}
    
