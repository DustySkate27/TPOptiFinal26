using System;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using Unity.Mathematics;
using static UnityEngine.EventSystems.EventTrigger;


public class WaveManager
{
    public Dictionary<UnityEngine.Object, Enemy> waveDict;

    private static Action OnWaveEnd;

    private int waveSize;
    private int currentWave;
    private int waveAmount;

    public WaveManager(EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        waveDict = new Dictionary<UnityEngine.Object, Enemy>();
        SpawnEnemy(enemySO, target, targetRB, spawnList);
    }

    private void SpawnEnemy(EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        UnityEngine.Object catchedRef = ObjectPoolManager.SpawnObject(enemySO.prefab, spawnList[UnityEngine.Random.Range(0, spawnList.Count)]);
        Enemy enemy = new Enemy(catchedRef.GameObject(), target, targetRB, enemySO);
        waveDict.Add(catchedRef, enemy);
    }

    private void EnemySubstract(Enemy enemy, float damage)
    {
        if (waveAmount <= 0) 
            OnWaveEnd?.Invoke();
    }

}
