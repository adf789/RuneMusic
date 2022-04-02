using RuneSkill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager
{
    private List<csSkill> skills = new List<csSkill>(3);
    public List<float> buffTimes { private set; get;} = new List<float> { 0, 0, 0 };
    public TargetPatern caster;
    public TargetPatern target;
    public EnumList.NOTE_JUDGE judgeBuffer;
    public MainStage battleManager;
    public int atkDmgBuffer, getDmgBuffer;
    private bool alreadyPassive;

    // 현재 조건을 만족하는지 판단하고 만약 만족한다면 스킬 스크립트 실행
    public void CheckConditionsAndExcute(EnumList.SKILL_SITUATION condition)
    {
        if (condition == EnumList.SKILL_SITUATION.ALWAYS)
        {
            if (alreadyPassive) return;
            else alreadyPassive = true;
        }

        for(int i = 0; i < skills.Count; i++)
        {
            if(skills[i].condition == condition && skills[i].CheckCondition())
            {
                // 지속시간이 있는경우
                if (skills[i].duration != 0)
                {
                    if (skills[i].duration <= Time.time - buffTimes[i])
                    {
                        skills[i].DoSkill();
                        buffTimes[i] = Time.time;
                    }
                }
                else
                    skills[i].DoSkill();
            }
        }
    }

    // 적용되어있던 패시브스킬 해제
    public void UnsetPassiveSkills()
    {
        if (!alreadyPassive) return;
        alreadyPassive = false;

        foreach (csSkill skill in skills)
        {
            if(skill.condition == EnumList.SKILL_SITUATION.ALWAYS)
            {
                skill.UndoSkill();
            }
        }
    }

    public void AddSkill(csSkill skill)
    {
        skills.Add(skill);
    }

    public void RemoveSkill(csSkill skill)
    {
        skills.Remove(skill);
    }

    public void Clear()
    {
        judgeBuffer = 0;
        atkDmgBuffer = 0;
        getDmgBuffer = 0;
        alreadyPassive = false;
        for (int i = 0; i < buffTimes.Count; i++) buffTimes[i] = 0;
        skills.Clear();
    }

    [System.Serializable]
    public class SkillData
    {
        public string className;
        public float obj1, obj2, obj3;

        public SkillData(string name, float _obj1, float _obj2, float _obj3)
        {
            this.className = name;
            this.obj1 = _obj1;
            this.obj2 = _obj2;
            this.obj3 = _obj3;
        }

        public static bool GetSkill(SkillData data, out csSkill skill)
        {
            EnumList.SKILL_NAME name = (EnumList.SKILL_NAME)Enum.Parse(typeof(EnumList.SKILL_NAME), data.className, true);

            switch (name)
            {
                case EnumList.SKILL_NAME.csSkill_AddAtk:
                    data.obj1 = 0.07f;
                    skill = new csSkill_AddAtk(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_BufAllStat:
                    data.obj1 = 0.02f;
                    data.obj2 = 0.5f;
                    skill = new csSkill_BufAllStat(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_BufAtk:
                    data.obj1 = 0.05f;
                    skill = new csSkill_BufAtk(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_BufAtkOnCondition:
                    data.obj1 = 0.05f;
                    data.obj2 = 0.05f;
                    skill = new csSkill_BufAtkOnCondition(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_BufCrit:
                    data.obj1 = 0.05f;
                    skill = new csSkill_BufCrit(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_BufCritDmg:
                    data.obj1 = 1.0f;
                    data.obj2 = 0.2f;
                    skill = new csSkill_BufCritDmg(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_BufDef:
                    data.obj1 = 0.1f;
                    skill = new csSkill_BufDef(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_BufHp:
                    data.obj1 = 0.1f;
                    skill = new csSkill_BufHp(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_ContinueCombo:
                    data.obj1 = 0.2f;
                    skill = new csSkill_ContinueCombo(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_DotDmg:
                    data.obj1 = 0.1f;
                    data.obj2 = 0.8f;
                    skill = new csSkill_DotDmg(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_Heal:
                    data.obj1 = 0.01f;
                    skill = new csSkill_Heal(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_IgnoreDamaged:
                    data.obj1 = 0.1f;
                    skill = new csSkill_IgnoreDamaged(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_NerfAtkCount:
                    data.obj1 = 0.9f;
                    skill = new csSkill_NerfAtkCount(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_NerfDef:
                    data.obj1 = 0.03f;
                    data.obj2 = 0.5f;
                    skill = new csSkill_NerfDef(data);
                    return true;
                case EnumList.SKILL_NAME.csSkill_Thorns:
                    data.obj1 = 0.07f;
                    skill = new csSkill_Thorns(data);
                    return true;
            }

            skill = null;
            return false;
        }
    }
}
