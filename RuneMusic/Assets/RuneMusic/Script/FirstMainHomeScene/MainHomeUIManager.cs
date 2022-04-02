using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class MainHomeUIManager : MonoBehaviour
{
    void Start()
    {
        noteModels = new ObjectPulling<Transform>(noteModel, 4);
        pos = Vector3.zero;
        sb = new StringBuilder();
        bFixCamera = true;

        DebugText.text = PlayerPrefs.GetString("DebugLog");

        spaceRenderer = GetComponent<MainSpaceRenderer>();
        // 장착한 서브룬의 수만큼 행성을 렌더링 함
        spaceRenderer.SetPlanetCount(ItemPref.instance.curMainRune.subRunes.Count);

        // 현재 카메라는 어느 스테이지 위에도 있지 않음
        curCameraOnStageIndex = 0;

        // 툴바 데이터 변경
        LevelText.text = PlayerPref.Instance.level.ToString();
        MoneyText.text = ItemPref.instance.Gold.ToString();
        RuneFragText.text = ItemPref.instance.RuneFrag.ToString();
        float expRate = (float)PlayerPref.Instance.curExperience / PlayerPref.Instance.maxExperience;
        ExperienceBar.fillAmount = expRate;
        sb.Length = 0;
        sb.Append(Math.Truncate((double)expRate * 1000) / 10);
        sb.Append('%');
        ExperiencePercentText.text = sb.ToString();
        
        // idle 윈도우 룬 이미지
        for(int i = 0; i < ItemPref.instance.curMainRune.subRunes.Count + 1; i++)
        {
            if (i == 0)
                IdleWindowRuneImage[i].sprite = ItemPref.instance.curMainRune.getItemImage();
            else
                IdleWindowRuneImage[i].sprite = ItemPref.instance.curMainRune.subRunes[i - 1].getItemImage();
        }
    }

    private void Update()
    {
        KeyDown();

        NoteRendering(1.0f, 2.0f);

        // 재화 자동수급창이 열려있을때 사용된 시간만큼 재화를 업데이트함
        if(bShowIdleWindow) updateReward();

        // 카메라 캐릭터 위 고정
        if (bFixCamera)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.x = CharacterT.transform.position.x;
            cameraPos.y = CharacterT.transform.position.y + 2.3f;
            Camera.main.transform.position = cameraPos;
        }
    }

    private void NoteRendering(float time, float destroyTime)
    {
        // 매 초마다 노트 렌더링
        CurTime += Time.deltaTime;
        if (CurTime > time)
        {
            CurTime = 0f;
            float range = UnityEngine.Random.Range(1.5f, 2.5f);
            float angle = UnityEngine.Random.Range(0f, 360f);
            pos.x = range * Mathf.Sin(angle) + CharacterT.position.x;
            pos.y = range * Mathf.Cos(angle) + CharacterT.position.y;

            GameObject obj = noteModels.GetObject(pos).gameObject;
            StartCoroutine(ObjectOff(obj, destroyTime));
        }
    }

    private void updateReward()
    {
        // 캐릭터 레벨 * 현재 사용된 시간
        TimeSpan span = DateTime.Now - PlayerPref.Instance.IdleTimer;
        getMoney = (int)span.TotalSeconds * PlayerPref.Instance.level;
        IdleWindowTimeText.text = curTime();
        sb.Length = 0;
        sb.Append(getMoney.ToString());
        sb.Append("골드");
        IdleWindowMoneyText.text = sb.ToString();
    }


    public void SetStageIndex(int index, bool isAdd)
    {
        int curTempIndex;
        if (isAdd)
        {
            curTempIndex = curCameraOnStageIndex + index;
            // 인덱스의 예외처리
            if (curTempIndex >= StageTs.Length) curTempIndex = 1;
            else if (curTempIndex < 1) curTempIndex = StageTs.Length - 1;
        }
        else
        {
            curTempIndex = index;
            // 인덱스의 예외처리
            if (curTempIndex >= StageTs.Length) curTempIndex = StageTs.Length - 1;
            else if (curTempIndex < 1) curTempIndex = 1;
        }

        StartCoroutine(CameraMoveToStage(curTempIndex));
    }

    // 현재 시간 format
    private string curTime()
    {
        TimeSpan span = DateTime.Now - PlayerPref.Instance.IdleTimer;
        int hour = span.Hours;
        int min = span.Minutes;
        int sec = span.Seconds;
        sb.Length = 0;
        if (hour < 10) sb.Append('0');
        sb.Append(hour);
        sb.Append(':');
        if (min < 10) sb.Append('0');
        sb.Append(min);
        sb.Append(':');
        if (sec < 10) sb.Append('0');
        sb.Append(sec);

        return sb.ToString();
    }

    IEnumerator ObjectOff(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
    }

    IEnumerator CountingWorth(float from, float to, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        float offset = (to - from) / duration;

        while (from < to)
        {
            from += offset * Time.deltaTime;
            MoneyText.text = ((int)from).ToString();
            yield return null;
        }

        from = to;
        MoneyText.text = ((int)from).ToString();
    }

    IEnumerator CameraMoveToStage(int index)
    {
        // 목적지가 캐릭터인 경우
        // 캐릭터 위치 -> 스테이지 위치인 경우
        // 스테이지 위치 -> 스테이지 위치인 경우
        Vector3 destinationPos;
        if (index == 0)
        {
            destinationPos = CharacterT.position;
            destinationPos.z = -11;
        }
        else if (curCameraOnStageIndex == 0) destinationPos = StageTs[0].position;
        else destinationPos = StageTs[index].position;

        Transform cameraT = Camera.main.transform;
        float distance = Vector3.Distance(cameraT.position, destinationPos);
        bFixCamera = false;

        while (true)
        {
            // 목적지가 캐릭터인 경우 주기적 목적지 업데이트가 필요
            if (index == 0)
            {
                destinationPos = CharacterT.position;
                destinationPos.z = -11;
            }
            // 목적지로 이동
            cameraT.position = Vector3.MoveTowards(cameraT.position, destinationPos, distance / CameraToStageTime * Time.deltaTime);

            // 경로에 도착했을때
            if (Vector3.SqrMagnitude(cameraT.position - destinationPos) <= 0.01f)
            {
                if (index == 0) bFixCamera = true;
                break;
            }
            yield return null;
        }

        // 캐릭터를 비추고 있었던 경우 최종 목적지로 이동
        if(curCameraOnStageIndex == 0)
        {
            yield return new WaitForSeconds(0.5f);

            while (true)
            {
                cameraT.position = Vector3.MoveTowards(cameraT.position, StageTs[index].position, distance / CameraToStageTime * 1.5f * Time.deltaTime);

                if (Vector3.SqrMagnitude(cameraT.position - StageTs[index].position) <= 0.01f)
                {
                    break;
                }
                yield return null;
            }
        }

        // 현재 위치인덱스를 저장
        curCameraOnStageIndex = index;
        curStageText.text = curCameraOnStageIndex.ToString();
    }
}
