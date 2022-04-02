namespace RuneSkill
{
    [System.Serializable]
    public abstract class csSkill
    {
        protected static SkillManager skillManager;
        public EnumList.SKILL_SITUATION condition { protected set; get; }
        public float duration { protected set; get; }
        public string text { protected set; get; }
        public SkillManager.SkillData data { protected set; get; }

        public csSkill(float _duration, SkillManager.SkillData _data)
        {
            if (skillManager == null) skillManager = PlayerPref.Instance.skillManager;

            this.data = _data;
            this.duration = _duration;
            SetCondition();
        }

        public abstract void DoSkill();
        public virtual void UndoSkill()
        {

        }
        public abstract bool CheckCondition();
        protected virtual void SetCondition()
        {
            condition = EnumList.SKILL_SITUATION.ALWAYS;
        }
    }
}