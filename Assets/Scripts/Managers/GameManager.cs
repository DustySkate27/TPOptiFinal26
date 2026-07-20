using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameManager gameManage;

    #region SERIFS
    public EnemySO enemySO;
    public PlayerSO playerSO;
    public Camera playerCamera;
    public List<Transform> spawnList;
    public GameObject particlePrefab;
    public LineRenderer lineRenderer;
    public Transform startOfLine;
    public AudioSource audioSource;
    #endregion

    private PlayerBrain playerBrain;
    private WaveManager waveManager;
    private ParticleManager particleManager;
    private LineRendManager lineManager;
    private AudioManager audioManager;


    private void Awake()
    {
        gameManage = this;

        Rigidbody playerInstanceRef = CreatePlayer(playerSO);

        lineManager = new LineRendManager(lineRenderer, startOfLine);

        particleManager = new ParticleManager(particlePrefab);

        audioManager = new AudioManager(audioSource, audioSource.transform);

        waveManager = new WaveManager(enemySO, playerInstanceRef.transform, playerInstanceRef, spawnList);

        ServicesRegistrations();

        playerBrain = new PlayerBrain(playerInstanceRef.transform, playerInstanceRef, playerCamera, playerSO);

        EventSubscriptions();
    }

    private void ServicesRegistrations() 
    {
        ServiceLocator.Register(waveManager.waveReferences);
        ServiceLocator.Register(gameManage);
        ServiceLocator.Register(particleManager);
        ServiceLocator.Register(lineManager);
    }

    private void EventSubscriptions() //Additional subscriptions: UIManager (on its Awake)
    {
        EventBus.Subscribe<WinGameEvent>(OnWinCond);
        EventBus.Subscribe<LoseGameEvent>(OnLoseCond);
        EventBus.Subscribe<DisableEntityEvent>(waveManager.OnEnemyDeadCond);
        EventBus.Subscribe<OnParticleEndEvent>(particleManager.OnParticleEnd);
    }
    private void UnregisterServicies()
    {
        ServiceLocator.Unregister<Dictionary<UnityEngine.Object, Enemy>>();
        ServiceLocator.Unregister<GameManager>();
        ServiceLocator.Unregister<ParticleManager>();
        ServiceLocator.Unregister<LineRendManager>();
    }

    /*
    public AudioSource CreateAudioSource() //For AudioPool (if done)
    {
        AudioSource audioSource;
        return audioSource = Instantiate(AudioSource);
    }
    */
    public void OnWinCond(WinGameEvent winEvent)
    {
        Time.timeScale = 0f;

        EventBus.Publish(new UnregisterEntitys());

        int eventSuscribe = EventBus.Clear();

        UnregisterServicies(); 

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Debug.Log("PREVIOUS");
        ObjectPoolManager.DebugPools();

        ObjectPoolManager.ClearPools();

        Debug.Log("POST MORTEM");
        ObjectPoolManager.DebugPools();

        SceneManager.LoadScene(1);
        
        Time.timeScale = 1f;
    }

    public void OnLoseCond(LoseGameEvent loseEvent)
    {
        Time.timeScale = 0f;

        EventBus.Publish(new UnregisterEntitys());;

        int eventSuscribe = EventBus.Clear();

        UnregisterServicies();

        Debug.Log("PREVIOUS");
        ObjectPoolManager.DebugPools();

        ObjectPoolManager.ClearPools();

        Debug.Log("POST MORTEM");
        ObjectPoolManager.DebugPools();
        SceneManager.LoadScene(0);

        Time.timeScale = 1f;
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

}
