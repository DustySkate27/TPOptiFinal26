using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : IUpdatable
{
    private ParticleSystem shootParticles;
    private List<ParticleSystem> activeParticles = new List<ParticleSystem>();

    public ParticlesController(ParticleSystem shootParticles)
    {
        this.shootParticles = shootParticles;

        CustomUpdateManager.Instance.Register(this);

        ParticleSystemPoolManager.WarmUp(shootParticles, 10);
    }

    public void Tick(float deltaTime)
    {
        for (int i = activeParticles.Count - 1; i >= 0; i--)
        {
            ParticleSystem particle = activeParticles[i];

            if (!particle.IsAlive(true))
            {
                activeParticles.RemoveAt(i);
                ParticleSystemPoolManager.ReturnObjectToPool(particle);
            }
        }
    }

    public void SpawnShootParticle(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        ParticleSystem instance = ParticleSystemPoolManager.SpawnObject(shootParticles, spawnPosition, spawnRotation);
        activeParticles.Add(instance);
    }
}