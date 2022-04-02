using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetPatern : MonoBehaviour
{
    [Tooltip("아무것도 설정하지 않으면 Component 내의 Animator를 가져옴")]
    public Animator anim;
    public Transform hitT, dmgTextT;
    public SpriteRenderer monsterRenderer;
    public Sprite idleSprite, hitSprite, attackSprite;
    protected TargetPref targetData;

    // Start is called before the first frame update
    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        SetTargetData();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void turnSprite(int state)
    {
        switch (state)
        {
            case (int)EnumList.TARGET_STATE.idle:
                if(idleSprite != null) monsterRenderer.sprite = idleSprite;
                break;

            case (int)EnumList.TARGET_STATE.hit:
                if (hitSprite != null) monsterRenderer.sprite = hitSprite;
                break;

            case (int)EnumList.TARGET_STATE.attack:
                if (attackSprite != null) monsterRenderer.sprite = attackSprite;
                break;
        }
    }

    public void hitAnim()
    {
        anim.SetTrigger("Hit");
    }

    public void attackAnim()
    {
        anim.SetTrigger("Attack");
    }

    public virtual TargetPref GetTargetData()
    {
        return targetData;
    }

    public virtual void SetTargetData(TargetPref _targetData)
    {
        this.targetData = _targetData;
    }

    protected abstract void SetTargetData();
    public abstract void activeSkill();
    public abstract void passiveSkill();
    public abstract void specialSkill();
}
