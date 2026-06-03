
using UnityEngine;

public class PlayerAttack
{
    Transform spawn;
    Camera playerCamera;
    float distance;

    public PlayerAttack(Transform spawnPoint, Camera playerCamera, float shootDistance)
    {
        spawn = spawnPoint;
        this.playerCamera = playerCamera;
        distance = shootDistance;
    }

    public void Shoot()
    {
        Ray ray = new Ray(spawn.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            /*
            foreach (Enemy enemy in EnemyManager.Instance.enemies)
            {
                if (enemy.col == hit.collider)
                {
                    enemy.TakeDamage(damage);
                    break;
                }
            }
            */
        }
    }
}
