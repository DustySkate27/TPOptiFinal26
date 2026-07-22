using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PooledObjectInfo
{
    public UnityEngine.Object Prefab;
    public List<GameObject> inactiveObjects = new();
}

public class ObjectPoolManager
{
    private Dictionary<UnityEngine.Object, PooledObjectInfo> ObjectPools = new();

    private Dictionary<GameObject, PooledObjectInfo> instanceToPool = new();

    public ObjectPoolManager() { }

    private PooledObjectInfo GetOrCreatePool(UnityEngine.Object prefab)
    {
        if (!ObjectPools.TryGetValue(prefab, out PooledObjectInfo pool))
        {
            pool = new PooledObjectInfo() { Prefab = prefab };
            ObjectPools.Add(prefab, pool);
        }

        return pool;
    }

    public void WarmUp(UnityEngine.Object prefab, int count)
    {
        PooledObjectInfo pool = GetOrCreatePool(prefab);

        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameManager.CreateEntity(prefab, Vector3.zero, Quaternion.identity).GameObject();
            obj.SetActive(false);
            pool.inactiveObjects.Add(obj);
            instanceToPool[obj] = pool;
        }
    }

    public UnityEngine.Object SpawnObject(UnityEngine.Object objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        PooledObjectInfo pool = GetOrCreatePool(objectToSpawn);

        GameObject spawneableObj;
        int lastIndex = pool.inactiveObjects.Count - 1;

        if (lastIndex < 0)
        {
            spawneableObj = GameManager.CreateEntity(objectToSpawn, spawnPosition, spawnRotation).GameObject();
            instanceToPool[spawneableObj] = pool;
        }
        else
        {
            spawneableObj = pool.inactiveObjects[lastIndex];
            pool.inactiveObjects.RemoveAt(lastIndex);
            spawneableObj.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            spawneableObj.SetActive(true);
        }

        return spawneableObj;
    }

    public UnityEngine.Object SpawnObject(UnityEngine.Object objectToSpawn, Transform spawn)
    {
        return SpawnObject(objectToSpawn, spawn.position, spawn.rotation);
    }

    public void ReturnObjectToPool(UnityEngine.Object obj)
    {
        //if (obj == null) return;

        GameObject go = obj.GameObject();

        if (!instanceToPool.TryGetValue(go, out PooledObjectInfo pool))
        {
            Debug.LogWarning("Se quiere liberar un objeto no pooleado => " + go.name);
            return;
        }

        go.SetActive(false);
        pool.inactiveObjects.Add(go);
    }

    public void DebugPools()
    {
        Debug.Log(ObjectPools.Count);
        Debug.Log(instanceToPool.Count);
    }

    public void ClearPools()
    {
        instanceToPool.Clear();
        ObjectPools.Clear();
    }
}