using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;

public class GameManager : MonoBehaviour
{
    private GameManager gameManage;

    #region SERIFS
    public EnemySO enemySO;
    public PlayerSO playerSO;
    public Camera playerCamera;
    public List<Transform> spawnList;

    public ParticleSystem shootParticles;
    #endregion

    private PlayerBrain playerBrain;
    private WaveManager waveManager;
    private ParticlesController particlesController;

    public int targetFrameRate = 60;
    private void Awake()
    {
        gameManage = this;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;

        Rigidbody playerInstanceRef = CreatePlayer(playerSO);

        playerBrain = new PlayerBrain(playerInstanceRef.transform, playerInstanceRef, playerCamera, playerSO.shootRange, playerSO.enemyLayer);

        waveManager = new WaveManager(enemySO, playerInstanceRef.transform, playerInstanceRef, spawnList);

        particlesController = new ParticlesController(shootParticles);

        ServicesRegistrations();

        EventSubscriptions();
    }

    private void ServicesRegistrations()
    {
        ServiceLocator.Register(waveManager.waveReferences);
        ServiceLocator.Register(waveManager.waveSize);
        ServiceLocator.Register(gameManage);
        ServiceLocator.Register(particlesController);
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

    public static Rigidbody CreatePlayer(PlayerSO playerSO)
    {
        return Instantiate(playerSO.rbPrefab, playerSO.spawnPoint.position, Quaternion.identity);
    }
    public static UnityEngine.Object CreateEntity(UnityEngine.Object entity, Transform transform)
    {
        return Instantiate(entity, transform);
    }

    public static UnityEngine.Object CreateEntity(UnityEngine.Object entity, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        return Instantiate(entity, spawnPosition, spawnRotation);
    }

    public static ParticleSystem SpawnParticles(ParticleSystem shootParticles, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        return Instantiate(shootParticles, spawnPosition, spawnRotation);
    }
}
