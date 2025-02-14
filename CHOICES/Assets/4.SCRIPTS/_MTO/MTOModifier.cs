using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTOModifier : MonoBehaviour
{
    public MeshRenderer MR;
    public List<Material> currMats;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (MR==null)
            MR = GetComponent<MeshRenderer>();

        OverWorldControl.Instance.SubscribeMTO(this);
        currMats = new List<Material>(MR.materials);
    }

    public void ChangeMaterial(Material iOldMat, Material iNewMat)
    {
        int idx = currMats.IndexOf(iOldMat);
        currMats[idx] = iNewMat;
    }

    public void RefreshMaterials()
    {
        MR.SetMaterials(currMats);
    }
}
