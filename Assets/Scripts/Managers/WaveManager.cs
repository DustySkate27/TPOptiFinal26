using System;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using Unity.Mathematics;


public class WaveManager
{
    private ObjectPool<Enemy> pool;

    public Dictionary<UnityEngine.Object, Enemy> enemies;

    private static Action OnWaveEnd;

    private int waveSize = 4;
    private int currentWave;
    private int waveAmount;

    public WaveManager(EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        Debug.Log(enemySO);
        Debug.Log(target);
        Debug.Log(targetRB);
        Debug.Log(spawnList);

        enemies = new Dictionary<UnityEngine.Object, Enemy>();
        SpawnEnemy(enemySO, target, targetRB, spawnList);
    }

    private void SpawnEnemy(EnemySO enemySO, Transform target, Rigidbody targetRB, List<Transform> spawnList)
    {
        for (int i = 0; i < waveSize; i++)
        {
            UnityEngine.Object enemyObj = GameManager.CreateEntity(enemySO.prefab, spawnList[UnityEngine.Random.Range(0, spawnList.Count)]);
            Enemy enemy = new Enemy(enemyObj, target, targetRB, enemySO);
            enemies.Add(enemyObj, enemy);
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
