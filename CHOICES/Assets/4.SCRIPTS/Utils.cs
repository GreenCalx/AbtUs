using UnityEngine;

public static class Utils
{
    public static float Lerp(float a, float b, float f)
    {
        return a * (1f - f) + (b * f);
    }

    public static float Remap(float iVal, float iOldMin, float iOldMax, float iNewMin, float iNewMax)
    {
        return iNewMin + (iVal/(iOldMax - iOldMin))*(iNewMax-iNewMin);
    }
    
    public static bool IsNaN(Vector3 iVec)
    {
        return (float.IsNaN(iVec.x) || float.IsNaN(iVec.y) || float.IsNaN(iVec.z));
    }
}
