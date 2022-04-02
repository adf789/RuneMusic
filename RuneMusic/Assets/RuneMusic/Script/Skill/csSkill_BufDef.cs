using RuneSkill;
using UnityEngine;

public class csSkill_BufDef : csSkill
{
    public csSkill_BufDef(SkillManager.SkillData data) : base(0, data)
    {
        text = "자신의 방어력을 " + Mathf.FloorToInt(this.data.obj1 * 100) + "% 상승";
    }



    public override void DoSkill()
    {
        skillManager.caster.GetTargetData().setDefRate(this.data.obj1, true);
    }

    public override void UndoSkill()
    {
        skillManager.caster.GetTargetData().setDefRate(this.data.obj1, false);
    }

    public override bool CheckCondition()
    {
        return true;
    }
}
