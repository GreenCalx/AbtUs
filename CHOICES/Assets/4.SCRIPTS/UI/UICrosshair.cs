using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UICrosshair : MonoBehaviour
{
    public RectTransform rt;
    private Image img;

    void Start()
    {
        img = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
    }

    public void ChangeSprite(Sprite iSprite)
    {
        if (iSprite == null)
        {
            img.enabled = false;
            return;
        }

        img.sprite = iSprite;
        img.enabled = true;

    }

    public void ChangeColor(Color iColor)
    {
        img.color = iColor;
    }
}
