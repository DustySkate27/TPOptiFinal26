using System;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Diagnostics.Tracing;


public class WaveManager
{
    private ObjectPool<Enemy> pool;

    private static Action OnWaveEnd;

    private int waveSize;
    private int currentWave;
    private int waveAmount;

    Dictionary<GameObject, Enemy> enemyList;

    public WaveManager()
    {
        //GameManager.PreComp += 
    }

    private void SpawnEnemy(Enemy enemy)
    {
        for (int i = 0; i < waveSize; i++)
        {
            //pool.Initialize();
        }
    }

    private void EnemySubstract(Enemy enemy, float damage)
    {
        if (enemy.TakeDamage(damage) <= 0)
            waveAmount--;

        if (waveAmount <= 0) 
            OnWaveEnd?.Invoke();
    }

}
