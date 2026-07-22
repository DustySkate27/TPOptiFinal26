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
    private ObjectPoolManager objectPoolManager;
    private UIManager uiManager;


    private void Awake()
    {
        gameManage = this;
        
        uiManager = ServiceLocator.Get<UIManager>();
        uiManager.GetGameManager(this);

        objectPoolManager = new ObjectPoolManager();

        Rigidbody playerInstanceRef = CreatePlayer(playerSO);

        lineManager = new LineRendManager(lineRenderer, startOfLine);

        particleManager = new ParticleManager(particlePrefab, objectPoolManager);

        audioManager = new AudioManager(audioSource, audioSource.transform);

        waveManager = new WaveManager(objectPoolManager, enemySO, playerInstanceRef.transform, playerInstanceRef, spawnList);

        ServicesRegistrations();

        playerBrain = new PlayerBrain(this, playerInstanceRef.transform, playerInstanceRef, playerCamera, playerSO);

        EventSubscriptions();
    }

    private void ServicesRegistrations() //Additional registration: UIManager (on its awake)
    {
        ServiceLocator.Register(waveManager.waveReferences);
        ServiceLocator.Register(gameManage);
        ServiceLocator.Register(particleManager);
        ServiceLocator.Register(lineManager);
    }

    private void EventSubscriptions() //Additional subscriptions: UIManager (on its Awake)
    {
        EventBus.Subscribe<EndGameEvent>(OnEndCond);
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

    public void OnPauseGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        uiManager.MovePanel(0);
        uiManager.PauseButtons();
    }

    public void OnEndCond(EndGameEvent end)
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        EventBus.Publish(new UnregisterEntities());

        objectPoolManager.ClearPools();

        CustomUpdateManager.Instance.CheckSuscriptions();

        uiManager.MovePanel(end.state);
        uiManager.EndingButtons();
    }

    public void RestartRun()
    {
        UnregisterServicies();

        EventBus.Publish(new UnregisterEntities());

        CustomUpdateManager.Instance.CheckSuscriptions();

        int eventSuscribe = EventBus.Clear();

        objectPoolManager.ClearPools();

        CustomUpdateManager.Instance.ClearAll();

        SceneManager.LoadScene("Game");

        Time.timeScale = 1f;
    }

    public void ReturnToMenu()
    {
        UnregisterServicies();

        EventBus.Publish(new UnregisterEntities());

        CustomUpdateManager.Instance.CheckSuscriptions();

        int eventSuscribe = EventBus.Clear();

        objectPoolManager.ClearPools();

        CustomUpdateManager.Instance.ClearAll();

        SceneManager.LoadScene("Menu");

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

    public static void CallCheck()
    {
        Debug.Log("Entre");
    }

    #region BUTTON ACTIONS
    public void OnReturnToMenu()
    {
        ReturnToMenu();
    }

    public void OnResetGame()
    {
        RestartRun();
    }

    public void OnResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion
}
