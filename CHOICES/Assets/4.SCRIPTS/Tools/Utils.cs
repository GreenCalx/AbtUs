using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class IntEvent : UnityEvent<int> {}

public static class Utils
{
    public static Vector2 GetMipsSize(RenderTexture iRT, int iMipsLevel, bool iForcePow2)
    {

        if (!iRT.useMipMap)
            return Vector2.zero;

        float width = iRT.width;
        float height = iRT.height;
        if (width == height)
        {
            int mipsize = (int)Mathf.Pow(2, iMipsLevel);
            return new Vector2(mipsize, mipsize);
        }

        float mip_w = width;
        float mip_h = height;
        int i = 0;
        while (i <= iMipsLevel)
        {
            mip_w /= 2f;
            mip_h /= 2f;
            i++;
        }
        if (iForcePow2)
        {
            float pow2_w = 1;
            float pow2_h = 1;
            while (pow2_w < mip_h) { pow2_w *= 2; }
            while (pow2_h < mip_h) { pow2_h *= 2; }

            mip_w = pow2_w;
            mip_h = pow2_h;
        }
        return new Vector2(mip_w, mip_h);
    }
    // Returns f(iX) of cauchy PDF
    public static float CauchySample(float iX0, float iQ, float iXSample)
    {
        float f_den = Mathf.PI * iQ * (1 + Mathf.Pow((iXSample - iX0) / iQ, 2));
        return 1f / f_den;
    }
    public static void CauchyToAnimCurve(ref AnimationCurve ioCurve, float iX0, float iQ)
    {
        ioCurve.ClearKeys();

        int n_steps = 10;
        for (int i = 0; i <= n_steps; i++)
        {
            float x_curve = (float)i / (float)n_steps;
            float y_curve = CauchySample(iX0, iQ, x_curve);

            int key_idx = ioCurve.AddKey(x_curve, y_curve);
            ioCurve.SmoothTangents(key_idx, 0f);
        }
    }
    public static T GetComp<T>(GameObject iGO)
    {
        T comp = iGO.GetComponent<T>();
        if (comp != null)
            return comp;
        return iGO.GetComponentInParent<T>();
    }

    public static T GetCompInParent<T>(GameObject iGO)
    {
        T ret = iGO.GetComponentInParent<T>();
        if (ret != null)
            return ret;
        Transform parent = iGO.transform.parent;
        while (parent != null)
        {
            ret = parent.gameObject.GetComponent<T>();
            if (ret != null)
                break;
            parent = parent.parent;
        }

        return ret;
    }

    public static float Lerp(float a, float b, float f)
    {
        return a * (1f - f) + (b * f);
    }

    public static float Remap(float iVal, float iOldMin, float iOldMax, float iNewMin, float iNewMax)
    {
        return iNewMin + (iVal / (iOldMax - iOldMin)) * (iNewMax - iNewMin);
    }

    public static bool IsNaN(Vector3 iVec)
    {
        return (float.IsNaN(iVec.x) || float.IsNaN(iVec.y) || float.IsNaN(iVec.z));
    }

    public static void Split<T>(T[] iSourceArr, int index, out T[] left, out T[] right)
    {
        int rlen = iSourceArr.Length - index;
        left = new T[index];
        right = new T[rlen];

        Array.Copy(iSourceArr, 0, left, 0, index);
        Array.Copy(iSourceArr, index, right, 0, rlen);
    }
}
