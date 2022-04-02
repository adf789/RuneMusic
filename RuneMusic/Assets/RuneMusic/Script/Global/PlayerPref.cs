using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPref : TargetPref
{
    public static PlayerPref Instance { private set; get; } = new PlayerPref(0, 0, 0, 1);
    public SkillManager skillManager { private set; get; } = new SkillManager();
    public int level;
    public int curExperience { get; private set; }
    public int maxExperience { get; private set; }
    public DateTime IdleTimer, rotationTimer;
    public Vector3 MainHomeCharacterPos = Vector3.zero;
    public Quaternion MainHomeCharacterRot = Quaternion.identity;
    private float critRate, critDmg;

    private PlayerPref(int _hp, int _atk, int _def, int _level) : base(_hp, _atk, _def)
    {
        this.level = _level;
        maxExperience = 100;
        IdleTimer = DateTime.Now;
    }

    // 현재 장착된 메인룬의 능력치를 입력 및 플레이어 능력치 초기화
    public override void Init()
    {
        Rune mainRune = ItemPref.instance.curMainRune;
        if (mainRune == null) return;
        this.originHp = mainRune.getMainAttr(EnumList.MAIN_RUNE_STAT.HP);
        this.originAtk = mainRune.getMainAttr(EnumList.MAIN_RUNE_STAT.ATK);
        this.originDef = mainRune.getMainAttr(EnumList.MAIN_RUNE_STAT.DEF);
        this.curHp = this.originHp;
        this.critRate = 0f;
        this.critDmg = 1.0f;

        skillManager.Clear();
        for(int i = 0; i < mainRune.subRunes.Count; i++)
        {
            skillManager.AddSkill(mainRune.subRunes[i].skill);
        }
    }
    
    public override int GetCurHp()
    {
        return base.GetCurHp();
    }
    public override int GetTotalAtk()
    {
        int baseAtk = base.GetTotalAtk();
        if (UnityEngine.Random.Range(0, 1.0f) < critRate) return (int)(baseAtk * critDmg);
        else return baseAtk;
    }

    public override int GetTotalDef()
    {
        return base.GetTotalDef();
    }

    public void SetCritRate(float rate, bool add)
    {
        if (add) critRate += rate;
        else critRate -= rate;
    }

     public void SetCritDmg(float rate, bool add)
    {
        if (add) critDmg += rate;
        else critDmg -= rate;
    }

    public void AddExp(int exp)
    {
        this.curExperience += exp;
        if(curExperience >= maxExperience)
        {
            level++;
            curExperience = curExperience - maxExperience;
            maxExperience += 100;
        }
    }

    public void LoadScene(EnumList.SCENE_NAME loadSceneName)
    {
        Scene prevScene = SceneManager.GetActiveScene();
        if (prevScene.name.Equals(EnumList.SCENE_NAME.MainHomeScene.ToString())) PlayerPref.Instance.rotationTimer = DateTime.Now;
        PlayerPrefs.SetInt("prevScene", prevScene.buildIndex);
        SceneManager.LoadScene(loadSceneName.ToString(), LoadSceneMode.Single);
    }

    public void LoadPrevScene()
    {
        Scene prevScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(PlayerPrefs.GetInt("prevScene"), LoadSceneMode.Single);
        PlayerPrefs.SetInt("prevScene", prevScene.buildIndex);
    }

    public EnumList.SCENE_NAME GetActiveSceneName()
    {
        EnumList.SCENE_NAME name;
        try
        {
            name = (EnumList.SCENE_NAME)Enum.Parse(typeof(EnumList.SCENE_NAME), SceneManager.GetActiveScene().name, true);
            return name;
        }catch(ArgumentException e)
        {
            Debug.LogError("enum 형식이 맞지 않습니다." + e.Message);

        }
        return EnumList.SCENE_NAME.FirstScene;
    }

    public bool IsActiveSceneName(EnumList.SCENE_NAME name)
    {
        return GetActiveSceneName().Equals(name);
    }
}
