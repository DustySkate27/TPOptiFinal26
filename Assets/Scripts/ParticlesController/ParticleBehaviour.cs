using UnityEngine;

public class ParticleBehaviour : IFixedUpdatable
{
    private Object thisInstance;

    private float aliveLimit = 1;
    private float currentTime = 0;

    public ParticleBehaviour(Object particle)
    {
        CustomUpdateManager.Instance.Register(this);

        thisInstance = particle;
    }

    public void Tick(float deltaTime)
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
}
