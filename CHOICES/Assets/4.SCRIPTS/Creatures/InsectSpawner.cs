using UnityEngine;

public class InsectSpawner : CreatureSpawner<InsectBehaviour>
{
    protected override void NotifyBSTPoolSpawn(InsectBehaviour iSpawned)
    {
        agentPool.SubscribeInsect(iSpawned);
    }
    protected override void NotifyBSTPoolDeSpawn(InsectBehaviour iToDespawn)
    {
        agentPool.UnSubscribeInsect(iToDespawn);
    }
}