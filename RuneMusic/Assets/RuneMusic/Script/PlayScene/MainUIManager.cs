using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MPTKDemoCatchMusic;
using UnityEngine.Events;

public class MainUIManager : MonoBehaviour
{
    // 이펙트
    public Transform playerBullet, monsterBullet, floatingText, judgeObj, touchEffect, getDmgEffect;
    // ui
    public GameObject defeatScreenObj, winScreenObj;
    public Slider playerHpBar, monsterHpBar;
    public Text playerCurHpText, playerMaxHpText, monsterCurHpText, monsterMaxHpText, statText;
    public Sprite[] judgeSprites;
    public MainStage battleManager;
    [SerializeField]
    private Animator[] effectAnims;
    private bool touch;
    private ObjectPulling<Transform> touchEffPulling, getDmgEffPulling, bulletPulling, evilBulletPulling, floatTextPulling, judgePulling;

    // Start is called before the first frame update
    void Start()
    {
        touch = true;
        battleManager.Init();

        touchEffPulling = new ObjectPulling<Transform>(touchEffect, 3);
        getDmgEffPulling = new ObjectPulling<Transform>(getDmgEffect, 3);
        bulletPulling = new ObjectPulling<Transform>(playerBullet, 2);
        evilBulletPulling = new ObjectPulling<Transform>(monsterBullet, 2);
        floatTextPulling = new ObjectPulling<Transform>(floatingText, 3);
        judgePulling = new ObjectPulling<Transform>(judgeObj, 3);
    }

    // Update is called once per frame
    void Update()
    {
        KeyDown();
    }

    public void SetStatText(string text)
    {
        statText.text = text;
    }

    private void KeyDown()
    {
        if (Input.GetButtonDown("Fire1") && touch)
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag.Equals("Note"))
                {
                    RuneNoteView noteView = hit.transform.GetComponent<RuneNoteView>();
                    if (noteView != null)
                    {
                        // 노트 파괴 이펙트
                        GameObject obj = touchEffPulling.GetObject(hit.transform.position).gameObject;
                        StartCoroutine(HideObject(obj, 2.0f));

                        battleManager.StartEventTouchNote(noteView);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            battleManager.ExitBattle();
            //SceneManager.LoadScene(1, LoadSceneMode.Single);
            //GameManager.LoadScene(EnumList.SCENE_NAME.MainHomeScene);
            PlayerPref.Instance.LoadScene(EnumList.SCENE_NAME.MainHomeScene);
        }
    }

    public void FloatText(string text, Vector3 pos, Color color, int size = 100, float duration = 1.0f)
    {
        floatingText.GetComponentInChildren<Text>().text = text;
        floatingText.GetComponentInChildren<Text>().fontSize = size;
        floatingText.GetComponentInChildren<Text>().color = color;
        GameObject damageObj = floatTextPulling.GetObject(pos).gameObject;
        StartCoroutine(HideObject(damageObj, duration));
    }

    // 발사 이펙트 출력
    public void FireBullet(Vector3 startPos, Vector3 finishPos, bool isPlayer, float speed = 50f, float acc = 100f, float destroyTime = 0.7f)
    {
        GameObject bulletObj;
        if (!isPlayer)
            bulletObj = bulletPulling.GetObject(startPos).gameObject;
        else
            bulletObj = evilBulletPulling.GetObject(startPos).gameObject;

        BulletBezierMove bullet = bulletObj.GetComponent<BulletBezierMove>();

        bullet.Init();
        bullet.startObj = startPos;
        bullet.targetObj = finishPos;
        bullet.speed = speed;
        bullet.accel = acc;
        bullet.autoHandleHeight = 35;
        bullet.destroyTime = destroyTime;
        bullet.repeat = false;
        bullet.targetUpdate = true;
        bullet.mainEvent = battleManager;
        bullet.targetIsPlayer = isPlayer;

        StartCoroutine(CheckBulletDistance(bullet));
    }

    public void SetPlayerHpBar(float value)
    {
        playerHpBar.value = value;
    }

    public void SetPlayerCurHpText(string text)
    {
        playerCurHpText.text = text;
    }

    public void SetPlayerMaxHpText(string text)
    {
        playerMaxHpText.text = text;
    }

    public void SetMonsterHpBar(float value)
    {
        monsterHpBar.value = value;
    }

    public void SetMonsterCurHpText(string text)
    {
        monsterCurHpText.text = text;
    }

    public void SetMonsterMaxHpText(string text)
    {
        monsterMaxHpText.text = text;
    }

    public void CreateAttackEffect(Vector3 pos, float duration = 0.7f)
    {
        GameObject obj = getDmgEffPulling.GetObject(pos).gameObject;
        StartCoroutine(HideObject(obj, duration));
    }

    // 판정 UI 보여줌
    public void FloatJudgeEffect(Vector3 pos, EnumList.NOTE_JUDGE judge, float duration, bool showText, string text = "")
    {
        GameObject judgeShow = judgePulling.GetObject(pos).gameObject;
        judgeShow.GetComponentInChildren<Image>().sprite = judgeSprites[(int)judge];
        judgeShow.GetComponentInChildren<Text>().text = text;
        judgeShow.GetComponentInChildren<Text>().enabled = showText;
        judgeShow.SetActive(true);

        // 판정 UI 표시 시간
        StartCoroutine(HideObject(judgeShow, duration));
    }

    public void ShowDefeatScreen()
    {
        if (defeatScreenObj.activeSelf) return;
        defeatScreenObj.SetActive(true);
        touch = false;
    }

    public void ShowWinScreen(int fromMoney, int toMoney)
    {
        if (winScreenObj.activeSelf) return;
        winScreenObj.SetActive(true);
        winScreenObj.GetComponent<ControlResultScreen>().activeWorthAnim(fromMoney, toMoney);
    }

    public Animator GetEffectAnimator(EnumList.MAIN_UI_SKILL_EFFECT_INDEX index)
    {
        return effectAnims[(int)index];
    }

    IEnumerator HideObject(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);

        obj.SetActive(false);
    }

    IEnumerator CheckBulletDistance(BulletBezierMove bullet)
    {
        while (true)
        {
            // 오브젝트 거리 계산 후 데미지를 가함
            float curDistance = Vector3.SqrMagnitude(bullet.transform.position - bullet.targetObj);

            if (curDistance < 100.0f)
            {
                if (battleManager != null)
                {
                    // attack from player to monster
                    if (!bullet.targetIsPlayer) battleManager.targetAttack(true);
                    // attack from monster to player
                    else battleManager.targetAttack(false);
                }

                StartCoroutine(HideObject(bullet.gameObject, bullet.destroyTime));
                break;
            }
            yield return null;
        }
    }
}
