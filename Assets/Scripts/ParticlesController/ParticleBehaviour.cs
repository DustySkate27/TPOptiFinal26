using UnityEngine;

public class ParticleBehaviour : IFixedUpdatable
{
    private Object thisInstance;

    private float aliveLimit = 1;
    private float currentTime = 0;

    private bool isRegistered;

    // Se llama UNA sola vez, cuando se crea el wrapper en el warm-up
    public ParticleBehaviour() { }

    // Se llama cada vez que la partícula se reactiva desde el pool
    public void Init(Object particle)
    {
        thisInstance = particle;
        currentTime = 0; // importante: resetear el estado del uso anterior

        if (!isRegistered)
        {
            CustomUpdateManager.Instance.Register(this);
            isRegistered = true;
        }
    }

    public void FixedTick(float deltaTime)
    {
        currentTime += deltaTime;
        if (currentTime > aliveLimit)
        {
            PoolBack();
        }
    }

    private void PoolBack()
    {
        EventBus.Publish(new OnParticleEndEvent(thisInstance));
    }

    // Se llama cuando la partícula vuelve al pool
    public void OnReturnedToPool()
    {
        CustomUpdateManager.Instance.Unregister(this);
        isRegistered = false;
        thisInstance = null;
    }
}