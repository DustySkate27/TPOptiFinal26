
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-2)]
public class CustomUpdateManager : MonoBehaviour
{
    public static CustomUpdateManager Instance;

    private List<IUpdatable> updatables = new List<IUpdatable>();
    private List<IUpdatable> pendingUpdate = new List<IUpdatable>();
    private int currentIndexUpdate;

    private List<IFixedUpdatable> fixedUpdatables = new List<IFixedUpdatable>();
    private List<IFixedUpdatable> pendingFixUpdate = new List<IFixedUpdatable>();
    private int currentIndexFixUpdate;

    private List<ILateUpdatable> latedUpdatables = new List<ILateUpdatable>();
    private List<ILateUpdatable> pendingLateUpdate = new List<ILateUpdatable>();
    private int currentIndexLateUpdate;

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
        for (currentIndexUpdate = 0; currentIndexUpdate <= updatables.Count -1 ; currentIndexUpdate++)
        {
            updatables[currentIndexUpdate].Tick(Time.deltaTime); 
        }

        updatables.AddRange(pendingUpdate);
        pendingUpdate.Clear();
    }

    void FixedUpdate()
    {
        for (currentIndexFixUpdate = 0; currentIndexFixUpdate <= fixedUpdatables.Count - 1; currentIndexFixUpdate++)
        {
            fixedUpdatables[currentIndexFixUpdate].FixedTick(Time.deltaTime);
        }

        fixedUpdatables.AddRange(pendingFixUpdate);
        pendingFixUpdate.Clear();
    }

    //private void LateUpdate()
    //{
    //    for (currentIndexLateUpdate = 0; currentIndexLateUpdate <= latedUpdatables.Count - 1; currentIndexLateUpdate++)
    //    {
    //        latedUpdatables[currentIndexLateUpdate].Tick(Time.deltaTime);
    //    }

    //    latedUpdatables.AddRange(pendingLateUpdate);
    //    pendingLateUpdate.Clear();
    //}

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

    public void Register(ILateUpdatable updatable)
    {
        if (!latedUpdatables.Contains(updatable))
            pendingLateUpdate.Add(updatable);
    }

    public void Unregister(ILateUpdatable updatable)
    {
        latedUpdatables.Remove(updatable);
        currentIndexLateUpdate--;
    }
}