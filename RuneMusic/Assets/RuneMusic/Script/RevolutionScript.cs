using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolutionScript : MonoBehaviour
{
    [Header("BaseSun"), SerializeField]
    private Transform SunT;
    [SerializeField]
    private int DistanceAtCenterOfUniverse, AngularVelocityOfSunPerSec;
    [Header("Planet"), SerializeField]
    private Transform[] PlanetTs;
    [SerializeField]
    private int[] DistanceAtSun, AngularVelocityPerSec;

    // Start is called before the first frame update
    void Start()
    {
        SunT.position = new Vector3(.0f, DistanceAtCenterOfUniverse, .0f);

        for(int i = 0; i < PlanetTs.Length; i++)
        {
            if (!PlanetTs[i].gameObject.activeSelf) continue;
            PlanetTs[i].position = new Vector3(.0f, DistanceAtSun[i], .0f) + SunT.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < PlanetTs.Length; i++)
        {
            if (!PlanetTs[i].gameObject.activeSelf) continue;
            float realDistance = Vector3.Distance(PlanetTs[i].position, SunT.position);
            float totalAngularVelocity = AngularVelocityPerSec[i] * Time.deltaTime * (DistanceAtSun[i] / realDistance);
            PlanetTs[i].RotateAround(SunT.position, Vector3.forward, totalAngularVelocity);
            Debug.Log(totalAngularVelocity);
        }

        SunT.RotateAround(Vector3.zero, Vector3.forward, AngularVelocityOfSunPerSec * Time.deltaTime);
    }

    private void RotateFrom(Transform rotator, Vector3 center, int distance, int velocity)
    {
        
    }
}
