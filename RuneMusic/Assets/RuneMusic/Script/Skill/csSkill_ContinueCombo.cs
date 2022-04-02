using RuneSkill;
using UnityEngine;

public class csSkill_ContinueCombo : csSkill
{
    public csSkill_ContinueCombo(SkillManager.SkillData data) : base(0, data)
    {
        text = "판정 시 MISS일 경우 " + Mathf.FloorToInt(this.data.obj1 * 100) + "% 확률로 콤보가 끊기는 것을 막음";
    }

    public override void DoSkill()
    {
        skillManager.battleManager.setBlockCancelComboRate(this.data.obj1);
    }

    public override void UndoSkill()
    {
        skillManager.battleManager.setBlockCancelComboRate(0f);
    }

    public override bool CheckCondition()
    {
        return true;
    }
}
