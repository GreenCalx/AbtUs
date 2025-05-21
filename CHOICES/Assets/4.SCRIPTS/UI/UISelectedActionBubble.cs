using UnityEngine;
using UnityEngine.UI;

public class UISelectedActionBubble : MonoBehaviour
{
    public PLAYER_ACTIONS selectedAction;
    public Image selfImg;
    public Rigidbody2D selfRB;

    void Update()
    {

    }

    public void ChangeSelectedAction(UIActionBubble iActionBubble)
    {
        selfImg.sprite = iActionBubble.selfImg.sprite;
        selectedAction = iActionBubble.associatedAction;
    }
}
