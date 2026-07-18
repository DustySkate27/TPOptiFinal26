using UnityEngine;
using System.Collections.Generic;

public class ParticleManager
{
    public Dictionary<UnityEngine.Object, ParticleBehaviour> particleReferences;

    private Object particle;

    private int defaultSize = 10;

    public ParticleManager(GameObject prefab)
    {
        particleReferences = new Dictionary<UnityEngine.Object, ParticleBehaviour>();

        particle = prefab;

        WarmUpParticles();

        EventBus.Subscribe<UnregisterEntitys>(UnregisterAllCurrentEnemies);
    }

    private void WarmUpParticles()
    {
        ObjectPoolManager.WarmUp(particle, defaultSize);
    }

    public void SpawnParticle(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        UnityEngine.Object catchedRef = ObjectPoolManager.SpawnObject(particle, spawnPosition, spawnRotation);
        particleReferences.Add(catchedRef, new ParticleBehaviour(catchedRef));
    }

    public void OnParticleEnd(OnParticleEndEvent end)
    {
        ObjectPoolManager.ReturnObjectToPool(end.objectInstance);

        CustomUpdateManager.Instance.Unregister(particleReferences[end.objectInstance]);

        particleReferences.Remove(end.objectInstance);
    }

    public void UnregisterAllCurrentEnemies(UnregisterEntitys unresEvent)
    {
        if (particleReferences.Count == 0) return;

        foreach (ParticleBehaviour enemy in particleReferences.Values)
        {
            CustomUpdateManager.Instance.Unregister(enemy);
        }
    }
}

