using RuneSkill;
using UnityEngine;

public class csSkill_BufAllStat : csSkill
{
    public csSkill_BufAllStat(SkillManager.SkillData data) : base(3.0f, data)
    {
        text = "공격 시 " + Mathf.FloorToInt(this.data.obj1 * 100) + "% 확률로 " + this.duration + "초간 자신의 모든 스텟이 " + Mathf.FloorToInt(this.data.obj2 * 100) + "% 상승";
    }

    public override void DoSkill()
    {
        skillManager.battleManager.StartSkill(EnumList.SKILLS.STATUP, skillManager.caster, this.data.obj2, this.duration);
    }

    public override bool CheckCondition()
    {
        if (UnityEngine.Random.Range(0f, 1f) < this.data.obj1) return true;
        else return false;
    }

    protected override void SetCondition()
    {
        this.condition = EnumList.SKILL_SITUATION.WHEN_ATTACK;
    }
}
