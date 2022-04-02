using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using UnityEditor;

public class FirstUIManager : MonoBehaviour
{
    public TextAsset runeDB;
    public Text helpText;         
    public GameObject[] objects;
    [SerializeField]
    private List<Sprite> ItemSprites;
    private bool isIncrease, initAlpha;

    private void Start()
    {
        // 에디터인지 실제 어플로 실행하는 경우인지 판단하여 데이터를 불러옴
        PlayerPrefs.SetString("DebugLog", "0");
#if UNITY_EDITOR
        //PlayerPrefs.SetInt("isLocal", 1);
        PlayFabLogin("1535");
#else
        LoginGoogleAndPlayfab();
#endif
        isIncrease = true;
        LoadHaveItems();
        initAlpha = false;
        StartCoroutine(IdleAnim());
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            PlayerPref.Instance.LoadScene(EnumList.SCENE_NAME.MainHomeScene);
        }
        flickerText();

        // 주변 행성들 일정 주기마다 알파값 증가
        if (!initAlpha)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf) break;

                objects[i].GetComponent<CanvasGroup>().alpha += Time.deltaTime;
                if (i == objects.Length - 1 && objects[i].GetComponent<CanvasGroup>().alpha >= 1.0f) initAlpha = true;
            }
        }
    }

    // "Touch Screen" 깜빡임
    private void flickerText()
    {
        if (helpText.color.a >= 1) isIncrease = false;
        else if (helpText.color.a <= 0) isIncrease = true;

        Color color = helpText.color;
        if (isIncrease)
        {
            color.a += 2 * Time.deltaTime;
        }
        else
        {
            color.a -= 2 * Time.deltaTime;
        }
        helpText.color = color;
    }

    private void LoadHaveItems()
    {
        CoroutineHandler.Instance.StartCoroutine(ItemPref.instance.loadItemDB(ItemSprites));
        ItemPref.instance.Gold += 200000;
    }

    IEnumerator IdleAnim()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(2.0f, 5.0f));
            objects[i].SetActive(true);
        }

    }

    public void GoogleLogout()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
        // 구글 로그아웃 이벤트
    }

    private void LoginGoogleAndPlayfab()
    {
        // 구글 로그인 및 플레이팹 로그인 or 회원가입
        try
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            Social.localUser.Authenticate((success) =>
            {
                if (success)
                {
                    PlayFabLogin();
                }
                else { PlayerPrefs.SetString("DebugLog", "구글 로그인 실패"); }
            });
        }
        catch (System.Exception e)
        {
            PlayerPrefs.SetString("DebugLog", e + "\n" + e.Message);
        }
    }

    private void PlayFabLogin()
    {
        try
        {
            // 플레이팹 로그인
            PlayerPrefs.SetString("DebugLog", "플레이팹 로그인 시도");
            var request = new LoginWithEmailAddressRequest { Email = Social.localUser.id + "@rune.com", Password = Social.localUser.id };
            PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
            {
                PlayerPrefs.SetString("DebugLog", "플레이팹 로그인 성공");
                GameDatas.AlreadyLoadAccount = true;
            }, (error) => PlayFabRegister());
        }
        catch (System.Exception e)
        {
            PlayerPrefs.SetString("DebugLog", e + "\n" + e.Message);
        }
    }

    private void PlayFabLogin(string id)
    {
        try
        {
            // 플레이팹 로그인
            Debug.Log("플레이팹 로그인 시도");
            var request = new LoginWithEmailAddressRequest { Email = id + "@rune.com", Password = id + "112233" };
            PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
            {
                Debug.Log("플레이팹 로그인 성공");
                GameDatas.AlreadyLoadAccount = true;
            }, (error) => PlayFabRegister(id));
        }
        catch (System.Exception e)
        {
            Debug.Log(e + "\n" + e.Message);
        }
    }

    private void PlayFabRegister()
    {
        try
        {
            PlayerPrefs.SetString("DebugLog", "플레이팹 회원가입 시도");
            // 플레이팹 계정 추가
            // id 조건 : '@' '.' 존재
            // pw 조건 : 6 ~ 100자
            // name 조건 : 3 ~ 20자
            var request = new RegisterPlayFabUserRequest { Email = Social.localUser.id + "@rune.com", Password = Social.localUser.id, Username = Social.localUser.userName };
            PlayFabClientAPI.RegisterPlayFabUser(request, (result) => PlayFabLogin(), (error) =>
            {
                PlayerPrefs.SetString("DebugLog", "플레이팹 회원가입 실패\nId : " + Social.localUser.id + "@rune.com" + "\nUsername : " + Social.localUser.userName + "\n" + error.ErrorMessage);
            });
        }
        catch (System.Exception e)
        {
            PlayerPrefs.SetString("DebugLog", e + "\n" + e.Message);
        }
    }

    private void PlayFabRegister(string id)
    {
        try
        {
            Debug.Log("플레이팹 회원가입 시도");
            // 플레이팹 계정 추가
            // id 조건 : '@' '.' 존재
            // pw 조건 : 6 ~ 100자
            // name 조건 : 3 ~ 20자
            var request = new RegisterPlayFabUserRequest { Email = id + "@rune.com", Password = id + "112233", Username = id };
            PlayFabClientAPI.RegisterPlayFabUser(request, (result) => PlayFabLogin(), (error) => {
                Debug.Log("플레이팹 회원가입 실패\nId : " + id + "@rune.com" + "\nUsername : " + id + "\n" + error.ErrorMessage); 
                Application.Quit();
            });
        }
        catch (System.Exception e)
        {
            Debug.Log(e + "\n" + e.Message);
        }
    }
}
