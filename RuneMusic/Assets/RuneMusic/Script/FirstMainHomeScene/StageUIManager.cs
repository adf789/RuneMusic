using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject ExplainObj;
    [SerializeField]
    private Text ExplaineText;
    [SerializeField]
    private MainHomeUIManager mainUIManager;
    private StageStar curStage;
    private StringBuilder sb;

    // Update is called once per frame
    void Start()
    {
        sb = new StringBuilder();
    }

    public void OnSelectStage(StageStar stage)
    {
        // 현재 선택된 스테이지
        curStage = stage;

        sb.Length = 0;
        sb.Append(stage.name);
        sb.Append('\n');
        sb.Append("Hp : ");
        sb.Append(stage.addHp);
        ExplainObj.SetActive(true);
        ExplaineText.text = sb.ToString();
    }

    public void OnClickStart()
    {
        // 몬스터 큐에 추가 후 스테이지 실행
        MonsterPref monster = new MonsterPref(curStage.addHp, curStage.addAtk, curStage.addDef, curStage.addGold, curStage.addExp);
        MonsterPref.monsterQueue.Enqueue(monster);
        PlayerPref.Instance.LoadScene(EnumList.SCENE_NAME.PlayGameScene);
    }

    public void OnClickCloseExplain()
    {
        ExplainObj.gameObject.SetActive(false);
        curStage.IsSelected(false);
    }

    public void OnClickNextStage()
    {
        ExplainObj.SetActive(false);
        mainUIManager.SetStageIndex(1, true);
    }

    public void OnClickPrevStage()
    {
        ExplainObj.SetActive(false);
        mainUIManager.SetStageIndex(-1, true);
    }
}
