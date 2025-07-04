using UnityEngine;
using TMPro;
public class FPSCounter : MonoBehaviour
{
    TextMeshProUGUI txt;
    float elapsed = 0f;
    int frameCount = 0;
    public float refreshRate = 0.5f;

    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        frameCount++;
        elapsed += Time.unscaledDeltaTime;
        if (elapsed >= refreshRate)
        {
            txt.text = (Mathf.Round(frameCount / elapsed)).ToString();
            frameCount = 0;
            elapsed = 0f;
        }
    }
}
