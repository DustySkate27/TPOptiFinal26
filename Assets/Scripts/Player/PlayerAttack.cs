
using UnityEngine;

public class PlayerAttack
{
    private Transform spawn;
    private  Camera playerCamera;
    private float distance;
    private LayerMask enemyLayer;

    public PlayerAttack(Transform spawnPoint, Camera playerCamera, float shootDistance, LayerMask detectionLayer)
    {
        spawn = spawnPoint;
        this.playerCamera = playerCamera;
        distance = shootDistance;
        enemyLayer = detectionLayer;
    }

    public void Shoot()
    {
        Ray ray = new Ray(spawn.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance, enemyLayer))
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
