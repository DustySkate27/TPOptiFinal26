public class WinGameEvent : IGameEvent
{
    public WinGameEvent() { }
}
public class LoseGameEvent : IGameEvent
{
    public LoseGameEvent() { }
}
public class PlayerDead : IGameEvent
{
    public PlayerDead() { }
}
public class DisableEntityEvent : IGameEvent
{
    public UnityEngine.Object objectInstance;

    public DisableEntityEvent(UnityEngine.Object instance)
    {
        objectInstance = instance;
    }
}

public class OnParticleEndEvent : IGameEvent
{
    public UnityEngine.Object objectInstance;

    public OnParticleEndEvent(UnityEngine.Object instance)
    {
        objectInstance = instance;
    }
}