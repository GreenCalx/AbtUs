using UnityEngine;
using UnityEngine.Rendering;

public class GTLModifier : MonoBehaviour
{
    public enum VOL_TYPE { EXTRA = 0, MAIN = 1}
    public Volume volume;
    public VOL_TYPE volumeType = VOL_TYPE.EXTRA;
    private float m_weight;
    public float weight
    {
        get { return m_weight; }
        set {
            m_weight = Mathf.Clamp(value, 0f,1f);
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

    public void Deactivate()
    {
        weight = 0f;
        gameObject.SetActive(false);
    }
}
