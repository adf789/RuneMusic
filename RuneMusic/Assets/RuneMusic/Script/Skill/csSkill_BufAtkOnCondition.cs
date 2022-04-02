using RuneSkill;

public class csSkill_BufAtkOnCondition : csSkill
{
    private float[] hitRates = {0.8f, 0.6f, 0.4f, 0.0f};
    private int prevCount;
    private bool isFirst;

    public csSkill_BufAtkOnCondition(SkillManager.SkillData data) : base(0, data)
    {
        prevCount = 0;
        isFirst = true;
        // this.data.obj1 : 기본 증가 %
        // this.data.obj2 : 증가 %
        text = "자신의 체력이 낮아질수록 공격력이 상승함";
    }

    public override void DoSkill()
    {
        // hp 따른 플레이어의 공격력 변화
        float targetHitRate = (float)skillManager.target.GetTargetData().GetCurHp() / skillManager.target.GetTargetData().GetOriginHp();
        if (!isFirst) skillManager.caster.GetTargetData().setAtkRate(this.data.obj1 + (this.data.obj2 * prevCount), false);

        for(int i = 0; i < hitRates.Length; i++)
        {
            if (targetHitRate >= hitRates[i])
            {
                skillManager.caster.GetTargetData().setAtkRate(this.data.obj1 + (this.data.obj2 * i), true);
                prevCount = i;
                isFirst = false;
                break;
            }
        }
    }
    
    public override bool CheckCondition()
    {
        return true;
    }

    protected override void SetCondition()
    {
        this.condition = EnumList.SKILL_SITUATION.WHEN_ATTACK;
    }
}
