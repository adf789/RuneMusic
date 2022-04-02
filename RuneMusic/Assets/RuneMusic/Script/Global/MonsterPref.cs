using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MonsterPref : TargetPref
{
    public int experience { get; private set; }
    public int gold { get; private set; }
    private int bufferAtk, bufferDef;
    public static Queue<MonsterPref> monsterQueue = new Queue<MonsterPref>();

    public MonsterPref(int _hp, int _atk, int _def, int _gold, int _exp) : base(_hp, _atk, _def)
    {
        this.gold = _gold;
        this.experience = _exp;
    }

    public override void Init()
    {
        curHp = originHp;
        addHp = 0;
        bufferAtk = -1;
        bufferDef = -1;
    }

    public void setJudgeDmg(EnumList.NOTE_JUDGE judge)
    {
        float judgeCoef = 0f;

        switch (judge)
        {
            case EnumList.NOTE_JUDGE.MISS:
                judgeCoef = 1.0f;
                break;
            case EnumList.NOTE_JUDGE.BAD:
                judgeCoef = 0.7f;
                break;
            case EnumList.NOTE_JUDGE.GOOD:
                judgeCoef = 0.4f;
                break;
            case EnumList.NOTE_JUDGE.GREAT:
                judgeCoef = 0.15f;
                break;
            case EnumList.NOTE_JUDGE.PERFECT:
                judgeCoef = 0f;
                break;
        }

        bufferAtk = (int)((originAtk + addAtk) * judgeCoef);
    }

    public override int GetTotalAtk()
    {
        if (bufferAtk != -1) return bufferAtk;
        else return base.GetTotalAtk();
    }

    public override int GetTotalDef()
    {
        if (bufferDef != -1) return bufferDef;
        else return base.GetTotalDef();
    }

    public static void InitMonsters()
    {
        for(int i = 0; i < monsterQueue.Count; i++)
        {
            MonsterPref monster = monsterQueue.Dequeue();
            monster.Init();
            monsterQueue.Enqueue(monster);
        }
    }
}