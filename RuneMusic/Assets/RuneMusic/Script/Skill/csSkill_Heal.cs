using RuneSkill;
using UnityEngine;

public class csSkill_Heal : csSkill
{
    private EnumList.NOTE_JUDGE judge = EnumList.NOTE_JUDGE.PERFECT;
    private bool includeUpper = true;

    public csSkill_Heal(SkillManager.SkillData data) : base(0, data)
    {
        text = "판정 시 PERFECT 이상일 경우 기본 데미지의 " + Mathf.FloorToInt(this.data.obj1 * 100) + "% 만큼 체력 회복";
    }



    public override void DoSkill()
    {
        skillManager.battleManager.StartSkill(EnumList.SKILLS.HEAL, skillManager.caster, skillManager.atkDmgBuffer * this.data.obj1, 0);
    }

    public override bool CheckCondition()
    {
        if (includeUpper && (int)skillManager.judgeBuffer >= (int)judge) return true;
        else if (!includeUpper && (int)skillManager.judgeBuffer < (int)judge) return true;

        return false;
    }

    protected override void SetCondition()
    {
        this.condition = EnumList.SKILL_SITUATION.WHEN_ATTACK;
    }
}
