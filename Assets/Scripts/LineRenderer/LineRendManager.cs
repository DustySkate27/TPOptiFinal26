using UnityEngine;

public class LineRendManager : IFixedUpdatable
{
    private LineRenderer line;
    private Transform startPoint;
    private int distance = 10;
    private bool act = false;
    private float timeAlive = 0;
    public LineRendManager(LineRenderer lineRenderer, Transform startPoint)
    {
        this.line = lineRenderer;
        this.startPoint = startPoint;

        CustomUpdateManager.Instance.Register(this);

        EventBus.Subscribe<UnregisterEntitys>(UnregisterEntity);
    }

    public void FixedTick(float deltaTime)
    {
        if (act)
        {
            timeAlive += deltaTime;
        }
        if (timeAlive > 0.1f)
        {
            line.gameObject.SetActive(false);
            act = false;
            timeAlive = 0;
        }
    }

    public void Shot()
    {
        line.gameObject.SetActive(true);
        act = true;
        line.SetPosition(0, startPoint.position);
        line.SetPosition(1, startPoint.position + startPoint.forward * distance);
    }

    private void UnregisterEntity(UnregisterEntitys unregisterEvent)
    {
        CustomUpdateManager.Instance.Unregister(this);
    }
}
