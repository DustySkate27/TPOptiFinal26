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

    private void Awake()
    {
        waveManager = new WaveManager(enemySO, targetTransform, targetRB, spawnList);

        ServicesRegistrations();

        EventSubscriptions();
    }

    private void ServicesRegistrations()
    {
        ServiceLocator.Register(waveManager.waveReferences);
        ServiceLocator.Register(waveManager.waveSize);
    }

    private void EventSubscriptions()
    {
        EventBus.Subscribe<WinGameEvent>(OnWinCond);
        EventBus.Subscribe<LoseGameEvent>(OnLoseCond);
        EventBus.Subscribe<DeadEnemyEvent>(waveManager.OnEnemyDeadCond);
    }
    private void EventUnsubscriptions()
    {
        EventBus.Unsubscribe<WinGameEvent>(OnWinCond);
        EventBus.Unsubscribe<LoseGameEvent>(OnLoseCond);
    }

    public void OnWinCond(WinGameEvent winEvent)
    {
        Time.timeScale = 0f;
        Debug.Log("You Win!!");
        EventUnsubscriptions();
    }

    public void OnLoseCond(LoseGameEvent loseEvent)
    {
        Time.timeScale = 0f;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("You Lose");
        EventUnsubscriptions();
    }

    public void ResetScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());

        EventSubscriptions();
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
