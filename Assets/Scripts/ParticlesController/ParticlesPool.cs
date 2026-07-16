using System.Collections.Generic;
using UnityEngine;

public class PooledParticleInfo
{
    public ParticleSystem Prefab;
    public List<ParticleSystem> inactiveParticles = new();
}

public class ParticleSystemPoolManager
{
    private static Dictionary<ParticleSystem, PooledParticleInfo> particlePools = new();
    private static Dictionary<ParticleSystem, PooledParticleInfo> instanceToPool = new();

    private static PooledParticleInfo GetOrCreatePool(ParticleSystem prefab)
    {
        if (!particlePools.TryGetValue(prefab, out PooledParticleInfo pool))
        {
            pool = new PooledParticleInfo() { Prefab = prefab };
            particlePools.Add(prefab, pool);
        }
        return pool;
    }

    public static void WarmUp(ParticleSystem prefab, int count)
    {
        PooledParticleInfo pool = GetOrCreatePool(prefab);

        for (int i = 0; i < count; i++)
        {
            ParticleSystem instance = GameManager.SpawnParticles(prefab, Vector3.zero, Quaternion.identity);
            instance.gameObject.SetActive(false);
            pool.inactiveParticles.Add(instance);
            instanceToPool[instance] = pool;
        }
    }

    public static ParticleSystem SpawnObject(ParticleSystem prefab, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        PooledParticleInfo pool = GetOrCreatePool(prefab);

        ParticleSystem instance;
        int lastIndex = pool.inactiveParticles.Count - 1;

        if (lastIndex < 0)
        {
            // no hay inactivos: crea uno nuevo
            instance = GameManager.SpawnParticles(prefab, spawnPosition, spawnRotation);
            instanceToPool[instance] = pool;
        }
        else
        {
            // hay inactivos: reactiva el último
            instance = pool.inactiveParticles[lastIndex];
            pool.inactiveParticles.RemoveAt(lastIndex);

            instance.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            instance.gameObject.SetActive(true);
        }

        instance.Clear();
        instance.Play();

        return instance;
    }

    public static ParticleSystem SpawnObject(ParticleSystem prefab, Transform spawn)
    {
        return SpawnObject(prefab, spawn.position, spawn.rotation);
    }

    public static void ReturnObjectToPool(ParticleSystem instance)
    {
        if (!instanceToPool.TryGetValue(instance, out PooledParticleInfo pool))
        {
            Debug.LogWarning("Se quiere liberar una partícula no pooleada => " + instance.name);
            return;
        }

        instance.gameObject.SetActive(false);
        pool.inactiveParticles.Add(instance);
    }
}