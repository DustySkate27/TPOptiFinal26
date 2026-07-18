using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour, IUpdatable
{
    public List<GameObject> waveMovableObjects;
    public List<GameObject> currentWaveMovableObjects;
    public TextMeshProUGUI enemiesLeft;

    private float movingTime;
    public float limitTime;
    public float speed;
    private bool act = false;
    private int currentWave;
    private int currentSize;

    private void Awake()
    {
        CustomUpdateManager.Instance.Register(this);
        EventBus.Subscribe<OnWaveInit>(OnWaveInit);
        EventBus.Subscribe<UpdateTextEvent>(UpdateText);
    }

    public void Tick(float deltaTime)
    {
        if (act)
        {
            Debug.Log("entre");
            movingTime += deltaTime;
            WaveTransition(currentWave, currentSize);
        }
        else
        {
            movingTime = 0;
            Debug.Log("sali");
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
}
