using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-2)]
public class CustomUpdateManager : MonoBehaviour
{
    public static CustomUpdateManager Instance;

    private List<IUpdatable> updatables = new List<IUpdatable>();

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
        foreach (var u in updatables)
        {
            u.Tick(Time.deltaTime); 
        }
    }

    public void Register(IUpdatable updatable)
    {
        if (!updatables.Contains(updatable))
            updatables.Add(updatable);
    }

    public void Unregister(IUpdatable updatable)
    {
        updatables.Remove(updatable);
    }

}