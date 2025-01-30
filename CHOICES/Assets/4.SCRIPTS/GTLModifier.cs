using UnityEngine;
using UnityEngine.Rendering;

public class GTLModifier : MonoBehaviour
{
    public enum GTL_TYPE { EXTRA = 0, MAIN = 1, SUN=2, LIGHT=4, WATER=3}
    public Volume volume;
    public GTL_TYPE gtlType = GTL_TYPE.EXTRA;
    private float m_weight;
    public float weight
    {
        get { return m_weight; }
        set {
            m_weight = Mathf.Clamp(value, 0f,1f);
            if (volume!=null)
                volume.weight = m_weight;
        }
    }
    public bool isActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volume = GetComponent<Volume>();

        if (!isActive)
        { weight = 0f; }
    }

    public void ChangeVolume(VolumeProfile iVProfile)
    {
        volume.sharedProfile = iVProfile;
        weight = 0f;
    }

    public void CrossfadeSun(GTLModifier iTarget, float iFader)
    {

    }

    public void Deactivate()
    {
        weight = 0f;
        gameObject.SetActive(false);
    }
}
