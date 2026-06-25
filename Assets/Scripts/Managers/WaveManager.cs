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

    public Dictionary<int, int> waveSize;
    private int currentAmount = 0;
    private int currentWave = 0;

    public WaveManager(EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        this.enemySO = enemySO;
        this.target = target;
        this.targetRB = targetRB;
        this.spawnList = spawnList;

        waveReferences = new Dictionary<UnityEngine.Object, Enemy>();
        waveSize = new Dictionary<int, int>();
        CustomUpdateManager.Instance.Register(this);

        waveSize.Add(0, 3);
        waveSize.Add(1, 7);
        waveSize.Add(2, 15);
        WaveSet();
    }

    public void Tick(float deltaTime)
    {
        if (currentAmount == 0)
            WaveSet();
    }

    private void WaveSet()
    {
        if (currentWave < waveSize.Count - 1)
        {
            for (int i = 0; i < waveSize[currentWave]; i++)
                SpawnEnemy(enemySO, target, targetRB, spawnList);

            currentAmount = waveSize[currentWave];
            currentWave++;

        }

        else
            EventBus.Publish(new WinGameEvent());
    }
    
    private void SpawnEnemy(EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        UnityEngine.Object catchedRef = ObjectPoolManager.SpawnObject(enemySO.prefab, spawnList[UnityEngine.Random.Range(0, spawnList.Count)]);
        waveReferences.Add(catchedRef, new Enemy(catchedRef, target, targetRB, enemySO));
        
    }

    public void OnEnemyDeadCond(DeadEnemyEvent dead)
    {
        ObjectPoolManager.ReturnObjectToPool(dead.objectInstance);

        CustomUpdateManager.Instance.Unregister(waveReferences[dead.objectInstance]);

        waveReferences.Remove(dead.objectInstance);

        currentAmount--;
    }
}
