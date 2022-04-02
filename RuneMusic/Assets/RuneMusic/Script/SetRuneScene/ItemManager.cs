using JetBrains.Annotations;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager
{
    private static ItemManager instance = null;
    private static readonly object Lock = new object();
    public static ItemManager Instance
    {
        get
        {
            lock (Lock)
            {
                if (instance == null) instance = new ItemManager();
                return instance;
            }
        }
    }
    public static readonly int InvenSize = 30;
    private StringReader sr;
    private string filePath;
    private bool isLocal;

    // Start is called before the first frame update
    private ItemManager()
    {
        isLocal = PlayerPrefs.GetInt("isLocal") == 1;

        filePath = Application.persistentDataPath + "/MyRuneText.txt";

        Load();
    }

    public static bool AddRune(EnumList.ITEMMANAGER_TYPE type, SubRune rune)
    {
        switch (type)
        {
            case EnumList.ITEMMANAGER_TYPE.MAINRUNE:
                if (ItemPref.instance.myRunes.Count < InvenSize) ItemPref.instance.myRunes.Add(rune);
                else return false;
                break;
            case EnumList.ITEMMANAGER_TYPE.SUBRUNE:
                if (ItemPref.instance.mySubRunes.Count < InvenSize) ItemPref.instance.mySubRunes.Add(rune);
                else return false; 
                break;
        }

        return true;
    }

    public static bool DeleteRune(EnumList.ITEMMANAGER_TYPE type, SubRune rune)
    {
        switch (type)
        {
            case EnumList.ITEMMANAGER_TYPE.MAINRUNE:
                if (!ItemPref.instance.myRunes.Remove(rune)) return false;
                break;
            case EnumList.ITEMMANAGER_TYPE.SUBRUNE:
                if (!ItemPref.instance.mySubRunes.Remove(rune)) return false;
                break;
        }

        return true;
    }

    public static bool SelectItem(EnumList.ITEMMANAGER_TYPE type, int index)
    {
        // 슬롯을 클릭했을 때
        if(EnumList.ITEMMANAGER_TYPE.MAINRUNE == type || EnumList.ITEMMANAGER_TYPE.SUBRUNE == type)
        {
            return SelectRune(type, index);
        }

        return true;
    }

    public static bool SelectRune(EnumList.ITEMMANAGER_TYPE type, int index)
    {
        // 선택된 룬
        if (ItemPref.instance.curMainRune.subRunes.Count == (int)ItemPref.instance.curMainRune.grade) return false;
        if (GetMyRuneList(type) == null && GetMyRuneList(type).Count <= index) return false;

        SubRune curRune = GetMyRuneList(type)[index];
        SubRune usingRune;

        if (type == EnumList.ITEMMANAGER_TYPE.MAINRUNE)
        {
            // 사용하고 있었던 룬
            usingRune = GetMyRuneList(type).Find(x => x.isUsing == true);
            if (usingRune != null)
            {
                usingRune.isUsing = false;
                if (usingRune.GetType() == typeof(Rune)) ((Rune)usingRune).unRune();
            }

            curRune.isUsing = true;

            ItemPref.instance.curMainRune = (Rune)curRune;
        }
        else if (type == EnumList.ITEMMANAGER_TYPE.SUBRUNE)
        {
            // 사용하고 있었던 룬
            usingRune = ItemPref.instance.curMainRune;
            // 메인룬에 결합 시도
            bool check = ((Rune)usingRune).unionSubRune(curRune);
            if (!check) return false;
        }

        Save();

        return true;
    }

    public static void DeSelectSubRune(int index)
    {
        // 서브룬만 호출됨
        // 슬롯을 클릭했을 때
        // 선택된 룬
        List<SubRune> subList = ItemPref.instance.curMainRune.subRunes;
        if (index >= subList.Count) return;
        SubRune curRune = subList[index];

        ItemPref.instance.curMainRune.minusSubRune(curRune);

        Save();
    }

    public static void Save()
    {
        //// 암호화
        //string jdata = JsonUtility.ToJson(new Serialization<Rune>(myRunes));
        //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jdata);
        //string code = System.Convert.ToBase64String(bytes);

        //File.WriteAllText(filePath, code);
    }

    public static void Load()
    {
        //// 초기 파일
        //// 로컬 로드
        //if (!File.Exists(filePath))
        //{
        //    Rune basicRune = allRunes[0];
        //    basicRune.isUsing = true;
        //    Rune.curMainRune = basicRune;
        //    myRunes = new List<Rune>() { basicRune };
        //    //for (int i = 1; i < allRunes.Count; i++)
        //    //{
        //    //    myRunes.Add(allRunes[i]);
        //    //}
        //    save();
        //}

        //string code = File.ReadAllText(filePath);
        //byte[] bytes = System.Convert.FromBase64String(code);
        //string jdata = System.Text.Encoding.UTF8.GetString(bytes);

        //myRunes = JsonUtility.FromJson<Serialization<Rune>>(jdata).target;

        //SubRune basicRune = ItemPref.getInstance().allRunes[0];
        //if (!ItemPref.getInstance().myRunes.Contains(basicRune))
        //{
        //    // 초기 메인룬
        //    basicRune.isUsing = true;
        //    ItemPref.getInstance().curMainRune = (Rune)basicRune;
        //    ItemPref.getInstance().myRunes.Add(basicRune);
        //}

        //if (!isLocal)
        //{
        //    //PlayFabClientAPI.GetUserInventory(new PlayFab.ClientModels.GetUserInventoryRequest(), (result) =>
        //    //{
        //    //    for (int i = 0; i < result.Inventory.Count; i++)
        //    //    {
        //    //        var item = result.Inventory[i];
        //    //        myRunes.Add(allRunes[int.Parse(item.ItemId)]);
        //    //    }
        //    //}, (error) => print("인벤토리 로드 실패"));
        //}
        //else
        //{
        //    // 로컬 저장(ram)
        //}
    }

    public static  List<SubRune> GetMyRuneList(EnumList.ITEMMANAGER_TYPE type)
    {
        List<SubRune> list = null;
        switch (type)
        {
            case EnumList.ITEMMANAGER_TYPE.MAINRUNE:
                list = ItemPref.instance.myRunes;
                break;
            case EnumList.ITEMMANAGER_TYPE.SUBRUNE:
                list = ItemPref.instance.mySubRunes;
                break;
        }

        return (list == null) ? null : list;
    }
}

[System.Serializable]
public class Serialization<T>
{
    public List<T> target;
    public Serialization(List<T> _target) => target = _target;
}
