using UnityEngine;

public static class Utils
{
    public static float Lerp(float a, float b, float f)
    {
        return a * (1f - f) + (b * f);
    }
}
