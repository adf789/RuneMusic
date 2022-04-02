using UnityEngine;

public class PlayerPatern : TargetPatern
{
    public GameObject[] subObjects;
    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        
        for(int i = 0; i < subObjects.Length - ItemPref.instance.curMainRune.subRunes.Count; i++)
        {
            subObjects[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        spinSubObj();
    }

    private void spinSubObj()
    {
        for(int i = 0; i < subObjects.Length; i++)
        {
            subObjects[i].transform.RotateAround(pos, Vector3.forward, 1.7f);
        }
    }

    protected override void SetTargetData()
    {
        this.targetData = PlayerPref.Instance;
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
