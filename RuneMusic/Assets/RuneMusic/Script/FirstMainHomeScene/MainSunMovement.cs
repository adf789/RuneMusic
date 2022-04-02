using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSunMovement : MonoBehaviour
{
    [SerializeField]
    private float DistanceAtSpaceCenter, AngularVelocity;
    private LineRenderer lineRenderer;
    private int lineIndex = 0;
    private float tailTimer = 0;
    private bool bStart = false;

    public void InitPosition()
    {
        if (PlayerPref.Instance.MainHomeCharacterPos == Vector3.zero) transform.position = new Vector3(.0f, DistanceAtSpaceCenter, .0f);
        else
        {
            transform.position = PlayerPref.Instance.MainHomeCharacterPos;
            transform.rotation = PlayerPref.Instance.MainHomeCharacterRot;
            TimeSpan span = DateTime.Now - PlayerPref.Instance.rotationTimer;
            transform.RotateAround(Vector3.zero, Vector3.back, AngularVelocity * (float)span.TotalSeconds);
        }
        bStart = true;

        // 꼬리선 초기화
        lineRenderer = GetComponent<LineRenderer>();
        if(lineRenderer)
            for (int i = 0; i < lineRenderer.positionCount; i++)
                lineRenderer.SetPosition(i, transform.position);
    }

    private void Update()
    {
        if (tailTimer > 0.5f)
        {
            tailTimer = 0;
            DrawTail(transform.position);
        }
        else tailTimer += Time.deltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!bStart) return;
        transform.RotateAround(Vector3.zero, Vector3.back, AngularVelocity * Time.fixedDeltaTime);
        if(PlayerPref.Instance.IsActiveSceneName(EnumList.SCENE_NAME.MainHomeScene)){
            PlayerPref.Instance.MainHomeCharacterPos = transform.position;
            PlayerPref.Instance.MainHomeCharacterRot = transform.rotation;
        }
    }

    private void DrawTail(Vector3 pos)
    {
        if (!lineRenderer) return;
        lineRenderer.SetPosition(lineIndex++, pos);
        if (lineIndex == lineRenderer.positionCount) lineIndex = 0;
    }

    public void Start()
    {
        bStart = true;
    }

    public void Stop()
    {
        bStart = false;
    }
}
