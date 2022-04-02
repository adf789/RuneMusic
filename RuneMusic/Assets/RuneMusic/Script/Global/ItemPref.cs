using PlayFab;
using RuneSkill;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemPref
{
    public static ItemPref instance { get; private set; } = new ItemPref();

    public List<SubRune> allRunes, myRunes;
    public List<SubRune> allSubRunes, mySubRunes;
    public Rune curMainRune;
    public int Gold, RuneFrag;
    private bool loaded;

    private ItemPref()
    {
        allRunes = new List<SubRune>();
        myRunes = new List<SubRune>(ItemManager.InvenSize);
        allSubRunes = new List<SubRune>();
        mySubRunes = new List<SubRune>(ItemManager.InvenSize);

        loaded = false;
    }

    public IEnumerator loadItemDB(List<Sprite> sprites)
    {
        if (loaded) yield break;

        while (!GameDatas.AlreadyLoadAccount)
        {
            yield return null;
        }
        // 전체 룬 불러오기
        //StringReader sr = new StringReader(runeDB.text);
        //string line = sr.ReadLine();
        //string[] row;
        //try
        //{

        //    while (line != null)
        //    {
        //        row = line.Split('\t');

        //        EnumList.RUNEGRADE grade = (EnumList.RUNEGRADE)Enum.Parse(typeof(EnumList.RUNEGRADE), row[7], true);
        //        if (!Enum.IsDefined(typeof(EnumList.RUNEGRADE), grade)) throw new Exception();
        //        // bool 인자는 string 형식으로 추가될 수 없으므로
        //        // 조건문으로 삽입
        //        if (row[1] == "MainRune")
        //        {

        //            allRunes.Add(new Rune(sprites[0], int.Parse(row[0]), row[2], row[3], row[4] == "TRUE", JsonUtility.FromJson<Serialization<short>>(row[5]).target, grade));
        //        }
        //        else if (row[1] == "SubRune")
        //        {
        //            csSkill skill;
        //            if (SkillManager.SkillData.GetSkill(JsonUtility.FromJson<SkillManager.SkillData>(row[6]), out skill))
        //            {
        //                allSubRunes.Add(new SubRune(sprites[1], int.Parse(row[0]), row[2], row[3], row[4] == "TRUE", skill, grade));
        //            }
        //        }

        //        line = sr.ReadLine();
        //    }
        //}catch(Exception e)
        //{
        //    Debug.LogError(e.Message);
        //}
        var request = new PlayFab.ClientModels.GetCatalogItemsRequest() { CatalogVersion = "Rune" };
        PlayFabClientAPI.GetCatalogItems(request, (result) =>
         {
             for (int i = 0; i < result.Catalog.Count; i++)
             {
                 var catalogItem = result.Catalog[i];
                 var itemForCustomData = JsonUtility.FromJson<RuneData>(catalogItem.CustomData);

                 EnumList.RUNEGRADE grade = (EnumList.RUNEGRADE)Enum.Parse(typeof(EnumList.RUNEGRADE), itemForCustomData.Grade, true);
                 if (!Enum.IsDefined(typeof(EnumList.RUNEGRADE), grade)) throw new Exception();
                 // bool 인자는 string 형식으로 추가될 수 없으므로
                 // 조건문으로 삽입
                 if (catalogItem.ItemClass.Equals("MainRune"))
                 {

                     allRunes.Add(new Rune(sprites[0], int.Parse(itemForCustomData.MusicIndex), catalogItem.DisplayName, catalogItem.Description, false, JsonUtility.FromJson<Serialization<short>>(itemForCustomData.MainAttribute).target, grade));

                 }
                 else if (catalogItem.ItemClass.Equals("SubRune"))
                 {
                     csSkill skill;
                     if (SkillManager.SkillData.GetSkill(JsonUtility.FromJson<SkillManager.SkillData>(itemForCustomData.SubAttribute), out skill))
                     {
                         allSubRunes.Add(new SubRune(sprites[1], int.Parse(itemForCustomData.MusicIndex), catalogItem.DisplayName, catalogItem.Description, false, skill, grade));
                     }
                 }
             }
             Debug.Log("아이템 로드 성공");
             InitRune();
         },
         (error) => { Debug.LogError("상점 불러오기 실패"); });

        loaded = true;
    }

    private void InitRune()
    {
        SubRune basicRune1 = allRunes[0];
        //SubRune basicRune1 = ItemPref.instance.allRunes.Find((x) => x.index == 66);
        SubRune basicSubRune1 = allSubRunes[0];

        // 메인룬, 서브룬 초기 장비
        if (basicRune1 != null && !ItemPref.instance.myRunes.Contains(basicRune1))
        {
            // 초기 메인룬
            basicRune1.isUsing = true;
            ItemPref.instance.curMainRune = (Rune)basicRune1;
            if (!ItemPref.instance.myRunes.Contains(basicRune1))
                ItemPref.instance.myRunes.Add(basicRune1);
        }
        else Debug.Log("MainRune 로드 실패");

        if (basicSubRune1 != null && !ItemPref.instance.mySubRunes.Contains(basicSubRune1))
        {
            basicSubRune1.isUsing = false;
            if (!ItemPref.instance.mySubRunes.Contains(basicSubRune1))
                ItemPref.instance.mySubRunes.Add(basicSubRune1);
        }
        else Debug.Log("SubRune 로드 실패");
    }
}

[Serializable]
class RuneData
{
    public string MainAttribute;
    public string SubAttribute;
    public string Grade;
    public string MusicIndex;

    public RuneData(string _MainAttribute, string _SubAttribute, string _Grade, string _MusicIndex)
    {
        this.MainAttribute = _MainAttribute;
        this.SubAttribute = _SubAttribute;
        this.Grade = _Grade;
        this.MusicIndex = _MusicIndex;
    }
}
