using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    [Header("Behaviour")]
    private bool m_cursorMode = false;
    public bool cursorMode {
        get { return m_cursorMode;}
        set { 
            if (!value && m_cursorMode)  
                ResetCursorToCenter();
            m_cursorMode = value;
            }
    }

    [Header("Cursor")]
    public float cursorSpeed = 0.1f;

    [Header("Crosshair")]
    public Image crosshairImg;
    public Color crossshairColor;
    public Color cursorColor = Color.white;
    public Vector2 crosshairSizeForDefault;
    public Vector2 crosshairSizeForActionCursors;

    public Sprite cursorDefault;
    public Sprite cursorOpenHand;
    public Sprite cursorCloseHand;

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
			crosshairImg.transform.position = Input.mousePosition;
        }
    }
    public void ResetCursorToCenter()
    {
        crosshairImg.GetComponent<RectTransform>().anchoredPosition  = Vector3.zero;
    }

    public void ForceCursorToDefault() { crosshairImg.sprite = cursorDefault; crosshairImg.color = crossshairColor; }
    public void ForceCursorToCloseHand() { crosshairImg.sprite = cursorCloseHand; crosshairImg.color = cursorColor; }
    public void ForceCursorToOpenHand() { crosshairImg.sprite = cursorOpenHand; crosshairImg.color = cursorColor; }

    public void UpdateCursorFromPlayerAction(PLAYER_ACTIONS iAct)
    {
        switch (iAct)
        {
            case PLAYER_ACTIONS.MOVE:
                ForceCursorToOpenHand();
                ChangeCrosshairSize(crosshairSizeForActionCursors);
                break;
            default:
                ForceCursorToDefault();
                ChangeCrosshairSize(crosshairSizeForDefault);
                break;
        }
    }

    public bool TryChangeCrosshairColor(Color iColor)
    {
        if (crossshairColor!=iColor)
        {
            crossshairColor = iColor;
            crosshairImg.color                      = crossshairColor;
            return true;
        }
        return false;
    }
    public bool ChangeCrosshairSize(Vector2 iSize)
    {
        if (crosshairImg.rectTransform.sizeDelta!=iSize)
        {
            crosshairImg.rectTransform.sizeDelta    = iSize;
            return true;
        }
        return false;
    }
}
