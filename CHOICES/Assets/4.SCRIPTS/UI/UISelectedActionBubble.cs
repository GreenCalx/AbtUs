using UnityEngine;
using UnityEngine.UI;

public class UISelectedActionBubble : MonoBehaviour
{
    public PLAYER_ACTIONS selectedAction;
    public Image selfImg;
    public Rigidbody2D selfRB;

    public void Reset()
    {
        selfImg.sprite = null;
        selfImg.enabled = false;
        selectedAction = PLAYER_ACTIONS.NONE;
    }

    public void ChangeSelectedAction(UIActionBubble iActionBubble)
    {
        selfImg.sprite = iActionBubble.selfImg.sprite;
        selfImg.enabled = selfImg.sprite != null;
        selectedAction = iActionBubble.associatedAction;
    }
}
