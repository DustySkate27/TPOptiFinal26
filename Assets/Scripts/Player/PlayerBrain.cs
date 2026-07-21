using Unity.VisualScripting;
using UnityEngine;

public class PlayerBrain : IUpdatable, IFixedUpdatable
{
    private Transform playerTransform;
    private Rigidbody rb;
    private Camera playerCamera;
    private float shootRange;
    private LayerMask enemyLayer;
    private AudioClip shootSound;
    

    private GameManager gameManager;
    private PlayerMovement playerMovement;
    private PlayerMovement playerRun;
    private PlayerAttack playerAttack;

    public PlayerBrain(GameManager gameManager,Transform _playerTransform, Rigidbody playerRB, Camera camera, PlayerSO playerSO)
    {
        this.gameManager = gameManager;
        playerTransform = _playerTransform;
        rb = playerRB;
        playerCamera = camera;
        shootRange = playerSO.shootRange;
        enemyLayer = playerSO.enemyLayer;
        shootSound = playerSO.shootSound;

        CustomUpdateManager.Instance.Register((IUpdatable)this);
        CustomUpdateManager.Instance.Register((IFixedUpdatable)this);

        playerAttack = new PlayerAttack(playerCamera, shootRange, enemyLayer, shootSound);
        playerMovement = new PlayerMovement(playerTransform, rb, playerCamera.transform, 10);
        playerRun = new PlayerMovement(playerTransform, rb, playerCamera.transform, 50);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EventBus.Subscribe<UnregisterEntities>(UnregisterEntity);
    }

    public void Tick(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.OnPauseGame();
        }
        playerMovement.MoveCamera();
        playerAttack.Tick(deltaTime);
    }

    public void FixedTick(float deltaTime)
    {
        playerMovement.Move();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerRun.Move();
        }
    }

    private void UnregisterEntity(UnregisterEntities unregisterEvent)
    {
        CustomUpdateManager.Instance.Unregister((IUpdatable)this);
        CustomUpdateManager.Instance.Unregister((IFixedUpdatable)this);
    }
}
