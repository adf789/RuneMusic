namespace EnumList
{
    public enum RUNEGRADE { RARE = 3, UNIQUE = 5, LEGEND = 7}
    public enum IMAGENAMES { icon_EmptyItem }
    public enum ITEMMANAGER_TYPE {MAINRUNE, SUBRUNE};
    public enum MAIN_RUNE_STAT { HP, ATK, DEF, SIZE };
    public enum MAIN_UI_SKILL_EFFECT_INDEX { IGNORE_DAMAGED };
    public enum NOTE_JUDGE { MISS, BAD, GOOD, GREAT, PERFECT };
    public enum SCENE_NAME { FirstScene, MainHomeScene, SetRuneScene, PlayGameScene, StoreScene};
    public enum SKILL_NAME
    {
        csSkill_AddAtk, csSkill_BufAllStat, csSkill_BufAtk, csSkill_BufAtkOnCondition, csSkill_BufCrit,
        csSkill_BufCritDmg, csSkill_BufDef, csSkill_BufHp, csSkill_ContinueCombo, csSkill_DotDmg, csSkill_Heal,
        csSkill_IgnoreDamaged, csSkill_NerfAtkCount, csSkill_NerfDef, csSkill_Thorns
    }
    public enum SKILL_SITUATION { ALWAYS, JUDGE, OTHER_HPRATE, SELF_HPRATE, WHEN_DAMAGED, WHEN_ATTACK }
    public enum SKILLS { DOUBLE_ATTACK, HEAL, THORNS, DOT_DAMAGE, SETCOMBO, NERF_DEF, IGNORE_DAMAGED, STATUP }
    public enum SLOT_TYPE { INVENTORY, EQUIP}
    public enum SUB_RUNE_STAT { ADDATK, NERFDEF, CRIT, DOTDMG, ATKTYPE, SIZE };
    public enum SUBRUNE_VIEW_TYPE { ALL };
    public enum TARGET_STATE { idle, hit, attack };
}
