using UnityEngine;

public enum EActiveObject
{
    NONE=0,
    SHROOM=1
}

public class ActiveObject : MonoBehaviour
{
    [Header("Tweaks")]
    public EActiveObject type;
    [Tooltip("No Exit effect is triggered")]
    public bool consumeObject = false;
    public bool needToBeDropped = false;

    [Header("Refs")]
    public InteractibleObject interact;

    public void Consume()
    {
        Destroy(gameObject);
    }
}
