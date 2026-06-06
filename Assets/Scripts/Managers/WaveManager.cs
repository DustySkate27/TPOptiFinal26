using System;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using Unity.Mathematics;
using static UnityEngine.EventSystems.EventTrigger;

public class WaveManager : IUpdatable
{
    private EnemySO enemySO;
    private Transform target;
    private Rigidbody targetRB;
    private List<Transform> spawnList;

    public Dictionary<UnityEngine.Object, Enemy> waveReferences;

    public Dictionary<int, (int baseAmount,int currentAmount)> waveSize;
    private int currentWave = 0;

    public WaveManager(EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        this.enemySO = enemySO;
        this.target = target;
        this.targetRB = targetRB;
        this.spawnList = spawnList;

        waveReferences = new Dictionary<UnityEngine.Object, Enemy>();
        waveSize = new Dictionary<int, (int baseAmount,int currentAmount)>();

        waveSize.Add(0, (3, 3));
        waveSize.Add(1, (7, 7));
        waveSize.Add(2, (15, 15));
    }

    public void Tick(float deltaTime)
    {
        if (waveSize[currentWave].currentAmount == 0)
            WaveSet();
    }
    
    private void SpawnEnemy(EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        UnityEngine.Object catchedRef = ObjectPoolManager.SpawnObject(enemySO.prefab, spawnList[UnityEngine.Random.Range(0, spawnList.Count)]);
        Enemy enemy = new Enemy(catchedRef.GameObject(), target, targetRB, enemySO);
        waveReferences.Add(catchedRef, enemy);
    }

    private void WaveSet()
    {
        if (currentWave < waveSize.Count - 1)
        {
            for (int i = 0; i < waveSize[currentWave].baseAmount; i++)
                SpawnEnemy(enemySO, target, targetRB, spawnList);
        }

        else
            GameManager.WinCond();
    }

}
