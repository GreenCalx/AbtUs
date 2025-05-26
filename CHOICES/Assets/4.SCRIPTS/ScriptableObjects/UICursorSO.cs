using UnityEngine;

[CreateAssetMenu(fileName = "UICursorSO", menuName = "Scriptable Objects/UICursorSO")]
public class UICursorSO : ScriptableObject
{
    public PLAYER_ACTIONS relatedAction;
    public Sprite image;
    [Tooltip("Alternative cursor image that can be used by interactibles within active PLAYER_ACTIONS state.")]
    public Sprite alt_image;
    [Tooltip("Higher Res sprite of image.")]
    public Sprite hres_image;
     [Tooltip("Higher Res sprite of alt_image.")]
    public Sprite hres_alt_image;
}
