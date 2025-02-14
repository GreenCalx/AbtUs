using UnityEngine;

public abstract class OWCListener :  MonoBehaviour
{
    public enum AXIS { gtl = 0, otc = 1, mto = 2 }
    public AXIS axis;
    private void Start()
    {
        OverWorldControl.Instance.SubscribeListener(this, axis);
        Init(OverWorldControl.Instance.getAxisValue(axis));
    }

    protected abstract void Init(float axis_value);
    public abstract void Call(float axis_value);

}
