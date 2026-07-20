using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(-2)]
public class UIManager : MonoBehaviour, IUpdatable
{
    public List<GameObject> waveMovableObjects;
    public List<GameObject> currentWaveMovableObjects;
    public TextMeshProUGUI enemiesLeft;

    private float movingTime = 0;
    private float limitTime = 3;
    private float speed = -500;
    private bool act = false;
    private int currentWave;
    private int currentSize;

    private void Awake()
    {
        CustomUpdateManager.Instance.Register(this);
        EventBus.Subscribe<OnWaveInit>(OnWaveInit);
        EventBus.Subscribe<UpdateTextEvent>(UpdateText);

        EventBus.Subscribe<UnregisterEntitys>(UnregisterEntity);
    }

    public void Tick(float deltaTime)
    {
        if (act)
        {
            movingTime += deltaTime;
            WaveTransition(currentWave, currentSize);
        }
        else
        {
            movingTime = 0;
        }
    }

    public void OnWaveInit(OnWaveInit init)
    {
        currentWave = init.wave;
        currentSize = init.waveSize;
        EventBus.Publish(new UpdateTextEvent(currentSize.ToString()));
        act = true;
    }

    public void UpdateText(UpdateTextEvent update)
    {
        enemiesLeft.text = $"Enemies left: {update.text}";
    }

    private void WaveTransition(int newWave, int waveSize)
    {
        if (movingTime < limitTime)
        {
            waveMovableObjects[newWave].transform.Translate(new Vector2(0, 1) * Time.deltaTime * speed);
        }
        else
        {
            act = false;
        }
    }

    private void UnregisterEntity(UnregisterEntitys unregisterEvent)
    {
        CustomUpdateManager.Instance.Unregister(this);
    }
}
