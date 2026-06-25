
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-2)]
public class CustomUpdateManager : MonoBehaviour
{
    public static CustomUpdateManager Instance;

    private List<IUpdatable> updatables = new List<IUpdatable>();
    private List<IUpdatable> pending = new List<IUpdatable>();
    private int currentIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        for (currentIndex = 0; currentIndex <= updatables.Count -1 ; currentIndex++)
        {
            updatables[currentIndex].Tick(Time.deltaTime); 
        }

        updatables.AddRange(pending);
        pending.Clear();
    }

    public void Register(IUpdatable updatable)
    {
        if (!updatables.Contains(updatable))
            pending.Add(updatable);
    }

    public void Unregister(IUpdatable updatable)
    {
        updatables.Remove(updatable);
        currentIndex--;
    }

}