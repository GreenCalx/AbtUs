using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

    >> TODO : Implement Lerp in new shader
    we might need a proxy yo handle shader features such as Chaos
    and material swapping
    might also need to limit new shader to LOD0 and have this current sytem for LOD1 & above

*/

public class MTOModifier : MonoBehaviour
{
    public MeshRenderer MR;
    public List<Material> currMats;
    public List<Material> targetMats;
    private List<Material> initMats;

    public bool lerp_done = false;
    public float lerpTime = 10f;
    private float elapsedLerp = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (MR==null)
            MR = GetComponent<MeshRenderer>();
        OverWorldControl.Instance.SubscribeMTO(this);
        initMats = new List<Material>(MR.materials);;
        currMats = initMats;

        ResetMod();
    }

    public void ChangeMaterial(Material iOldMat, Material iNewMat)
    {
        int idx = currMats.IndexOf(iOldMat);
        targetMats[idx] = iNewMat;
    }

    public void RefreshMaterials()
    {
        elapsedLerp = 0f;
        lerp_done = false;
    }

    public void ResetMaterials()
    {
        MR.SetMaterials(initMats);
    }

    void Update()
    {
        if (lerp_done)
            return;
        
        for(int i=0; i<targetMats.Count; i++)
        {
            float frac = lerpTime / elapsedLerp;
            MR.materials[i].Lerp(currMats[i], targetMats[i], frac);
        }
        elapsedLerp += Time.deltaTime;
        if (elapsedLerp >= lerpTime)
        {
            
            MR.SetMaterials(targetMats);
            currMats = new List<Material>(MR.materials);

            ResetMod();
        }
    }

    private void ResetMod()
    {
        targetMats = new List<Material>(currMats);
        lerp_done = true;
    }
}
