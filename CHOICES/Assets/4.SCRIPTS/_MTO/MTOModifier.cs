using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventLog;
/*

    >> TODO : Implement Lerp in new shader
    we might need a proxy yo handle shader features such as Chaos
    and material swapping
    might also need to limit new shader to LOD0 and have this current sytem for LOD1 & above

*/

public class MTOModifier : OWCModifier
{
    public Renderer MR;
    public GFXWrapper shaderCom;
    public float lerpTime = 10f;

    void Start()
    {
        if (MR == null)
            MR = GetComponent<Renderer>();
        OverWorldControl.Instance.SubscribeMTO(this);

        shaderCom.InitShader();
        Managers.Instance.ObjectPools.AddObject(this);

        ResetMod();
    }

    void Destroy()
    {
        Managers.Instance.ObjectPools.RemoveObject(this);
    }

    public int GetMatSlotCount()
    {
        return shaderCom.targetMats.Count;
    }

    public void ChangeMaterials(List<MatDefSO> iNewMats)
    {
        shaderCom.ChangeMatText(iNewMats, lerpTime);
    }

    public void UpdateChaos(float iValue)
    {
        shaderCom.SetChaos(iValue);
    }

    public bool IsAvailable()
    {
        return shaderCom.IsAvailable();
    }

    public void ResetMaterials()
    {
    }

    void Update()
    {

    }

    private void ResetMod()
    {
        // targetMats = new List<Material>(currMats);
        // lerp_done = true;
    }

    public override void OnPoolSleep()
    {
        INFO("MTOModifier::OnPoolSleep : " + gameObject.name);
        shaderCom.Sleep(true);
    }

    public override void OnPoolAwake()
    {
        INFO("MTOModifier::OnPoolAwake : " + gameObject.name);
        shaderCom.Sleep(false);
    }
}
