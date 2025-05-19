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

    // protected override void UpdateSpawnFeedback()
    // {
    //     if (spawnFeedback == null)
    //         return;
        
    //     float fValue = (float)spawnedCreatures.Count /  (float)MAX_SPAWNS;
    //     spawnFeedback.Influence = fValue;
    //     spawnFeedback.use();
    // }
    
    public override float feedbackEvaluator()
    {
        return (float)spawnedCreatures.Count /  (float)MAX_SPAWNS;
    }

    public override void InitFromFeedbackFunc(float iFeedbackInfluence)
    {

    }
}