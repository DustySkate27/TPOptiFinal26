using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    #region SERIFS
    public EnemySO enemySO;
    public Rigidbody targetRB;
    public Transform targetTransform;
    public List<Transform> spawnList;
    #endregion

    private WaveManager waveManager;

    private void Awake()
    {
        ServicesRegistrations();

        //EventBus.Subscribe<WinGameEvent>(WinCond);
        EventBus.Subscribe<LoseGameEvent>(LoadScene);
    }

    private void OnDestroy()
    {
        //EventBus.Unsubscribe<WinGameEvent>(WinCond);
        EventBus.Unsubscribe<LoseGameEvent>(LoadScene);
    }

    private void ServicesRegistrations()
    {
        waveManager = new WaveManager(enemySO, targetTransform, targetRB, spawnList);
        ServiceLocator.Register(waveManager.waveReferences);
        ServiceLocator.Register(waveManager.waveSize);
    }
    public static void WinCond()
    {
        Time.timeScale = 0f;
        Debug.Log("You Win!!");
    }

    public void LoadScene(LoseGameEvent winEvent)
    {
        //SceneManager.LoadScene(name);
        Time.timeScale = 0f;
        Debug.Log("You Lose");
    }

    public void ResetScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
    }

    public static UnityEngine.Object CreateEntity(UnityEngine.Object entity, Transform transform)
    {
        return Instantiate(entity, transform);
    }

    public static UnityEngine.Object CreateEntity(UnityEngine.Object entity, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        return Instantiate(entity, spawnPosition, spawnRotation);
    }
}
