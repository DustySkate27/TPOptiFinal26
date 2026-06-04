
using System.Collections.Generic;
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
            Dictionary<UnityEngine.Object, Enemy> enemyDick = ServiceLocator.Get<Dictionary<UnityEngine.Object, Enemy>>();

            if (enemyDick.TryGetValue(hit.collider.gameObject, out Enemy enemy))
            {
                Debug.Log(enemy.TakeDamage(1f));
            }
        }
    }
}
