using System.Collections;
using UnityEngine;

public class BulletBezierMove : MonoBehaviour
{
    public Vector3 startObj;
    public Vector3 targetObj;
    public MainStage mainEvent;
    
    public float speed;
    public float accel;
    public float destroyTime = 0f;
    public float autoHandleHeight = 1f;
    public bool rotateAlong = false;
    public bool repeat = true;
    public bool targetUpdate = true;
    public bool debugLine = true;
    public bool targetIsPlayer;

    [Header("Custom Handle Mode")]
    public bool customHandleMode = false;
    public GameObject handleObj;


    [Header("Handle Randomizer")]
    public bool handleRandomize = false;
    public float randomWidth = 1f;

    private float routeAmount, theDistance;
    private bool destroyReady = false;
    private Vector3 startVec;
    private Vector3 destinationVec;
    private Vector3 bezierRoute;
    private Vector3 bezierRouteShow;
    private Vector3 handlePos;
    private Vector3 forHeight = new Vector3(0f,1f,0f);
    private Vector3 originalHandlePos;
    private Vector3 positionBefore;

    public void Init()
    {
        if (startObj == null || targetObj == null)
        {
            return;
        }

        routeAmount = 0f;

        setFactors();
        handleManage();

        destroyReady = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        if (startObj == null || targetObj == null)
        {
            return;
        }

        if (targetUpdate)
        {
            setFactors();
            handleManage();
        }

        doHandleRandom();
        doBezier();
    }

    void setFactors() {
        startVec = startObj;
        destinationVec = targetObj;
        theDistance = Vector3.Distance(startVec, destinationVec);
    }

    void handleManage()
    {
        if (routeAmount==0f) {
            if (customHandleMode)
            {
                if (handleObj != null)
                {
                    handlePos = originalHandlePos = handleObj.transform.position;
                }
                else {
                    Debug.Log("No handle object assigned. Enforce Auto Handle Mode");
                    customHandleMode = false;
                }                
            }

            if(!customHandleMode || handleObj==null)
            {
                if (handleObj != null) {
                    handleObj.transform.position = originalHandlePos;
                }
                handlePos = originalHandlePos =(startVec + destinationVec) / 2f + forHeight * autoHandleHeight;
                
            }
        }
            
    }

    void doBezier() {
        if (routeAmount <= 1f)
        {
            speed += accel * Time.deltaTime;
            routeAmount += Time.deltaTime * speed / theDistance;
            bezierRoute = (1 - routeAmount) * (((1 - routeAmount) * startVec) + (routeAmount * handlePos)) + (routeAmount * (((1 - routeAmount) * handlePos) + (routeAmount * destinationVec)));
            transform.position = bezierRoute;
            if (rotateAlong)
            {
                transform.rotation = Quaternion.LookRotation(bezierRoute - positionBefore);
                positionBefore = bezierRoute;
            }

        }
        else if (repeat)
        {
            routeAmount = 0;
        }
    }




    //for randomize curve a bit
    Vector3 randomVec;
    float ranFlo;

    void doHandleRandom() {
        if (handleRandomize && routeAmount == 0f)
        {
            ranFlo = Random.Range(-randomWidth, randomWidth);
            randomVec.x = ranFlo;
            ranFlo = Random.Range(-randomWidth, randomWidth);
            randomVec.y = ranFlo;
            ranFlo = Random.Range(-randomWidth, randomWidth);
            randomVec.z = ranFlo;
                        
            handlePos = originalHandlePos + randomVec;
        }
    }


    //for debug curve line on editor
    bool posAB;
    Vector3 posA = new Vector3(0f, 0f, 0f);
    Vector3 posB = new Vector3(0f, 0f, 0f);


    void doBezierShow(float floatAmount)
    {
        bezierRouteShow = (1 - floatAmount) * (((1 - floatAmount) * startVec) + (floatAmount * handlePos)) + (floatAmount * (((1 - floatAmount) * handlePos) + (floatAmount * destinationVec)));
    }


    private void OnDrawGizmos()
    {        
        if (debugLine) {
            if (startObj!=null && targetObj!=null ) {
                posAB = true;
                setFactors();
                handleManage();
                doHandleRandom();
                for (float i = 0f; i < 1.1f; i += 0.1f)
                {

                    doBezierShow(i);

                    if (posAB)
                    {
                        posA = bezierRouteShow;
                        posAB = false;
                    }
                    else
                    {
                        posB = bezierRouteShow;
                        posAB = true;
                    }

                    if (i != 0f)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(posB, posA);
                    }

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(startVec, 0.5f);
                    Gizmos.DrawWireSphere(destinationVec, 0.5f);

                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(originalHandlePos, 0.5f);
                }
            }
           
        }
    }


}
