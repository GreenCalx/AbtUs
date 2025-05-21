using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIActionBubble : MonoBehaviour
{
    public PLAYER_ACTIONS associatedAction;
    public Image selfImg;

    public UnityEvent<UIActionBubble> CollidedCB;

    void OnCollisionEnter2D(Collision2D iCollider)
    {
        CollidedCB.Invoke(this);
    }

}
