using UnityEngine;
using UnityEngine.UI;

using static Constants;

public class UISelectedActionBubble : MonoBehaviour
{
    public PLAYER_ACTIONS selectedAction;
    public Image selfImg;
    public Image shaderAnimatedImg;
    public Rigidbody2D selfRB;

    public void Reset()
    {
        selfImg.sprite = null;
        selfImg.enabled = false;
        selectedAction = PLAYER_ACTIONS.NONE;

        if (shaderAnimatedImg != null)
        {
            var rect = shaderAnimatedImg.rectTransform.rect;
            // Vector2 size = new Vector2(rect.width, rect.height);
            // shaderAnimatedImg.material.SetVector(uishad_sizeParm, size);
        }
    }

    public void ChangeSelectedAction(UIActionBubble iActionBubble)
    {
        selfImg.sprite = iActionBubble.selfImg.sprite;
        selfImg.enabled = selfImg.sprite != null;
        selectedAction = iActionBubble.associatedAction;
    }
}
