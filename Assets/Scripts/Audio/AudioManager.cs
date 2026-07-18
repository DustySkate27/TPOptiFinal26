using UnityEngine;

public class AudioManager
{
    private AudioSource audioSource;
    private Transform transform;

    public AudioManager(AudioSource audioSource, Transform transform)
    {
        this.audioSource = audioSource;
        this.transform = transform;

        EventBus.Subscribe<OnPlaySound>(PlaySound);
    }

    public void PlaySound(OnPlaySound soundEvent)
    {
        transform.position = soundEvent.position;

        audioSource.clip = soundEvent.sound;

        audioSource.Play();
    }
}
