
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class PlayerAttack : IUpdatable
{
    private  Camera playerCamera;
    private float distance;
    private LayerMask enemyLayer;
    private float rateOfFire = 0.5f;

    private float currentTime = 0f;

    public PlayerAttack(Camera playerCamera, float shootDistance, LayerMask detectionLayer)
    {
        this.playerCamera = playerCamera;
        distance = shootDistance;
        enemyLayer = detectionLayer;

        CustomUpdateManager.Instance.Register(this);
    }
    public void Tick(float deltaTime)
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            if (currentTime >= rateOfFire)
            {
                Shoot();
                currentTime = 0f;
            }
        }

        if (currentTime < rateOfFire + 0.5f)
        {
            currentTime += deltaTime;
        }

        Debug.Log(currentTime);
    }

    public void Shoot()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit collision, distance))
        {
            ServiceLocator.Get<ParticlesController>().SpawnShootParticle(collision.point, Quaternion.LookRotation(collision.normal));
        }

        if (Physics.Raycast(ray, out RaycastHit hit, distance, enemyLayer))
        {
            //Debug.Log($"Hit: {hit.collider.gameObject.name} en posición {hit.point}");

            //ServiceLocator.Get<ParticlesController>().SpawnShootParticle(hit.transform.position, hit.transform.rotation);

            Dictionary<UnityEngine.Object, Enemy> enemyDict = ServiceLocator.Get<Dictionary<UnityEngine.Object, Enemy>>();

            if (enemyDict.TryGetValue(hit.collider.gameObject, out Enemy enemy))
            {
                enemy.TakeDamage(1f);
            }
        }
    }

}
