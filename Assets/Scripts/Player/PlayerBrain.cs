using UnityEngine;

public class PlayerBrain : MonoBehaviour, IUpdatable, IHealth
{

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float shootRange;
    [SerializeField] private LayerMask enemyLayer;

    private float horizontalInput, verticalInput;
    private float xRotation, yRotation;

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    private float hp;

    private void Start()
    {
        playerAttack = new PlayerAttack(spawnPoint, playerCamera, shootRange, enemyLayer);
        playerMovement = new PlayerMovement(this.transform,playerCamera.transform , 50f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CustomUpdateManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        CustomUpdateManager.Instance.Unregister(this);
    }

    public void Tick(float deltaTime)
    {
        playerMovement.Move(MoveInputDirection());
        playerMovement.MoveCamera(MoveCameraDirection());

        if (Input.GetKeyDown(KeyCode.P))
        {
            playerAttack.Shoot();
        }
    }

    private Vector3 MoveInputDirection()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 dir = this.transform.forward * verticalInput + this.transform.right * horizontalInput;

        return dir;
    }

    private Vector3 MoveCameraDirection()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 50f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * 50f;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Vector2 dir = new Vector2(xRotation, yRotation);

        return dir;
    } 

    public float TakeDamage(float damage)
    {
        return hp -= damage;
    }


}
