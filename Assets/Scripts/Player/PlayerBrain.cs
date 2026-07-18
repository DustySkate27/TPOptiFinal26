using Unity.VisualScripting;
using UnityEngine;

public class PlayerBrain : IUpdatable,IFixedUpdatable , IHealth
{
    private Transform playerTransform;
    private Rigidbody rb;
    private Camera playerCamera;
    private float shootRange;
    private LayerMask enemyLayer;
    private AudioClip shootSound;

    private float horizontalInput, verticalInput;
    private float xRotation, yRotation;
    private float sensitivity = 100f;

    private PlayerMovement playerMovement;
    private PlayerMovement playerRun;
    private PlayerAttack playerAttack;

    private float hp;

    public PlayerBrain(Transform _playerTransform, Rigidbody playerRB, Camera camera, PlayerSO playerSO)
    {
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
    }

    private void PlayerLoses(PlayerDead playerDead)
    {
        CustomUpdateManager.Instance.Unregister((IUpdatable)this);
        CustomUpdateManager.Instance.Unregister((IFixedUpdatable)this);
    }

    public void Tick(float deltaTime)
    {
        playerMovement.MoveCamera(MoveCameraDirection());
    }

    public void FixedTick(float deltaTime)
    {
        playerMovement.Move(MoveInputDirection());

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerRun.Move(MoveInputDirection());
        }
    }

    private Vector3 MoveInputDirection()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 dir = playerTransform.forward * verticalInput + playerTransform.right * horizontalInput;

        return dir;
    }

    private Vector3 MoveCameraDirection()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Vector2 dir = new Vector2(xRotation, yRotation);

        return dir;
    } 

    public void TakeDamage(float damage)
    {
        hp -= damage;
    }

}
