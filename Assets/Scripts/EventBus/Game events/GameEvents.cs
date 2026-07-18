using UnityEngine;

public class WinGameEvent : IGameEvent
{
    public WinGameEvent() { }
}
public class LoseGameEvent : IGameEvent
{
    public LoseGameEvent() { }
}

public class UnsuscribeClasses : IGameEvent
{
    public UnsuscribeClasses() { }
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

public class OnPlaySound : IGameEvent
{
    public AudioClip sound;
    public Vector3 position;

    public OnPlaySound(AudioClip sound, Vector3 position)
    {
        this.sound = sound;
        this.position = position;
    }

    public OnPlaySound(AudioClip sound, Transform transform)
    {
        this.sound = sound;
        position = transform.position;
    }
}

public class OnWaveInit : IGameEvent
{
    public int wave;
    public int waveSize;

    public OnWaveInit(int currentWave, int initialSize) 
    {
        wave = currentWave;
        waveSize = initialSize;
    }
}

public class UpdateTextEvent : IGameEvent
{
    public string text;

    public UpdateTextEvent(string text)
    {
        this.text = text;
    }
}

public class EnemyDicUnregister : IGameEvent
{
    public EnemyDicUnregister() { }
}