using System;
using UnityEngine.Pool;


public class WaveManager
{
    private ObjectPool<UnityEngine.Object> pool;

    private static Action OnWaveEnd;

    private int waveSize;
    private int currentWave;
    private int waveAmount;

    public WaveManager()
    {
        //GameManager.PreComp += 
    }

    private void SpawnEnemy(UnityEngine.Object enemy)
    {
        for (int i = 0; i < waveSize; i++)
        {
            //pool.Spawn();
        }
    }

    private void EnemySubstract(UnityEngine.Object enemy, float damage)
    {
        if (enemy.TakeDamage(damage) <= 0)
            waveAmount--;

        if (waveAmount <= 0) 
            OnWaveEnd?.Invoke();
    }

}
