using UnityEngine;

public class MonsterPatern : TargetPatern
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void SetTargetData()
    {
        this.targetData = MonsterPref.monsterQueue.Peek();
    }

    public override void activeSkill()
    {
       
    }

    public override void passiveSkill()
    {
        
    }

    public override void specialSkill()
    {
        
    }
}
