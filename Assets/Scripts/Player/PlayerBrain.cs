using UnityEngine;

public class PlayerBrain : IUpdatable, IHealth
{
    private Transform playerTransform;
    private Rigidbody rb;
    private Camera playerCamera;
    private float shootRange;
    private LayerMask enemyLayer;

    private float horizontalInput, verticalInput;
    private float xRotation, yRotation;
    private float sensitivity = 500f;

    private PlayerMovement playerMovement;
    private PlayerMovement playerRun;
    private PlayerAttack playerAttack;

    private float hp;

    public PlayerBrain(Transform _playerTransform, Rigidbody playerRB, Camera camera, float _shootRange, LayerMask _enemyLayer)
    {
        playerTransform = _playerTransform;
        rb = playerRB;
        playerCamera = camera;
        shootRange = _shootRange;
        enemyLayer = _enemyLayer;

        CustomUpdateManager.Instance.Register(this);

        playerAttack = new PlayerAttack(playerCamera, shootRange, enemyLayer);
        playerMovement = new PlayerMovement(playerTransform, rb, playerCamera.transform, 2000f);
        playerRun = new PlayerMovement(playerTransform, rb, playerCamera.transform, 4000f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void PlayerLoses(PlayerDead playerDead)
    {
        CustomUpdateManager.Instance.Unregister(this);
    }

    public void Tick(float deltaTime)
    {
        playerMovement.MoveCamera(MoveCameraDirection());
        playerMovement.Move(MoveInputDirection());

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerRun.Move(MoveInputDirection());
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            playerAttack.Shoot();
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
