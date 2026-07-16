using UnityEngine;

public class ParticlesController : IUpdatable
{
    private ParticleSystem shootPartciles;
    private GameManager gameManager;

    public ParticlesController(ParticleSystem shootParticles, GameManager gameManager)
    {
        this.shootPartciles = shootParticles;
        this.gameManager = gameManager;

        CustomUpdateManager.Instance.Register(this);
    }

    public void Tick(float deltaTime)
    {
        
    }

    public void SpawnShootParticle(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        gameManager.SpawnParticles(spawnPosition, spawnRotation);
    }
}
