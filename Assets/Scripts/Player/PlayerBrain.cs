using UnityEngine;

public class PlayerBrain : MonoBehaviour, IUpdatable, IHealth
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float shootRange;

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        playerAttack = new PlayerAttack(spawnPoint, playerCamera, shootRange);

        CustomUpdateManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        CustomUpdateManager.Instance.Unregister(this);
    }

    public void Tick(float deltaTime)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(horizontal, vertical);

        playerMovement.Move(dir);

        if (Input.GetKeyDown(KeyCode.P))
        {
            playerAttack.Shoot();
        }
    }

    public void TakeDamage(float damage)
    {

    }


}
