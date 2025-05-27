using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    [Header("Mand Refs")]
    public UICursorCollection cursorCollection;

    public UICrosshair crosshair;
    public UIInfoPanel infoPanel;

    [Header("UI Action Mode")]
    public Transform UIAction_Handle;
    public UIActionBubbles actionWheel;
    [Header("Behaviour")]
    private bool m_cursorMode = false;
    public bool cursorMode
    {
        get { return m_cursorMode; }
        set
        {
            if (!value && m_cursorMode)
                ResetCursorToCenter();
            m_cursorMode = value;
        }
    }

    [Header("Cursor")]
    public float cursorSpeed = 0.1f;

    [Header("Crosshair")]
    public Color crossshairColor;
    public Color cursorColor = Color.white;
    public Vector2 crosshairSizeForDefault;
    public Vector2 crosshairSizeForActionCursors;

    public Sprite cursorDefault;
    public Sprite cursorOpenHand;
    public Sprite cursorCloseHand;

    public Sprite cursorWheel2;
    public Sprite cursorWheel3;
    public Sprite cursorWheel4;

    private static UIGame instance = null;
    public static UIGame Instance => instance;

    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    void Update()
    {
        if (cursorMode)
        {
            crosshair.transform.position = Input.mousePosition;
        }
    }
    public void ResetCursorToCenter()
    {
        crosshair.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }

    public void UpdateCursorFromPlayerAction(PLAYER_ACTIONS iAct)
    {
        crosshair.ChangeSprite(GetActionSprite(iAct));
        switch (iAct)
        {
            case PLAYER_ACTIONS.DEFAULT:
                ChangeCrosshairSize(crosshairSizeForDefault);
                break;
            default:
                ChangeCrosshairSize(crosshairSizeForActionCursors);
                break;
        }
    }

    public void ForceHideCursor()
    {
        crosshair.ChangeSprite(GetActionSprite(PLAYER_ACTIONS.NONE));
    }

    public void SetCursorToDefault()
    {
        ChangeCrosshairSize(crosshairSizeForDefault);
        crosshair.ChangeSprite(GetActionSprite(PLAYER_ACTIONS.DEFAULT));
    }

    public void UpdateAltCursorFromPlayerAction(PLAYER_ACTIONS iAct)
    {
        crosshair.ChangeSprite(GetAltActionSprite(iAct));
    }

    public void EnterActionWheelMode(PLAYER_ACTIONS[] iWheelActions)
    {
        UIAction_Handle.gameObject.SetActive(true);
        actionWheel.Init(iWheelActions);
    }

    public void ExitActionWheelMode()
    {
        actionWheel.Clear();
        UIAction_Handle.gameObject.SetActive(false);
    }

    public PLAYER_ACTIONS GetSelectedAction()
    {
        return actionWheel.selector.selectedAction;
    }

    public bool TryChangeCrosshairColor(Color iColor)
    {
        if (crossshairColor != iColor)
        {
            crossshairColor = iColor;
            crosshair.ChangeColor(iColor);
            return true;
        }
        return false;
    }
    public bool ChangeCrosshairSize(Vector2 iSize)
    {
        // if (crosshair.rt.sizeDelta != iSize)
        Vector2 selfScale = new Vector2(crosshair.rt.localScale.x, crosshair.rt.localScale.y);
        if (selfScale != iSize)
        {
            crosshair.rt.localScale = new Vector2(iSize.x, iSize.y);
            // crosshair.rt.sizeDelta = iSize;
            return true;
        }
        return false;
    }

    public Sprite GetActionSprite(PLAYER_ACTIONS iAct)
    {
        return cursorCollection.GetImageFromAction(iAct);
    }
    public Sprite GetAltActionSprite(PLAYER_ACTIONS iAct)
    {
        return cursorCollection.GetAltImageFromAction(iAct);
    }
    public Sprite GetHResActionSprite(PLAYER_ACTIONS iAct)
    {
        return cursorCollection.GetHResImageFromAction(iAct);
    }
    public Sprite GetHResAltActionSprite(PLAYER_ACTIONS iAct)
    {
        return cursorCollection.GetHResAltImageFromAction(iAct);
    }
    
}
