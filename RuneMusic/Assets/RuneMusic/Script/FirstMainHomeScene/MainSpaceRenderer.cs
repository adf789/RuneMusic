using UnityEngine;

public class MainSpaceRenderer : MonoBehaviour
{
    [SerializeField]
    private MainSunMovement SunData;
    [SerializeField]
    private MainPlanetMovement[] PlanetDatas;
    [Header("항성 공전"), SerializeField]
    private bool bRevolutionSun;
    [Header("행성 공전"), SerializeField]
    private bool bRevolutionPlanet;

    // 우주의 기본 배치
    // Start is called before the first frame update
    void Start()
    {
        if(bRevolutionSun)
            SunData.InitPosition();

        if(bRevolutionPlanet)
            foreach (MainPlanetMovement planet in PlanetDatas)
            {
                if (!planet.gameObject.activeSelf) continue;
                planet.InitPosition(SunData.transform);
            }
    }

    // 행성의 렌더링 수
    public void SetPlanetCount(int count)
    {
        if (count < 0 || count >= PlanetDatas.Length) return;

        for(int i = count; i < PlanetDatas.Length; i++)
        {
            PlanetDatas[i].gameObject.SetActive(false);
        }
    }
}
