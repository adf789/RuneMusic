using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveEffect : MonoBehaviour
{
    public ControlResultScreen resultScreen;

    public void active()
    {
        resultScreen.activeWinEffect();
    }
}
