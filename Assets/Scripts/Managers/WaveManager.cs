using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Net;

public class WaveManager : IUpdatable
{
    private ObjectPoolManager poolManager;
    private EnemySO enemySO;
    private Transform target;
    private Rigidbody targetRB;
    private List<Transform> spawnList;

    public Dictionary<UnityEngine.Object, Enemy> waveReferences;

    private Dictionary<int, int> waveSize;
    private int currentAmount = 0;
    private int enemiesKilled = 0;
    private int currentWave = 0;
    private int countCheck = 0;
    private float timePassed = 0f;
    private float spawnInterval = 0.3f;

    public WaveManager(ObjectPoolManager objPoolMan, EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        poolManager = objPoolMan;
        this.enemySO = enemySO;
        this.target = target;
        this.targetRB = targetRB;
        this.spawnList = spawnList;

        waveReferences = new Dictionary<UnityEngine.Object, Enemy>();
        waveSize = new Dictionary<int, int>();
        CustomUpdateManager.Instance.Register(this);

        waveSize.Add(0, 10);
        waveSize.Add(1,20);
        waveSize.Add(2, 30);

        WarmUpEnemies(enemySO, 30);

        WaveSet();
        EventBus.Publish(new UpdateTextEvent(enemiesKilled.ToString(), waveSize[currentWave].ToString()));
        EventBus.Publish(new OnWaveInit(currentWave, waveSize[currentWave]));

        EventBus.Subscribe<UnregisterEntities>(UnregisterAllCurrentEnemies);
    }

    public void Tick(float deltaTime)
    {
        if (currentAmount == 0 || countCheck != 0)
            WaveSet();
    }

    private void WaveSet()
    {
        timePassed += Time.deltaTime;
        if (currentWave < waveSize.Count)
        {
            if (timePassed > spawnInterval)
            {
                if (countCheck < waveSize[currentWave])
                {
                    SpawnEnemy(enemySO, target, targetRB, spawnList);
                    countCheck++;
                    timePassed = 0;
                }
                else
                {
                    countCheck = 0;
                    timePassed = 0;
                }
            }
        }
    }

    private void SpawnEnemy(EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        UnityEngine.Object catchedRef = poolManager.SpawnObject(enemySO.prefab, spawnList[UnityEngine.Random.Range(0, spawnList.Count)]);
        waveReferences.Add(catchedRef, new Enemy(catchedRef, target, targetRB, enemySO));
        currentAmount++;
    }

    private void WarmUpEnemies(EnemySO enemySO, int count)
    {
        poolManager.WarmUp(enemySO.prefab, count);
    }

    public void OnEnemyDeadCond(DisableEntityEvent dead)
    {
        poolManager.ReturnObjectToPool(dead.objectInstance);

        CustomUpdateManager.Instance.Unregister(waveReferences[dead.objectInstance]);

        waveReferences.Remove(dead.objectInstance);

        currentAmount--;
        enemiesKilled++;

        if(currentAmount == 0)
        {
            currentWave++;
            if(currentWave < waveSize.Count)
            {
                enemiesKilled = 0;
                EventBus.Publish(new OnWaveInit(currentWave, waveSize[currentWave]));
            }
            else
            {
                EventBus.Publish(new EndGameEvent(1));
                return;
            }
        }
        EventBus.Publish(new UpdateTextEvent(enemiesKilled.ToString(), waveSize[currentWave].ToString()));
    }

    public void UnregisterAllCurrentEnemies(UnregisterEntities unresEvent)
    {
        CustomUpdateManager.Instance.Unregister(this);

        if(waveReferences.Count == 0) return;
        foreach(Enemy enemy in waveReferences.Values)
        {
            CustomUpdateManager.Instance.Unregister(enemy);
        }
    }
}
