using RuneSkill;
using UnityEngine;

public class csSkill_NerfAtkCount : csSkill
{
    public csSkill_NerfAtkCount(SkillManager.SkillData data) : base(0, data)
    {
        text = "몬스터의 공격 빈도수를 " + Mathf.FloorToInt(this.data.obj1 * 100) + "% 감소";
    }



    public override void DoSkill()
    {
        skillManager.battleManager.musicControl.adjustBlockNoteFrequency(this.data.obj1, true);
    }

    public override void UndoSkill()
    {
        skillManager.battleManager.musicControl.adjustBlockNoteFrequency(this.data.obj1, false);
    }

    public override bool CheckCondition()
    {
        return true;
    }
}
