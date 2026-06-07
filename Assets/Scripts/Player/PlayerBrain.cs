using UnityEngine;

public class PlayerBrain : MonoBehaviour, IUpdatable, IHealth
{

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float shootRange;
    [SerializeField] private LayerMask enemyLayer;

    private float horizontalInput, verticalInput;
    private float xRotation, yRotation;

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    private float hp;

    private void Awake()
    {
        CustomUpdateManager.Instance.Register(this);

        playerAttack = new PlayerAttack(spawnPoint, playerCamera, shootRange, enemyLayer);
        playerMovement = new PlayerMovement(transform, rb, playerCamera.transform, 1000f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void PlayerLoses()
    {
        CustomUpdateManager.Instance.Unregister(this);
    }

    public void Tick(float deltaTime)
    {
        playerMovement.MoveCamera(MoveCameraDirection());
        playerMovement.Move(MoveInputDirection());

        if (Input.GetKeyDown(KeyCode.P))
        {
            playerAttack.Shoot();
        }
    }

    private Vector3 MoveInputDirection()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 dir = transform.forward * verticalInput + transform.right * horizontalInput;

        return dir;
    }

    private Vector3 MoveCameraDirection()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 400f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * 400f;

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
