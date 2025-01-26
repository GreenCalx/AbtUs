using UnityEngine;
using UnityEngine.UI;

public class UIOverWorldControl : MonoBehaviour
{
    public Slider slider_GtL;
    public Slider slider_MtO;
    public Slider slider_OtC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateControlSliders(float iGtL, float iMtO, float iOtC)
    {
        slider_GtL.value = iGtL;
        slider_MtO.value = iMtO;
        slider_OtC.value = iOtC;
    }
}
