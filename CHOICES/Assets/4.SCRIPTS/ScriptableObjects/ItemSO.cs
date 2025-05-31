using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public string info;
    public float showInfoDuration = 5f;
    public PLAYER_ACTIONS[] availableActions;
    public float shatterTime = 1f; 
}
