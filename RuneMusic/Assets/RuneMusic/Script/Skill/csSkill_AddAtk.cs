using RuneSkill;
using UnityEngine;

public class csSkill_AddAtk : csSkill
{
    private EnumList.NOTE_JUDGE judge = EnumList.NOTE_JUDGE.GREAT;

    public csSkill_AddAtk(SkillManager.SkillData data) : base(0, data)
    {
        text = "판정 시 GREAT 이상일 경우 " + Mathf.FloorToInt(data.obj1 * 100) + "% 확률로 추가 공격을 함";
    }

    public override void DoSkill()
    {
        skillManager.battleManager.StartSkill(EnumList.SKILLS.DOUBLE_ATTACK, skillManager.target, 0, 0);
    }

    public override bool CheckCondition()
    {
        if ((int)skillManager.judgeBuffer < (int)judge) return false;

        if (UnityEngine.Random.Range(0f, 1f) < data.obj1) return true;
        else return false;
    }

    protected override void SetCondition()
    {
        this.condition = EnumList.SKILL_SITUATION.JUDGE;
    }
}
