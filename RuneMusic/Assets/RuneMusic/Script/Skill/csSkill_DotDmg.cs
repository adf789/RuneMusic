using RuneSkill;
using UnityEngine;

public class csSkill_DotDmg : csSkill
{
    public csSkill_DotDmg(SkillManager.SkillData data) : base(5.0f, data)
    {
        text = "공격 시 " + Mathf.FloorToInt(this.data.obj1 * 100) + "% 확률로 " + this.duration + "초간 기본 데미지의 " + Mathf.FloorToInt(this.data.obj2 * 100) + "% 만큼 지속 피해를 입힘";
    }

    public override void DoSkill()
    {
        PlayerPref targetData = (PlayerPref)skillManager.caster.GetTargetData();
        // 크리티컬 잠시 무효화
        targetData.SetCritRate(1.0f, false);
        skillManager.battleManager.StartSkill(EnumList.SKILLS.DOT_DAMAGE, skillManager.target, -skillManager.caster.GetTargetData().GetTotalAtk() * this.data.obj2, this.duration);
        targetData.SetCritRate(1.0f, true);
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
