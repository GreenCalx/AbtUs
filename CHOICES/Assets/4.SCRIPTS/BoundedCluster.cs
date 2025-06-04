using UnityEngine;
using System.Collections.Generic;
public class BoundedCluster : MonoBehaviour
{
    [Header("Settables")]
    public List<GameObject> members = new List<GameObject>();

    [Header("Internals")]
    public Bounds bounds;

    public void Encapsulate(GameObject iGO)
    {
        // update members
        if (members.Contains(iGO))
            return;

        // update bounds
        MeshRenderer MR = iGO.GetComponent<MeshRenderer>();
        if (!!MR)
        {
            if (members.Count == 0)
            { bounds = MR.bounds; }
            else
            { bounds.Encapsulate(MR.bounds); }
        }

        members.Add(iGO);
    }

    public void RefreshBounds()
    {
        bounds = new Bounds();
        foreach (GameObject go in members) { bounds.Encapsulate(go.transform.localPosition); }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(
                bounds.center, bounds.size
             );
    }

    public void ApplyMargin(float iMargin) { bounds.Expand(iMargin); }

    public void Remove(GameObject iObject)
    {
        if (!members.Contains(iObject))
            return;
        members.Remove(iObject);
        RefreshBounds();
    }
}

