using RuneSkill;
using UnityEngine;

public class csSkill_IgnoreDamaged : csSkill
{
    public csSkill_IgnoreDamaged(SkillManager.SkillData data) : base(2.0f, data)
    {
        text = "피해를 입을 시 " + Mathf.FloorToInt(this.data.obj1 * 100) + "% 확률로 " + this.duration + "초간 무적";
    }

    public override void DoSkill()
    {
        skillManager.battleManager.StartSkill(EnumList.SKILLS.IGNORE_DAMAGED, skillManager.caster, this.duration, 0);
    }

    public override bool CheckCondition()
    {
        if (UnityEngine.Random.Range(0f, 1f) < this.data.obj1) return true;
        else return false;
    }

    protected override void SetCondition()
    {
        this.condition = EnumList.SKILL_SITUATION.WHEN_DAMAGED;
    }
}
