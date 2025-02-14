using System;
using UnityEngine;
using UnityEngine.Rendering;

public enum GTL_TYPE { EXTRA = 0, MAIN = 1, SUN=2, LIGHT=4, WATER=3}

[Serializable]
public class GTLModifier<T, K> : MonoBehaviour
{
    public T modifierTarget;
    public GTL_TYPE gtlType = GTL_TYPE.EXTRA;
    protected float m_weight;
    public float weight
    {
        get { return m_weight; }
        set {
            m_weight = Mathf.Clamp(value, 0f,1f);
            ChangeTargetWeight(m_weight);
        }
    }
    public bool isActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        init();
    }

    protected void init()
    {
        OverWorldControl.Instance.SubscribeGTL(this);
    }

    public virtual void ChangeTarget (K iTarget) {}
    public virtual void ChangeTargetWeight(float iValue) {}

    public void Deactivate()
    {
        weight = 0f;
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        weight = 1f;
        gameObject.SetActive(true);
    }
}


