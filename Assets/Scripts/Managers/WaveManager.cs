using UnityEngine;
using System.Collections.Generic;

public class WaveManager : IUpdatable
{
    private EnemySO enemySO;
    private Transform target;
    private Rigidbody targetRB;
    private List<Transform> spawnList;

    public Dictionary<UnityEngine.Object, Enemy> waveReferences;

    private Dictionary<int, int> waveSize;
    private int currentAmount = 0;
    private int currentWave = 0;
    private int countCheck = 0;
    private float timePassed = 0f;
    private float spawnInterval = 0.3f;

    public WaveManager(EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        this.enemySO = enemySO;
        this.target = target;
        this.targetRB = targetRB;
        this.spawnList = spawnList;

        waveReferences = new Dictionary<UnityEngine.Object, Enemy>();
        waveSize = new Dictionary<int, int>();
        CustomUpdateManager.Instance.Register(this);

        waveSize.Add(0, 1);
        waveSize.Add(1, 1);
        waveSize.Add(2, 1);

        WarmUpEnemies(enemySO, 3);

        Debug.Log("Entra Wave");
        EventBus.Publish(new OnWaveInit(currentWave, waveSize[currentWave]));

        WaveSet();

        EventBus.Subscribe<UnregisterEntitys>(UnregisterAllCurrentEnemies);
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
                    currentWave++;
                    countCheck = 0;
                    timePassed = 0;
                }
            }
        }
        else
            EventBus.Publish(new WinGameEvent());
    }

    private void SpawnEnemy(EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        UnityEngine.Object catchedRef = ObjectPoolManager.SpawnObject(enemySO.prefab, spawnList[UnityEngine.Random.Range(0, spawnList.Count)]);
        waveReferences.Add(catchedRef, new Enemy(catchedRef, target, targetRB, enemySO));
        currentAmount++;
    }

    private void WarmUpEnemies(EnemySO enemySO, int count)
    {
        ObjectPoolManager.WarmUp(enemySO.prefab, count);
    }

    public void OnEnemyDeadCond(DisableEntityEvent dead)
    {
        ObjectPoolManager.ReturnObjectToPool(dead.objectInstance);

        CustomUpdateManager.Instance.Unregister(waveReferences[dead.objectInstance]);

        waveReferences.Remove(dead.objectInstance);

        currentAmount--;

        EventBus.Publish(new UpdateTextEvent(currentAmount.ToString()));

        if(currentAmount == 0 && currentWave < waveSize.Count) 
            EventBus.Publish(new OnWaveInit(currentWave, waveSize[currentWave]));
    }

    public void UnregisterAllCurrentEnemies(UnregisterEntitys unresEvent)
    {
        if(waveReferences.Count == 0) return;
        foreach(Enemy enemy in waveReferences.Values)
        {
            CustomUpdateManager.Instance.Unregister(enemy);
        }

        CustomUpdateManager.Instance.Unregister(this);
    }
}
