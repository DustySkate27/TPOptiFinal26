using UnityEngine;
using System.Collections.Generic;

public class ParticleManager
{
    public Dictionary<UnityEngine.Object, ParticleBehaviour> particleReferences;

    private Object particle;

    private int defaultSize = 10;

    ObjectPoolManager poolManager;

    public ParticleManager(GameObject prefab, ObjectPoolManager objPoolMan)
    {
        poolManager = objPoolMan;
        particleReferences = new Dictionary<UnityEngine.Object, ParticleBehaviour>();

        particle = prefab;

        WarmUpParticles();

        EventBus.Subscribe<UnregisterEntities>(UnregisterAllCurrentEnemies);
    }

    private void WarmUpParticles()
    {
        poolManager.WarmUp(particle, defaultSize);
    }

    public void SpawnParticle(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        UnityEngine.Object catchedRef = poolManager.SpawnObject(particle, spawnPosition, spawnRotation);
        particleReferences.Add(catchedRef, new ParticleBehaviour(catchedRef));
    }

    public void OnParticleEnd(OnParticleEndEvent end)
    {
        poolManager.ReturnObjectToPool(end.objectInstance);

        CustomUpdateManager.Instance.Unregister(particleReferences[end.objectInstance]);

        particleReferences.Remove(end.objectInstance);
    }

    public void UnregisterAllCurrentEnemies(UnregisterEntities unresEvent)
    {
        if (particleReferences.Count == 0) return;

        foreach (ParticleBehaviour particle in particleReferences.Values)
        {
            CustomUpdateManager.Instance.Unregister(particle);
        }
    }
}

