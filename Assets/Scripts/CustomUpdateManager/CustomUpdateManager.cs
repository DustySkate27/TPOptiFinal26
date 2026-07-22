
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-3)]
public class CustomUpdateManager : MonoBehaviour
{
    public static CustomUpdateManager Instance;

    private List<IUpdatable> updatables = new List<IUpdatable>();
    private List<IUpdatable> pendingUpdate = new List<IUpdatable>();
    private int currentIndexUpdate;

    private List<IFixedUpdatable> fixedUpdatables = new List<IFixedUpdatable>();
    private List<IFixedUpdatable> pendingFixUpdate = new List<IFixedUpdatable>();
    private int currentIndexFixUpdate;

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
        for (currentIndexUpdate = 0; currentIndexUpdate < updatables.Count ; currentIndexUpdate++)
        {
            if(updatables.Count > 0)
                updatables[currentIndexUpdate].Tick(Time.deltaTime); 
        }
        
        updatables.AddRange(pendingUpdate);
        pendingUpdate.Clear();
    }

    void FixedUpdate()
    {
        for (currentIndexFixUpdate = 0; currentIndexFixUpdate < fixedUpdatables.Count; currentIndexFixUpdate++)
        {
            fixedUpdatables[currentIndexFixUpdate].FixedTick(Time.deltaTime);
        }

        fixedUpdatables.AddRange(pendingFixUpdate);
        pendingFixUpdate.Clear();
    }

    public void Register(IUpdatable updatable)
    {
        if (!updatables.Contains(updatable))
            pendingUpdate.Add(updatable);
    }

    public void Unregister(IUpdatable updatable)
    {
        updatables.Remove(updatable);
        currentIndexUpdate--;
    }

    public void Register(IFixedUpdatable updatable)
    {
        if (!fixedUpdatables.Contains(updatable))
            pendingFixUpdate.Add(updatable);
    }

    public void Unregister(IFixedUpdatable updatable)
    {
        fixedUpdatables.Remove(updatable);
        currentIndexFixUpdate--;
    }

    public void ClearAll()
    {
        updatables.Clear();
        pendingUpdate.Clear();
        currentIndexUpdate = 0;

        fixedUpdatables.Clear();
        pendingFixUpdate.Clear();
        currentIndexFixUpdate = 0;
    }

    public void CheckSuscriptions()
    {
        Debug.Log("Update: " + updatables.Count + "; FixedUpdate: " + fixedUpdatables.Count);

        foreach(IUpdatable entitys in updatables)
        {
            Debug.Log(entitys.ToString());
        }
    }
}