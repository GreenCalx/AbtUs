using UnityEngine;
using static EventLog;

public class OWCModifier : MonoBehaviour, IPoolable
{
    #region IPoolable
    public string GetName() { return gameObject.name; }
    public virtual OBJ_NATURE GetNature() { return OBJ_NATURE.NONE; }
    public virtual void OnPoolSleep()
    {
        INFO("OWCModifier::OnPoolSleep : " + gameObject.name);

    }

    public virtual void OnPoolAwake()
    { 
        INFO("OWCModifier::OnPoolAwake : " + gameObject.name);
    }

    public virtual bool UseInFeedback() { return false; }
    public virtual Transform GetTransform() { return transform; }
    #endregion
}
