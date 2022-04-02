using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class TargetPref
{
    public int addHp, addAtk, addDef;
    private float addHpRate = 1.0f, addAtkRate = 1.0f, addDefRate = 1.0f;
    protected int originHp, originAtk, originDef, curHp;
    protected bool ignoreDamaged;

    public TargetPref(int _hp, int _atk, int _def)
    {
        this.originHp = _hp;
        this.originAtk = _atk;
        this.originDef = _def;
        curHp = originHp;
        ignoreDamaged = false;
    }

    public virtual int GetCurHp()
    {
        return curHp + addHp;
    }

    public abstract void Init();

    public int ModifyHp(int amount)
    {
        if (ignoreDamaged) return GetCurHp();

        int tempAmount = amount;

        // 체력이 감소하나 보호막이 있는 경우
        if (tempAmount < 0 && addHp != 0)
        {
            if (addHp >= -tempAmount)
            {
                addHp += tempAmount;
                return curHp + addHp;
            }
            else if (addHp < -tempAmount)
            {
                tempAmount += addHp;
                addHp = 0;
            }
        }

        // 보호막에 상쇄된 데미지 추가 계산
        if (curHp + tempAmount <= 0)
        {
            curHp = 0;
        }
        // Maximum Hp
        else if (curHp + tempAmount > originHp) curHp = originHp;
        else
        {
            curHp += tempAmount;
        }

        return curHp + addHp;
    }

    public int GetOriginHp()
    {
        return originHp;
    }

    public virtual int GetTotalAtk()
    {
        if (originAtk + addAtk <= 0) return 0; 
        else return originAtk + addAtk;
    }

    public virtual int GetTotalDef()
    {
        if (originDef + addDef <= 0) return 0; 
        else return originDef + addDef;
    }

    public void setHpRate(float rate, bool add)
    {
        addHp = (int)Mathf.Round(addHp / addHpRate);
        curHp = (int)Mathf.Round(curHp / addHpRate);
        originHp = (int)Mathf.Round(originHp / addHpRate);

        if(add)
            addHpRate += rate;
        else
            addHpRate -= rate;

        addHp = (int)Mathf.Round(addHp * addHpRate);
        curHp = (int)Mathf.Round(curHp * addHpRate);
        originHp = (int)Mathf.Round(originHp * addHpRate);
    }

    public void setAtkRate(float rate, bool add)
    {
        addAtk = (int)Mathf.Round(addAtk / addAtkRate);
        originAtk = (int)Mathf.Round(originAtk / addAtkRate);

        if (add)
            addAtkRate += rate;
        else
            addAtkRate -= rate;

        addAtk = (int)Mathf.Round(addAtk * addAtkRate);
        originAtk = (int)Mathf.Round(originAtk * addAtkRate);
    }

    public void setDefRate(float rate, bool add)
    {
        addDef = (int)Mathf.Round(addDef / addDefRate);
        originDef = (int)Mathf.Round(originDef / addDefRate);

        if (add)
            addDefRate += rate;
        else
            addDefRate -= rate;

        addDef = (int)Mathf.Round(addDef * addDefRate);
        originDef = (int)Mathf.Round(originDef * addDefRate);
    }

    public void setAllStatsRate(float rate, bool add)
    {
        setHpRate(rate, add);
        setAtkRate(rate, add);
        setDefRate(rate, add);
    }

    public void SetIgnoreDamaged(bool ignore)
    {
        ignoreDamaged = ignore;
    }
}
