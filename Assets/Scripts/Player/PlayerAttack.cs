
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack
{
    private Camera playerCamera;
    private float distance;
    private LayerMask enemyLayer;
    private float rateOfFire = 0.5f;

    private float currentTime = 0f;
    private ParticleManager particleManager;
    private LineRendManager line;

    private AudioClip shootSound;

    public PlayerAttack(Camera playerCamera, float shootDistance, LayerMask detectionLayer, AudioClip shootSound)
    {
        this.playerCamera = playerCamera;
        distance = shootDistance;
        enemyLayer = detectionLayer;
        this.shootSound = shootSound;

        particleManager = ServiceLocator.Get<ParticleManager>();
        line = ServiceLocator.Get<LineRendManager>();
    }
    public void Tick(float deltaTime)
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            if (currentTime >= rateOfFire && !Cursor.visible)
            {
                Shoot();
                currentTime = 0f;
            }
        }
        

        if (currentTime < rateOfFire + 0.5f)
        {
            currentTime += deltaTime;
        }
    }

    public void Shoot()
    {
        EventBus.Publish(new OnPlaySound(shootSound, playerCamera.transform));

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        line.Shot();

        if (Physics.Raycast(ray, out RaycastHit collision, distance))
        {
            particleManager.SpawnParticle(collision.point, Quaternion.LookRotation(collision.normal));
        }

        if (Physics.Raycast(ray, out RaycastHit hit, distance, enemyLayer))
        {
            Dictionary<UnityEngine.Object, Enemy> enemyDict = ServiceLocator.Get<Dictionary<UnityEngine.Object, Enemy>>();

            if (enemyDict.TryGetValue(hit.collider.gameObject, out Enemy enemy))
            {
                enemy.TakeDamage(1f);
            }
        }
    }

}
