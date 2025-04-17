using UnityEngine;

public class DreamCatcherSpawner : CreatureSpawner<DreamCatcherBehaviour>
{
    protected override void NotifyBSTPoolSpawn(DreamCatcherBehaviour iSpawned)
    {
        agentPool.SubscribeDreamCatcher(iSpawned);
    }
    protected override void NotifyBSTPoolDeSpawn(DreamCatcherBehaviour iToDespawn)
    {
        agentPool.UnSubscribeDreamCatcher(iToDespawn);
    }
}