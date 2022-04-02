using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreUIManager : MonoBehaviour
{
    public RectTransform dirT;
    private Vector3 touchOriginPos;
    private Vector2 dirAnchor, dirAnchorPos;
    private bool onTouch;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        dirAnchor = Vector2.zero;
        dirAnchorPos = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        KeyEvent();
        //if (TouchPhase.Began == Input.GetTouch(0).phase)
        //    Debug.Log(Input.GetTouch(0).position);
        checkMoveDirection();
        time = Time.deltaTime;
    }

    public void onClickBack()
    {
        //SceneManager.LoadScene(PlayerPrefs.GetInt("prevScene"), LoadSceneMode.Single);
        //GameManager.LoadPrevScene();
        PlayerPref.Instance.LoadPrevScene();
    }

    private void KeyEvent()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            onTouch = true;
            touchOriginPos = Input.mousePosition;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            onTouch = false;
        }
    }

    // 화면의 이동방향을 설정
    private void checkMoveDirection()
    {
        if (!onTouch) return;
        Vector3 dir = touchOriginPos - Input.mousePosition;
        if (dir.magnitude < 30) return;

        int dirIndex = 0;
        float x = dir.x;
        float y = dir.y;

        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            // x축 방향
            if (x > 30)
            {
                // 오른쪽
                dirAnchor.x = 1.0f;
                dirAnchor.y = 0.5f;
            }
            else if (x <= 30)
            {
                // 왼쪽
                dirAnchor.x = 0.0f;
                dirAnchor.y = 0.5f;
                dirIndex = 1;
            }
        }
        else
        {
            // y축 방향
            if (y > 30)
            {
                // 위쪽
                dirAnchor.x = 0.5f;
                dirAnchor.y = 1.0f;
                dirIndex = 2;
            }
            else if (y <= 30)
            {
                // 아래쪽
                dirAnchor.x = 0.5f;
                dirAnchor.y = 0.0f;
                dirIndex = 3;
            }
        }

        dirT.anchorMin = dirAnchor;
        dirT.anchorMax = dirAnchor;
        dirAnchorPos.x = 0;
        dirAnchorPos.y = 0;

        switch (dirIndex)
        {
            case 0:
                dirT.rotation = Quaternion.Euler(0, 0, 0);
                dirAnchorPos.x = -50;
                break;
            case 1:
                dirT.rotation = Quaternion.Euler(0, 180, 0);
                dirAnchorPos.x = 50;
                break;
            case 2:
                dirT.rotation = Quaternion.Euler(0, 90, 0);
                dirAnchorPos.y = -50;
                break;
            case 3:
                dirT.rotation = Quaternion.Euler(0, -90, 0);
                dirAnchorPos.y = 50;
                break;
        }

        dirT.anchoredPosition = dirAnchorPos;
    }
}
