using UnityEngine;

public class WinGameEvent : IGameEvent
{
    public WinGameEvent() { }
}
public class LoseGameEvent : IGameEvent
{
    public LoseGameEvent() { }
}
public class DeadEnemyEvent : IGameEvent
{
    public UnityEngine.Object objectInstance;

    public DeadEnemyEvent(UnityEngine.Object instance)
    {
        objectInstance = instance;
    }
}