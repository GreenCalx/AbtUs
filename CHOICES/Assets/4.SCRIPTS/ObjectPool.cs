using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private List<GameObject> pool = new List<GameObject>();
    public Bounds bounds;

    public void Encapsulate(GameObject iGO)
    {
        // update pool
        if (pool.Contains(iGO))
            return;

        // update bounds
        MeshRenderer MR = iGO.GetComponent<MeshRenderer>();
        if (!!MR)
        {
            if (pool.Count==0)
            { bounds = MR.bounds; }
            else
            { bounds.Encapsulate(MR.bounds); }
        }
        
        pool.Add(iGO);
    }

    public void ApplyMargin(float iMargin) { bounds.Expand(iMargin); }

    public void Remove(GameObject iObject)
    {
        if (!pool.Contains(iObject))
            return;
        pool.Remove(iObject);
        RefreshBounds();
    }

    public void RefreshBounds()
    {
        bounds = new Bounds();
        foreach(GameObject go in pool) { bounds.Encapsulate(go.transform.localPosition); }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(
                bounds.center, bounds.size
             );
    }
}
