using UnityEngine;

public class LineRendManager : IFixedUpdatable
{
    public LineRenderer line;
    private Transform startPoint;
    private int distance = 10;
    public LineRendManager(LineRenderer lineRenderer, Transform startPoint)
    {
        this.line = lineRenderer;
        this.startPoint = startPoint;

        CustomUpdateManager.Instance.Register(this);
    }

    public void Tick(float deltaTime)
    {
        line.SetPosition(0, startPoint.position);
        line.SetPosition(1, startPoint.position + startPoint.forward * distance);
    }
}
