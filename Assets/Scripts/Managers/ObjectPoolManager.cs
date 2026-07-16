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
    // Pools indexadas por prefab (referencia), no por nombre.
    private static Dictionary<UnityEngine.Object, PooledObjectInfo> ObjectPools = new();

    // Mapea cada instancia activa a la pool de la que vino, para poder
    // devolverla sin necesidad de comparar nombres ni agregar componentes.
    private static Dictionary<GameObject, PooledObjectInfo> instanceToPool = new();

    private static PooledObjectInfo GetOrCreatePool(UnityEngine.Object prefab)
    {
        if (!ObjectPools.TryGetValue(prefab, out PooledObjectInfo pool))
        {
            pool = new PooledObjectInfo() { Prefab = prefab };
            ObjectPools.Add(prefab, pool);
        }
        return pool;
    }

    // Precrea "count" instancias desactivadas y las deja cargadas en la pool.
    public static void WarmUp(UnityEngine.Object prefab, int count)
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

    public static UnityEngine.Object SpawnObject(UnityEngine.Object objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        PooledObjectInfo pool = GetOrCreatePool(objectToSpawn);

        GameObject spawneableObj;
        int lastIndex = pool.inactiveObjects.Count - 1;

        if (lastIndex < 0)
        {
            // no hay inactivos: crea uno nuevo
            spawneableObj = GameManager.CreateEntity(objectToSpawn, spawnPosition, spawnRotation).GameObject();
            instanceToPool[spawneableObj] = pool;
        }
        else
        {
            // hay inactivos: reactiva el último (evita el costo de List.Remove por índice)
            spawneableObj = pool.inactiveObjects[lastIndex];
            pool.inactiveObjects.RemoveAt(lastIndex);

            spawneableObj.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            spawneableObj.SetActive(true);
        }

        return spawneableObj;
    }

    public static UnityEngine.Object SpawnObject(UnityEngine.Object objectToSpawn, Transform spawn)
    {
        return SpawnObject(objectToSpawn, spawn.position, spawn.rotation);
    }

    public static void ReturnObjectToPool(UnityEngine.Object obj)
    {
        GameObject go = obj.GameObject();

        if (!instanceToPool.TryGetValue(go, out PooledObjectInfo pool))
        {
            Debug.LogWarning("Se quiere liberar un objeto no pooleado => " + go.name);
            return;
        }

        go.SetActive(false);
        pool.inactiveObjects.Add(go);
    }
}