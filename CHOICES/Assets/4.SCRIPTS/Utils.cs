using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class IntEvent : UnityEvent<int> {}

public static class Utils
{
    public static T GetComp<T>(GameObject iGO)
    {
        T comp = iGO.GetComponent<T>();
        if (comp!=null)
            return comp;
        return iGO.GetComponentInParent<T>();
    }
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
