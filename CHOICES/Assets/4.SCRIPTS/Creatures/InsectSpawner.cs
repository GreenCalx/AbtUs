using UnityEngine;

public class InsectSpawner : CreatureSpawner<InsectBehaviour>
{
    protected override void NotifyBSTPool(InsectBehaviour iSpawned)
    {
        agentPool.SubscribeInsect(iSpawned);
    }
}