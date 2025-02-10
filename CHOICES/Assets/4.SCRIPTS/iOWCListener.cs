using UnityEngine;

public abstract class OWCListener :  MonoBehaviour
{
    public enum AXIS { gtl = 0, otc = 1, mto = 2 }
    public AXIS axis;
    private void Start()
    {
        OverWorldControl.Instance.SubscribeListener(this, axis);
        Init();
    }

    protected abstract void Init();
    public abstract void Call(float axis_value);

}
