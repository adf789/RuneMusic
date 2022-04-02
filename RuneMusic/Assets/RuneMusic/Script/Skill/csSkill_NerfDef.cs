using RuneSkill;
using UnityEngine;

public class csSkill_NerfDef : csSkill
{
    private float nerfDefRate = 0.5f;
    public csSkill_NerfDef(SkillManager.SkillData data) : base(10.0f, data)
    {
        text = "공격 시 " + Mathf.FloorToInt(this.data.obj1 * 100) + "%의 확률로 " + this.duration + "초간 상대방 방어력의 " + Mathf.FloorToInt(nerfDefRate * 100) + "% 감소";
    }

    public override void DoSkill()
    {
        skillManager.battleManager.StartSkill(EnumList.SKILLS.NERF_DEF, skillManager.target, nerfDefRate, this.duration);
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
