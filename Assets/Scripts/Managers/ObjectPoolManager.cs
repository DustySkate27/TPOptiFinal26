using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPoolManager
{
    public static List<PooledObjectInfo> ObjectPools = new();

    public static UnityEngine.Object SpawnObject(UnityEngine.Object objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

        // si la pool no existe se crea 
        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }

        //chequea si hay objectos inactivos en la pool
        GameObject spawneableObj = pool.inactiveObjects.FirstOrDefault();

        if (spawneableObj == null)
        {
            //si no hay inactivos crea uno
            spawneableObj = GameManager.CreateEntity(objectToSpawn, spawnPosition, spawnRotation).GameObject();
        }
        else
        {
            //si hay inactivos los reactiva
            spawneableObj.transform.position = spawnPosition;
            spawneableObj.transform.rotation = spawnRotation;
            pool.inactiveObjects.Remove(spawneableObj);
            spawneableObj.SetActive(true);
        }

        return spawneableObj;
    }

    public static UnityEngine.Object SpawnObject(UnityEngine.Object objectToSpawn, Transform spawn)
    {

        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

        // si la pool no existe se crea 
        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }

        //chequea si hay objectos inactivos en la pool
        GameObject spawneableObj = pool.inactiveObjects.FirstOrDefault();

        if (spawneableObj == null)
        {
            //si no hay inactivos crea uno
            spawneableObj = GameManager.CreateEntity(objectToSpawn, spawn).GameObject();
        }
        else
        {
            //si hay inactivos los reactiva
            spawneableObj.transform.position = spawn.position;
            spawneableObj.transform.rotation = spawn.rotation;
            pool.inactiveObjects.Remove(spawneableObj);
            spawneableObj.SetActive(true);
        }


        return spawneableObj;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        string goName = obj.name.Replace("(Clone)", string.Empty);

        PooledObjectInfo pool = ObjectPools.Find(pool => pool.LookupString == goName);

        if (pool == null)
        {
            //Debug.LogWarning("quiere liberar obj no pooleado => " + obj.name);
        }
        else
        {
            obj.SetActive(false);
            pool.inactiveObjects.Add(obj);
        }
    }
}
public class PooledObjectInfo
{
    public string LookupString;
    public List<GameObject> inactiveObjects = new List<GameObject>();
}