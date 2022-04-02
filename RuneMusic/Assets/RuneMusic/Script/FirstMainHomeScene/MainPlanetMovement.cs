using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlanetMovement : MonoBehaviour
{
    [SerializeField]
    private float DistanceAtSun, AngularVelocity, RotateFront;
    private float gravityTemp;
    private int theta1, theta2;
    private Transform SunT;
    private bool bStart = false;
    private Vector3 AxisRotation;

    public void InitPosition(Transform sunT)
    {
        SunT = sunT;
        // 행성의 초기 위치, 반지름 DistanceAtSun의 구 안의 랜덤 좌표
        theta1 = UnityEngine.Random.Range(1, 360);
        theta2 = UnityEngine.Random.Range(0, 360);
        float z = DistanceAtSun * Mathf.Sin(theta1);
        float x = DistanceAtSun * Mathf.Cos(theta1) * Mathf.Cos(theta2);
        float y = DistanceAtSun * Mathf.Cos(theta1) * Mathf.Sin(theta2);
        transform.localPosition = new Vector3(x, y, z);
        // 행성의 초기 회전각도
        transform.LookAt(SunT);
        AxisRotation = transform.up;
        transform.Rotate(AxisRotation, RotateFront);
        bStart = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!bStart || !SunT) return;

        float curAngularVelocity = (AngularVelocity * Mathf.Pow(DistanceAtSun, 2.0f) / Vector3.SqrMagnitude(SunT.position - transform.position)) * Time.fixedDeltaTime;
        transform.RotateAround(SunT.position, AxisRotation, curAngularVelocity);
        // 항성과 행성의 거리 유지
        if (Vector3.SqrMagnitude(SunT.position - transform.position) > Mathf.Pow(DistanceAtSun, 2.0f))
            gravityTemp += 0.02f;
        else gravityTemp = .0f;
        transform.position = Vector3.MoveTowards(transform.position, SunT.position, gravityTemp * Time.fixedDeltaTime);
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
