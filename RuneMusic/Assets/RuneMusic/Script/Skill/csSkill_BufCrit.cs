using RuneSkill;
using UnityEngine;

public class csSkill_BufCrit : csSkill
{
    public csSkill_BufCrit(SkillManager.SkillData data) : base(0, data)
    {
        text = "자신의 치명타 확률을 " + Mathf.FloorToInt(this.data.obj1 * 100) + "% 상승";
    }



    public override void DoSkill()
    {
        ((PlayerPref)skillManager.caster.GetTargetData()).SetCritRate(this.data.obj1, true);
    }

    public override void UndoSkill()
    {
        ((PlayerPref)skillManager.caster.GetTargetData()).SetCritRate(this.data.obj1, false);
    }

    public override bool CheckCondition()
    {
        return true;
    }
}
