using UnityEngine;

public class OWCModifier : MonoBehaviour, IPoolable
{
    #region IPoolable
    public string GetName() { return gameObject.name; }
    public virtual OBJ_NATURE GetNature() { return OBJ_NATURE.NONE; }
    public virtual void OnPoolSleep() { }

    public virtual void OnPoolAwake() { }

    public virtual bool UseInFeedback() { return false; }
    public virtual Transform GetTransform() { return transform; }
    #endregion
}
