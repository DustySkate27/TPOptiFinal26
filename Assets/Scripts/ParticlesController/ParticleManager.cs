using UnityEngine;
using System.Collections.Generic;

public class ParticleManager
{
    public Dictionary<UnityEngine.Object, ParticleBehaviour> particleReferences;

    private Stack<ParticleBehaviour> particleWrapperPool;

    private Object particle;

    private int defaultSize = 10;

    ObjectPoolManager poolManager;

    public ParticleManager(GameObject prefab, ObjectPoolManager objPoolMan)
    {
        poolManager = objPoolMan;
        particleReferences = new Dictionary<UnityEngine.Object, ParticleBehaviour>(defaultSize);
        particleWrapperPool = new Stack<ParticleBehaviour>(defaultSize);

        particle = prefab;

        WarmUpParticles();

        EventBus.Subscribe<UnregisterEntities>(UnregisterAllCurrentEnemies);
    }

    private void WarmUpParticles()
    {
        poolManager.WarmUp(particle, defaultSize);

        for (int i = 0; i < defaultSize; i++)
            particleWrapperPool.Push(new ParticleBehaviour());
    }

    public void SpawnParticle(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        UnityEngine.Object catchedRef = poolManager.SpawnObject(particle, spawnPosition, spawnRotation);

        ParticleBehaviour behaviour = GetOrCreateParticleWrapper();
        behaviour.Init(catchedRef);

        particleReferences.Add(catchedRef, behaviour);
    }

    private ParticleBehaviour GetOrCreateParticleWrapper()
    {
        if (particleWrapperPool.Count > 0)
            return particleWrapperPool.Pop();

        return new ParticleBehaviour(); // fallback de emergencia, no debería dispararse tras el warm-up
    }

    public void OnParticleEnd(OnParticleEndEvent end)
    {
        poolManager.ReturnObjectToPool(end.objectInstance);

        ParticleBehaviour behaviour = particleReferences[end.objectInstance];
        behaviour.OnReturnedToPool();
        particleWrapperPool.Push(behaviour);

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