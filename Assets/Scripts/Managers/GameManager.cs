using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region SERIFS
    [SerializeField] public EnemySO enemySO;
    [SerializeField] public Rigidbody targetRB;
    [SerializeField] public Transform targetTransform;
    [SerializeField] public List<Transform> spawnList;
    #endregion

    private WaveManager waveManager;

    #region EVENTS
    public Action OnWinCondition;
    public Action ServicesRegistrations => () => 
    {
        waveManager = new WaveManager(enemySO, targetTransform, targetRB, spawnList);
        ServiceLocator.Register(waveManager.enemies);

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
}
