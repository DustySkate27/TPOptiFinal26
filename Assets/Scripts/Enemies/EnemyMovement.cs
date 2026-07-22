using UnityEngine;

public class EnemyMovement
{
    Transform target;
    private Rigidbody targetRB;

    private Transform transform;
    private Vector3 velocity;

    private float speed;
    private float maxForce;
    private float rotationSpeed;
    private float predictionFactor;

    private float personalArea;
    private float obstacleAngle;
    private float avoidanceRadius;
    private int colliderCapacity;
    public float targetWeight;
    private LayerMask obsMask;

    private Collider[] colliders;

    private float obstacleCheckInterval = 0.5f; 
    private float obstacleCheckTimer;
    private Vector3 cachedDeflectedDir;

    private float cosHalfObstacleAngle;

    public EnemyMovement(Transform transform, Transform target, Rigidbody targetRB, float speed, float maxForce, float rotationSpeed, float predictionFactor, EnemyAvoidanceSO avoidanceData)
    {
        this.transform = transform;
        this.speed = speed;
        this.maxForce = maxForce;
        this.rotationSpeed = rotationSpeed;
        this.predictionFactor = predictionFactor;
        this.target = target;
        this.targetRB = targetRB;

        personalArea = avoidanceData.ObstaclePersonalArea;
        avoidanceRadius = avoidanceData.ObstacleRadius;
        obstacleAngle = avoidanceData.ObstacleAngle;
        colliderCapacity = avoidanceData.ObstacleCount;
        targetWeight = avoidanceData.targetWeight;

        obsMask = avoidanceData.ObstacleLayer;

        colliders = new Collider[colliderCapacity];

        cosHalfObstacleAngle = Mathf.Cos(obstacleAngle * 0.5f * Mathf.Deg2Rad);

        obstacleCheckTimer = Random.Range(0f, obstacleCheckInterval);
    }

    public void Execute(float deltaTime)
    {
        Flocking(deltaTime);
        MoveWithAvoidance(deltaTime);
    }

    private Vector3 Pursuit()
    {
        if (target == null) return Vector3.zero;

        Vector3 pos = transform.position;
        Vector3 targetPos = target.position;
        Vector3 targetVelocity = targetRB.linearVelocity;
        targetVelocity.y = 0;

        float distance = Vector3.Distance(pos, targetPos);

        float maxLookAhead = 1.5f; 
        float lookAheadTime = Mathf.Min(distance / speed, maxLookAhead);

        Vector3 predictedPos = targetPos + targetVelocity * lookAheadTime;
        predictedPos.y = pos.y;

        Vector3 desired = (predictedPos - pos);
        desired.y = 0;
        desired.Normalize();

        desired *= speed;

        return CalculateSteering(desired);
    }

    private void MoveWithAvoidance(float deltaTime)
    {
        if (velocity == Vector3.zero) return;

        Vector3 flatVelocity = velocity;
        flatVelocity.y = 0;

        obstacleCheckTimer -= deltaTime;
        if (obstacleCheckTimer <= 0f)
        {
            Vector3 deflectedDir = ObstacleAvoidance(flatVelocity.normalized, calculateY: false);
            deflectedDir.y = 0;
            if (deflectedDir == Vector3.zero) deflectedDir = flatVelocity.normalized;
            deflectedDir.Normalize();

            cachedDeflectedDir = deflectedDir;
            obstacleCheckTimer = obstacleCheckInterval;
        }

        Vector3 moveVelocity = cachedDeflectedDir * flatVelocity.magnitude;

        if (cachedDeflectedDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cachedDeflectedDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * deltaTime
            );
        }

        transform.position += moveVelocity * deltaTime;
        velocity.y = 0;
    }

    public Vector3 ObstacleAvoidance(Vector3 currDir, bool calculateY = true)
    {
        Vector3 pos = transform.position;

        int count = Physics.OverlapSphereNonAlloc(pos, avoidanceRadius, colliders, obsMask);

        Collider nearColl = null;
        float nearCollDistance = 0;
        Vector3 nearClosetPoint = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            var currColl = colliders[i];
            Vector3 closetPoint = currColl.ClosestPoint(pos);
            if (!calculateY) closetPoint.y = pos.y;
            Vector3 dirToColl = closetPoint - pos;
            float distance = dirToColl.magnitude;

            float dot = Vector3.Dot(dirToColl.normalized, currDir);
            if (dot < cosHalfObstacleAngle) continue;

            if (nearColl == null || distance < nearCollDistance)
            {
                nearColl = currColl;
                nearCollDistance = distance;
                nearClosetPoint = closetPoint;
            }
        }

        if (nearColl == null)
        {
            return currDir;
        }

        Vector3 relativePos = transform.InverseTransformPoint(nearClosetPoint);
        Vector3 dirToClosetPoint = (nearClosetPoint - pos).normalized;
        Vector3 newDir;
        if (relativePos.x < 0)
        {
            newDir = Vector3.Cross(transform.up, dirToClosetPoint);
        }
        else
        {
            newDir = -Vector3.Cross(transform.up, dirToClosetPoint);
        }

        return Vector3.Lerp(currDir, newDir, (avoidanceRadius - Mathf.Clamp(nearCollDistance - personalArea, 0, avoidanceRadius)) / avoidanceRadius);
    }

    private void Flocking(float deltaTime)
    {
        Vector3 pursuitForce = Pursuit() * targetWeight;

        AddForce(pursuitForce, deltaTime);
    }

    public Vector3 CalculateSteering(Vector3 desired)
    {
        Vector3 steering = desired - velocity;
        steering.y = 0;
        return Vector3.ClampMagnitude(steering, 100f);
    }

    public void AddForce(Vector3 force, float deltaTime)
    {
        force.y = 0;
        velocity.y = 0;
        velocity = Vector3.ClampMagnitude(velocity + force * deltaTime, speed);
    }
}