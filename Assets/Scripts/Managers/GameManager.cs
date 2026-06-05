using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region SERIFS
    public EnemySO enemySO;
    public Rigidbody targetRB;
    public Transform targetTransform;
    public List<Transform> spawnList;
    #endregion

    private WaveManager waveManager;

    #region EVENTS
    public Action OnWinCondition;
    public Action ServicesRegistrations => () => 
    {
        waveManager = new WaveManager(enemySO, targetTransform, targetRB, spawnList);
        ServiceLocator.Register(waveManager.waveDict);

    }; //Todo lo que necesite PreComp en pantalla de carga
    #endregion

    private void Awake()
    {
        ServicesRegistrations?.Invoke();

        OnWinCondition += StopTime;
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ResetScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
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
