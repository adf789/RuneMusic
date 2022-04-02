using RuneSkill;
using UnityEngine;

public class csSkill_BufAtk : csSkill
{
    public csSkill_BufAtk(SkillManager.SkillData data) : base(0, data)
    {
        text = "자신의 공격력이 " + Mathf.FloorToInt(this.data.obj1 * 100) + "% 상승";
    }

    public override void DoSkill()
    {
        skillManager.caster.GetTargetData().setAtkRate(this.data.obj1, true);
    }

    public override void UndoSkill()
    {
        skillManager.caster.GetTargetData().setAtkRate(this.data.obj1, false);
    }

    public override bool CheckCondition()
    {
        return true;
    }
}
