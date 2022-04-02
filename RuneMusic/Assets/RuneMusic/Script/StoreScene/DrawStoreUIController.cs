//using PlayFab;
//using PlayFab.ClientModels;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DrawStoreUIController : MonoBehaviour
{
    public DrawStoreEffect effectController;
    [SerializeField]
    private Text logText, goldText;
    [SerializeField]
    private GameObject BtnDraw, BtnDraw10;
    [SerializeField]
    private Animator drawAnim;
    [SerializeField]
    private DrawNewItemListUI drawItemListUI;
    private bool isLocal;
    private StringBuilder sb;
    private short whatDrawNumber;
    // Start is called before the first frame update
    void Start()
    {
        sb = new StringBuilder();
        isLocal = /*PlayerPrefs.GetInt("isLocal") == 1;*/true;
        getCurrency();
        whatDrawNumber = 1;
    }

    public void onClickDraw()
    {
        logText.text = "";
        if (ItemPref.instance.Gold < 300)
        {
            logText.text = "돈 부족(300 필요)";
            return;
        }

        whatDrawNumber = 1;
        ActiveDrawButton(false);
        drawAnim.SetTrigger("StartDraw");
    }

    public void onClickDraw10()
    {
        logText.text = "";
        if (ItemPref.instance.Gold < 3000)
        {
            logText.text = "돈 부족(3000 필요)";
            return;
        }

        whatDrawNumber = 10;
        ActiveDrawButton(false);
        drawAnim.SetTrigger("StartDraw");
    }

    public void onClickItemBox()
    {
        drawItemListUI.ShowUI(true);
        drawRune(whatDrawNumber);
        drawAnim.SetTrigger("OpenBox");
        effectController.onKeyAndLockActive();
    }

    private void drawRune(short drawCount)
    {
        int count = 0;

        for (int i = 0; i < drawCount; i++)
        {
            if (!isLocal)
            {
                //string _itemId = "";
                //string _itemName = "";
                //// 상점 불러오기, 랜덤 아이템 아이디 생성
                //PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest() { CatalogVersion = "Rune" }, (result) =>
                //{
                //    var item = result.Catalog[UnityEngine.Random.Range(0, result.Catalog.Count - 1)];
                //    _itemId = item.ItemId;
                //    _itemName = item.DisplayName;
                //}, (error) => print(logText.text = "상점 불러오기 실패"));

                //// 아이템 구매
                //var request = new PurchaseItemRequest() { CatalogVersion = "Rune", ItemId = _itemId, VirtualCurrency = "RS", Price = 300 };
                //PlayFabClientAPI.PurchaseItem(request, (result) =>
                //{
                //    logText.text = _itemName + " 구매";
                //}, (error) => logText.text = "돈 부족(300 필요)");
            }
            else
            {
                ItemPref itemData = ItemPref.instance;
                SubRune newRune = null;
                sb.Length = 0;
                // draw 메인룬
                if (UnityEngine.Random.Range(0, 2) < 1)
                {
                    int size = itemData.allRunes.Count;
                    newRune = itemData.allRunes[UnityEngine.Random.Range(1, size - 1)];
                    // 중복 처리
                    if (!itemData.myRunes.Contains(newRune))
                    {
                        count++;
                        if (!ItemManager.AddRune(EnumList.ITEMMANAGER_TYPE.MAINRUNE, newRune)) return;
                    }
                    else newRune = null;
                }
                // draw 서브룬
                else
                {
                    int size = itemData.allSubRunes.Count;
                    newRune = itemData.allSubRunes[UnityEngine.Random.Range(0, size - 1)];
                    // 중복 처리
                    if (!itemData.mySubRunes.Contains(newRune))
                    {
                        count++;
                        if (!ItemManager.AddRune(EnumList.ITEMMANAGER_TYPE.SUBRUNE, newRune)) return;
                    }
                    else newRune = null;
                }
                //sb.Append(newRune.name);
                //logText.text = sb.ToString();
                if(newRune != null) drawItemListUI.AddItem(newRune);
                drawItemListUI.SetNewCount(count);
                ItemPref.instance.Gold -= 300;
            }
        }

        getCurrency();
    }

    private void getCurrency()
    {
        int curRS = ItemPref.instance.Gold;
        //if (!isLocal) PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (result) => curRS = result.VirtualCurrency["RS"], (error) => curRS = 0);
        goldText.text = curRS + "";
    }

    public void ActiveDrawButton(bool isActive)
    {
        BtnDraw.SetActive(isActive);
        BtnDraw10.SetActive(isActive);
    }
}
