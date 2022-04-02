using RuneSkill;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Rune : SubRune
{
    public List<short> stats { get; private set; }
    public List<SubRune> subRunes { get; private set; }

    public Rune(Sprite _image, int _mindex, string _name, string _explain, bool _isUsing, List<short> _stats, EnumList.RUNEGRADE _grade)
        : base(_image, _mindex, _name, _explain, _isUsing, null, _grade)
    {
        subRunes = new List<SubRune>(3);
        stats = _stats;
    }
    
    public bool unionSubRune(SubRune subRune)
    {
        if (subRunes.Count >= 3) return false;
        else if (subRune.isUsing) return false;
        else if (subRunes.Contains(subRune)) return false;

        subRune.isUsing = true;
        subRunes.Add(subRune);

        return true;
    }

    public bool minusSubRune(SubRune subRune)
    {
        if (!subRunes.Contains(subRune))
        {
            return false;
        }
        else if (!subRune.isUsing)
        {
            return false;
        }

        subRune.isUsing = false;
        subRunes.Remove(subRune);

        return true;
    }

    public void unRune()
    {
        foreach (SubRune rune in subRunes)
        {
            rune.isUsing = false;
        }

        subRunes.Clear();
    }
    
    // 메인 능력치
    public int getMainAttr(EnumList.MAIN_RUNE_STAT index)
    {
        return index == EnumList.MAIN_RUNE_STAT.SIZE ?  0 : stats[(int)index];
    }
}

[System.Serializable]
public class SubRune
{
    protected Sprite image;
    public int musicIndex;
    public string name, explain;
    public bool isUsing = false;
    public csSkill skill;
    public EnumList.RUNEGRADE grade;

    public SubRune(Sprite _image, int _mindex, string _name, string _explain, bool _isUsing, csSkill _skill, EnumList.RUNEGRADE _grade)
    {
        this.image = _image;
        this.name = _name;
        this.explain = _explain;
        this.isUsing = _isUsing;
        this.musicIndex = _mindex;
        this.skill = _skill;
        this.grade = _grade;
    }

    public Sprite getItemImage()
    {
        return this.image;
    }
}
