using RuneSkill;
using UnityEngine;

public class csSkill_Thorns : csSkill
{
    public csSkill_Thorns(SkillManager.SkillData data) : base(0, data)
    {
        text = "상대방의 피해 " + Mathf.FloorToInt(this.data.obj1 * 100) + "%를 반사";
    }

    public override void DoSkill()
    {
        skillManager.battleManager.StartSkill(EnumList.SKILLS.THORNS, skillManager.target, -skillManager.getDmgBuffer * this.data.obj1, 0);
    }

    public override bool CheckCondition()
    {
        return true;
    }

    protected override void SetCondition()
    {
        this.condition = EnumList.SKILL_SITUATION.WHEN_DAMAGED;
    }
}
