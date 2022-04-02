using RuneSkill;
using UnityEngine;

public class csSkill_BufCritDmg : csSkill
{
    public csSkill_BufCritDmg(SkillManager.SkillData data) : base(0, data)
    {
        text = "자신의 치명타 데미지를 " + Mathf.FloorToInt(this.data.obj1 * 100) + "% 상승시키고 공격력을" + Mathf.FloorToInt(this.data.obj2 * 100) + "% 만큼 감소시킨다.";
    }



    public override void DoSkill()
    {
        ((PlayerPref)skillManager.caster.GetTargetData()).SetCritDmg(this.data.obj1, true);
        skillManager.caster.GetTargetData().setAtkRate(this.data.obj2, false);
    }

    public override void UndoSkill()
    {
        ((PlayerPref)skillManager.caster.GetTargetData()).SetCritDmg(this.data.obj1, false);
        skillManager.caster.GetTargetData().setAtkRate(this.data.obj2, false);
    }

    public override bool CheckCondition()
    {
        return true;
    }
}
