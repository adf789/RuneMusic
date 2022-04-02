using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MPTKDemoCatchMusic;
//using PlayFab.ClientModels;
//using PlayFab;

public class MainStage : MonoBehaviour
{
    public TargetPatern monsterPatern, playerPatern;
    public RuneMusicView musicControl;
    public MainUIManager uiManager;
    private LinkedList<int> damageStack;
    private StringBuilder sb;
    private Vector3 skillPos;
    private TargetPref monsterData, playerData;
    private SkillManager skillManager;
    private int combo;
    private bool isLocal, showResultScreen;
    private float blockCancelComboRate;
    private EnumList.NOTE_JUDGE curJudge;



    public void Init()
    {
        isLocal = PlayerPrefs.GetInt("isLocal") == 1;
        damageStack = new LinkedList<int>();
        sb = new StringBuilder();
        showResultScreen = false;
        blockCancelComboRate = 0f;

        // 플레이어의 스텟
        playerData = PlayerPref.Instance;
        playerData.Init();
        playerPatern.SetTargetData(playerData);


        // 몬스터의 스텟
        monsterData = MonsterPref.monsterQueue.Peek();
        MonsterPref.InitMonsters();
        monsterPatern.SetTargetData(monsterData);


        skillManager = ((PlayerPref)playerData).skillManager;
        skillManager.battleManager = this;
        // 스킬 (상시)
        skillManager.caster = playerPatern;
        skillManager.target = monsterPatern;
        skillManager.CheckConditionsAndExcute(EnumList.SKILL_SITUATION.ALWAYS);

        // 룬 속성 확인용
        sb.Length = 0;
        sb.Append(PlayerPref.Instance.GetOriginHp() + " " + PlayerPref.Instance.GetTotalAtk() + " " + PlayerPref.Instance.GetTotalDef());

        uiManager.SetStatText(sb.ToString());
        uiManager.SetPlayerCurHpText(playerData.GetCurHp().ToString());
        uiManager.SetPlayerMaxHpText(playerData.GetOriginHp().ToString());
        uiManager.SetMonsterCurHpText(monsterData.GetCurHp().ToString());
        uiManager.SetMonsterMaxHpText(monsterData.GetOriginHp().ToString());
    }

    public void StartEventTouchNote(RuneNoteView note)
    {
        // 노트 터치
        EnumList.NOTE_JUDGE judge = note.touchNote();
        curJudge = judge;
        skillPos = note.transform.position;

        // 판정이 방어인 경우
        if (note.block)
        {
            // 방어 이벤트
            targetReadyGetDamage(judge, skillPos, playerPatern.hitT.position, true);
            checkJudgement(skillPos, judge, true);
            return;
        }

        checkJudgement(skillPos, judge, false);
        targetReadyGetDamage(judge, skillPos, monsterPatern.hitT.position, false);


        // 스킬 (해당 판정 시)
        skillManager.CheckConditionsAndExcute(EnumList.SKILL_SITUATION.JUDGE);
    }

    // 판정에 따른 데미지 측정
    private int mesureDamage(EnumList.NOTE_JUDGE judge, int flatDmg)
    {
        int addDamage = (int)(combo * 0.2f);

        float judgeCoef = 0f;

        switch (judge)
        {
            case EnumList.NOTE_JUDGE.MISS:
                judgeCoef = 0.2f;
                break;
            case EnumList.NOTE_JUDGE.BAD:
                judgeCoef = 0.4f;
                break;
            case EnumList.NOTE_JUDGE.GOOD:
                judgeCoef = 0.6f;
                break;
            case EnumList.NOTE_JUDGE.GREAT:
                judgeCoef = 0.8f;
                break;
            case EnumList.NOTE_JUDGE.PERFECT:
                judgeCoef = 1.0f;
                break;
        }

        return (int)((flatDmg + addDamage) * UnityEngine.Random.Range(0.95f, 1.05f) * judgeCoef);
    }

    // 플레이어가 데미지를 입는 이벤트
    private void playerGetDamage()
    {
        // 플레이어 체력 조정
        int damage = monsterData.GetTotalAtk() - playerData.GetTotalDef();
        if (damage < 0) damage = 0;

        playerData.ModifyHp(-damage);

        // 플레이어의 체력바 수정
        repaintHp(playerData);

        // 타격 이펙트
        uiManager.CreateAttackEffect(playerPatern.hitT.position);

        // 플레이어 피해 애니메이션
        playerPatern.hitAnim();

        // 데미지 텍스트 애니메이션
        if (damage == 0)
        {
            uiManager.FloatText(damage.ToString(), playerPatern.dmgTextT.position, Color.gray, 40, 0.8f);
            uiManager.GetEffectAnimator(EnumList.MAIN_UI_SKILL_EFFECT_INDEX.IGNORE_DAMAGED).SetTrigger("SetGuard");
        }
        else uiManager.FloatText(damage.ToString(), playerPatern.dmgTextT.position, Color.white, 100, 0.8f);

        // 스킬 (피해를 입은 경우)
        skillManager.getDmgBuffer = damage;
        skillManager.CheckConditionsAndExcute(EnumList.SKILL_SITUATION.WHEN_DAMAGED);

        // 플레이어가 진 경우
        if (playerData.GetCurHp() <= 0) deathGame();
    }

    // 몬스터가 데미지를 입는 이벤트
    private void monsterGetDamage()
    {
        // 몬스터의 체력 조정
        if (damageStack.Count == 0) return;
        int damage = damageStack.First.Value - monsterData.GetTotalDef();
        if (damage < 0) damage = 0;
        damageStack.RemoveFirst();

        monsterData.ModifyHp(-damage);


        // 몬스터의 체력바 수정
        repaintHp(monsterData);

        // 데미지 텍스트 애니메이션
        if (damage == 0) uiManager.FloatText(damage.ToString(), monsterPatern.dmgTextT.position, Color.gray, 40, 0.8f);
        else uiManager.FloatText(damage.ToString(), monsterPatern.dmgTextT.position, Color.white, 100, 0.8f);

        // 타격 이펙트
        uiManager.CreateAttackEffect(monsterPatern.hitT.position);

        // 몬스터 피해 애니메이션
        monsterPatern.hitAnim();

        // 스킬 (피해를 입힌 경우)
        skillManager.atkDmgBuffer = damage;
        skillManager.CheckConditionsAndExcute(EnumList.SKILL_SITUATION.WHEN_ATTACK);

        // 몬스터가 죽은 경우
        if (monsterData.GetCurHp() <= 0) winGame();
    }

    private void repaintHp(TargetPref targetPref)
    {
        int curHp = targetPref.GetCurHp();
        int maxHp = targetPref.GetOriginHp();
        if (targetPref.GetType() == typeof(PlayerPref))
        {
            // 플레이어 체력바
            uiManager.SetPlayerHpBar((float)curHp / maxHp);

            // 플레이어 체력 텍스트
            uiManager.SetPlayerCurHpText(curHp.ToString());
            uiManager.SetPlayerMaxHpText(maxHp.ToString());
        }
        else
        {
            // 플레이어 체력바
            uiManager.SetMonsterHpBar((float)curHp / maxHp);

            // 플레이어 체력 텍스트
            uiManager.SetMonsterCurHpText(curHp.ToString());
            uiManager.SetMonsterMaxHpText(maxHp.ToString());
        }
    }

    // 해당 대상에게 발사 이펙트와 데미지를 줌(데미지를 주기위한 시작)
    public void targetReadyGetDamage(EnumList.NOTE_JUDGE judge, Vector3 start, Vector3 finish, bool targetIsPlayer)
    {
        bool isDefeat = uiManager.defeatScreenObj.activeSelf;

        // Player's Attack
        if (!targetIsPlayer && !isDefeat)
            damageStack.AddLast(mesureDamage(judge, playerData.GetTotalAtk()));
        // Monster's Attack
        else
            ((MonsterPref)monsterData).setJudgeDmg(judge);

        if (!isDefeat)
            uiManager.FireBullet(start, finish, targetIsPlayer);
        // 졌을 경우 발사 이펙트는 전부 플레이어 방향으로 전환
        else
            uiManager.FireBullet(start, playerPatern.hitT.position, true);
    }

    // 어떤 객체가 공격을 실행하는 이벤트 (발사 이펙트가 도착했을 경우)
    public void targetAttack(bool isPlayer)
    {
        // Player's Attack
        if (isPlayer)
        {
            playerPatern.attackAnim();
            monsterGetDamage();
        }
        // Monster's Attack
        else
        {
            monsterPatern.attackAnim();
            playerGetDamage();
        }
    }

    public void checkJudgement(Vector3 pos, EnumList.NOTE_JUDGE judge, bool isBlock)
    {
        // 스킬 조건 중 최근 판정
        skillManager.judgeBuffer = judge;

        if (judge == EnumList.NOTE_JUDGE.MISS)
        {
            if (blockCancelComboRate == 0f)
                combo = 0;
            else if (UnityEngine.Random.Range(0f, 1f) >= blockCancelComboRate)
                combo = 0;
            // 콤보 텍스트 비활성화
            uiManager.FloatJudgeEffect(pos, judge, 1.0f, false);
        }
        else
        {
            // 판정이 block인 경우에 콤보 계산을 하지않음
            if (isBlock) uiManager.FloatJudgeEffect(pos, judge, 1.0f, false);
            else
            {
                sb.Length = 0;
                sb.Append(++combo);
                sb.Append(" Phrase!");
                uiManager.FloatJudgeEffect(pos, judge, 1.0f, true, sb.ToString());
            }
        }
    }

    // 사망 이벤트
    public void deathGame()
    {
        if (showResultScreen) return;
        showResultScreen = true;

        // 패시브 스킬 리셋
        skillManager.UnsetPassiveSkills();
        musicControl.musicStop(5.0f);

        uiManager.ShowDefeatScreen();
    }

    // 승리 이벤트
    public void winGame()
    {
        if (showResultScreen) return;
        showResultScreen = true;

        // 패시브 스킬 리셋
        skillManager.UnsetPassiveSkills();
        musicControl.musicStop(0.0f);

        int curRS = 0, addRS = ((MonsterPref)monsterData).gold;
        // 현재 가진 금액 불러오기
        if (isLocal)
        {
            curRS = ItemPref.instance.Gold;
        }
        else
        {
            //curRS = 0;
            //PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (result) => curRS = result.VirtualCurrency["RS"], (error) => curRS = 0);
        }

        // 결과화면 출력
        uiManager.ShowWinScreen(curRS, curRS + addRS);

        // add Money
        if (isLocal)
        {
            ItemPref.instance.Gold = curRS + addRS;
            PlayerPref.Instance.AddExp(((MonsterPref)monsterData).experience);
        }
        else
        {
            //var request = new AddUserVirtualCurrencyRequest() { VirtualCurrency = "RS", Amount = addRS };
            //PlayFabClientAPI.AddUserVirtualCurrency(request, (result) =>
            //{
            //// 획득 애니메이션 실행
            //winScreenObj.GetComponent<ControlResultScreen>().activeWorthAnim(curRS, addRS);
            //}, (error) => print("돈 획득 실패"));
        }
    }

    public void ExitBattle()
    {
        MonsterPref.monsterQueue.Clear();
        musicControl.StopAllCoroutines();
        skillManager.UnsetPassiveSkills();
    }

    public void StartSkill(EnumList.SKILLS skill, TargetPatern target, float obj1, float obj2)
    {
        switch (skill)
        {
            case EnumList.SKILLS.DOUBLE_ATTACK:
                StartCoroutine(DoubleAttack(target));
                break;
            case EnumList.SKILLS.HEAL:
                StartCoroutine(Heal(target, obj1));
                break;
            case EnumList.SKILLS.THORNS:
                StartCoroutine(Thorns(target, obj1));
                break;
            case EnumList.SKILLS.DOT_DAMAGE:
                StartCoroutine(DotAttack(target, obj1, obj2));
                break;
            case EnumList.SKILLS.NERF_DEF:
                StartCoroutine(NerfDef(target, obj1, obj2));
                break;
            case EnumList.SKILLS.IGNORE_DAMAGED:
                StartCoroutine(IgnoreDamaged(target, obj1));
                break;
            case EnumList.SKILLS.STATUP:
                StartCoroutine(AllStatsUp(target, obj1, obj2));
                break;
        }
    }

    // ---------------- Skills -----------------------
    IEnumerator DoubleAttack(TargetPatern target)
    {
        uiManager.FloatText("더블 어택!", skillPos, Color.white, 50, 0.8f);

        yield return new WaitForSeconds(0.5f);

        targetReadyGetDamage(curJudge, skillPos, target.hitT.position, false);
    }

    IEnumerator Heal(TargetPatern target, float amount)
    {
        uiManager.FloatText("회복!", skillPos, Color.white, 50, 0.8f);

        TargetPref targetData = target.GetTargetData();
        targetData.ModifyHp((int)amount);
        repaintHp(targetData);

        yield return null;
    }

    IEnumerator Thorns(TargetPatern target, float amount)
    {
        uiManager.FloatText("반사!", skillManager.caster.hitT.position, Color.blue, 50, 0.8f);

        TargetPref targetData = target.GetTargetData();
        targetData.ModifyHp((int)amount);
        repaintHp(targetData);

        yield return null;
    }

    IEnumerator DotAttack(TargetPatern target, float amount, float duration)
    {
        uiManager.FloatText("맹독!", target.hitT.position, Color.blue, 50, 0.8f);
        float durationUnit = 0.5f;

        for(float i = 0; i < duration; i += durationUnit)
        {
            TargetPref targetData = target.GetTargetData();
            targetData.ModifyHp((int)amount);
            repaintHp(targetData);

            // 데미지 텍스트 애니메이션
            uiManager.FloatText(((int)amount).ToString(), target.dmgTextT.position, Color.green, 50, 0.8f);
            yield return new WaitForSeconds(durationUnit);
        }
    }

    public void setBlockCancelComboRate(float rate)
    {
        blockCancelComboRate = rate;
    }

    IEnumerator NerfDef(TargetPatern target, float amount, float duration)
    {
        uiManager.FloatText("방어감소!", target.hitT.position, Color.red, 100, 0.8f);

        TargetPref targetData = target.GetTargetData();
        targetData.setDefRate(amount, false);

        yield return new WaitForSeconds(duration);
        targetData.setDefRate(amount, true);
    }

    IEnumerator IgnoreDamaged(TargetPatern target, float duration)
    {
        uiManager.FloatText("무적!", target.hitT.position, Color.white, 100, 0.8f);

        TargetPref targetData = target.GetTargetData();
        targetData.SetIgnoreDamaged(true);

        yield return new WaitForSeconds(duration);
        targetData.SetIgnoreDamaged(false);
    }

    IEnumerator AllStatsUp(TargetPatern target, float amount, float duration)
    {
        uiManager.FloatText("강화!", target.hitT.position, Color.blue, 100, 0.8f);

        TargetPref targetData = target.GetTargetData();
        targetData.setAllStatsRate(amount, true);

        yield return new WaitForSeconds(duration);
        targetData.setAllStatsRate(amount, false);
    }
}
