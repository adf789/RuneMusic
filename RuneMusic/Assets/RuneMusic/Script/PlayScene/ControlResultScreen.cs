using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlResultScreen : MonoBehaviour
{
    public bool isWin = false;
    public float fadeTime, maxAlpha;
    public GameObject winEffect;
    public Text winGoldText;
    public MainStage battleManager;
    private Image screen;
    private GameObject btnRe, btnHome;
    [Header("돈 애니메이션")]
    public float delay;
    public float duration;

    private void Start()
    {
        screen = GetComponent<Image>();
        btnRe = transform.GetChild(0).gameObject;
        btnHome = transform.GetChild(1).gameObject;
        if (maxAlpha > 1.0f) maxAlpha = 1.0f;
        else if (maxAlpha < 0.0f) maxAlpha = 0.0f;
    }
    // Update is called once per frame
    void Update()
    {
        resultFadeIn();
    }

    private void resultFadeIn()
    {
        // fadeTime 동안 스크린 이미지의 알파값을 0.5f 까지 증가
        Color color = screen.color;
        Color textColor = color;
        if (isWin)
            textColor = winGoldText.color;
        // fadeTime 설정하지 않은 경우 maxAlpha 까지 투명도를 밝힘
        if (fadeTime <= 0)
        {
            color.a = maxAlpha;
            if(isWin)
                textColor.a = maxAlpha;
            screen.color = color;
            winGoldText.color = textColor;
            btnRe.SetActive(true);
            btnHome.SetActive(true);
            return;
        }
        if (color.a > maxAlpha)
        {
            btnRe.SetActive(true);
            btnHome.SetActive(true);
            return;
        }
        color.a += maxAlpha / fadeTime * Time.deltaTime;
        screen.color = color;

        if (isWin)
        {
            textColor.a += maxAlpha / fadeTime * Time.deltaTime;
            winGoldText.color = textColor;
        }
    }

    public void onClickHome()
    {
        battleManager.ExitBattle();
        //SceneManager.LoadScene(1, LoadSceneMode.Single);
        //GameManager.LoadScene(EnumList.SCENE_NAME.MainHomeScene);
        PlayerPref.Instance.LoadScene(EnumList.SCENE_NAME.MainHomeScene);
    }

    public void onClickReplay()
    {
        //SceneManager.LoadScene(3, LoadSceneMode.Single);
        //GameManager.LoadScene(EnumList.SCENE_NAME.PlayGameScene);
        PlayerPref.Instance.LoadScene(EnumList.SCENE_NAME.PlayGameScene);
    }

    public void activeWinEffect()
    {
        winEffect.SetActive(true);
    }

    public void activeWorthAnim(float prevWorth, float curWorth)
    {
        StartCoroutine(countingWorth(prevWorth, curWorth));
    }

    IEnumerator countingWorth(float from, float to)
    {
        yield return new WaitForSeconds(delay);

        float offset = (to - from) / duration;

        while(from < to)
        {
            from += offset * Time.deltaTime;
            winGoldText.text = (int)from + "";
            yield return null;
        }

        from = to;
        winGoldText.text = (int)from + "";
    }
}
