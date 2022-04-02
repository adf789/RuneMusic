using UnityEngine;

public class csClickTap : MonoBehaviour
{
    public EnumList.SUBRUNE_VIEW_TYPE type;
    private static SetRuneUIManager uiManager;
    private bool init;

    private void Start()
    {
        if (uiManager == null) uiManager = transform.GetComponentInParent<SetRuneUIManager>();
        init = true;
    }

    public void OnClickTab()
    {
        if (!init) return;
        uiManager.OnTabClick(type);
    }
}
