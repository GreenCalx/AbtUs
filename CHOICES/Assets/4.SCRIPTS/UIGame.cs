using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    [Header("Crosshair")]
    public Image crosshairImg;
    public Color crossshairColor;
    public Vector2 crosshairSize;


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

    public bool TryChangeCrosshairColor(Color iColor)
    {
        if (crossshairColor!=iColor)
        {
            crossshairColor = iColor;
            RefreshCrosshair();
            return true;
        }
        return false;
    }
    public bool ChangeCrosshairSize(Vector2 iSize)
    {
        if (crosshairSize!=iSize)
        {
            crosshairSize = iSize;
            RefreshCrosshair();
            return true;
        }
        return false;
    }

    public void RefreshCrosshair()
    {
        crosshairImg.color                      = crossshairColor;
        crosshairImg.rectTransform.sizeDelta    = crosshairSize; 
    }
}
