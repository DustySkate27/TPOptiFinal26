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

        colliders = new Collider[colliderCapacity]; // <-- esto faltaba
    }

    public void Execute(float deltaTime)
    {
        Flocking(deltaTime);
        MoveWithAvoidance();
    }

    private Vector3 Pursuit()
    {
        if (target == null) return Vector3.zero;

        Vector3 targetPos = target.position;
        Vector3 targetVelocity = targetRB.linearVelocity;
        targetVelocity.y = 0;

        float distance = Vector3.Distance(transform.position, targetPos);

        // Limita el lookAhead: cerca del target predice poco, lejos predice más
        float maxLookAhead = 1.5f;  // ajustable en segundos
        float lookAheadTime = Mathf.Min(distance / speed, maxLookAhead);

        Vector3 predictedPos = targetPos + targetVelocity * lookAheadTime;
        predictedPos.y = transform.position.y;

        Debug.DrawLine(transform.position, predictedPos, Color.green);

        Vector3 desired = (predictedPos - transform.position);
        desired.y = 0;
        desired.Normalize();

        desired *= speed;

        return CalculateSteering(desired);
    }

    private void MoveWithAvoidance()
    {
        if (velocity == Vector3.zero) return;

        Vector3 flatVelocity = velocity;
        flatVelocity.y = 0;

        Vector3 deflectedDir = ObstacleAvoidance(flatVelocity.normalized, calculateY: false);
        deflectedDir.y = 0;
        if (deflectedDir == Vector3.zero) deflectedDir = flatVelocity.normalized;
        deflectedDir.Normalize();

        Vector3 moveVelocity = deflectedDir * flatVelocity.magnitude;

        if (deflectedDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(deflectedDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        transform.position += moveVelocity * Time.deltaTime;
        velocity.y = 0;
    }

    public Vector3 ObstacleAvoidance(Vector3 currDir, bool calculateY = true)
    {

        int count = Physics.OverlapSphereNonAlloc(transform.position, avoidanceRadius, colliders, obsMask);

        Collider nearColl = null;
        float nearCollDistance = 0;
        Vector3 nearClosetPoint = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            var currColl = colliders[i];
            Vector3 closetPoint = currColl.ClosestPoint(transform.position);
            if (!calculateY) closetPoint.y = transform.position.y;
            Vector3 dirToColl = closetPoint - transform.position;
            float distance = dirToColl.magnitude;
            float currAngle = Vector3.Angle(dirToColl, currDir);
            if (currAngle > obstacleAngle / 2) continue;

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
        Vector3 dirToClosetPoint = (nearClosetPoint - transform.position).normalized;
        Vector3 newDir;
        if (relativePos.x < 0)
        {
            newDir = Vector3.Cross(transform.up, dirToClosetPoint);
        }
        else
        {
            newDir = -Vector3.Cross(transform.up, dirToClosetPoint);
        }
        Debug.DrawRay(transform.position, newDir, Color.red);
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


    #region oldCode
    /*
    public void Pursuit()
    {
        var toQuarry = target.position - transform.position; //direccion al objetivo
        var distance = toQuarry.magnitude; //distancia
        float t = distance * predictionFactor; //factor de prediccion

        var pForward = transform.forward; //forward del enemigo
        var qForward = target.forward; //forward objetivo

        var relativeHeading = Vector3.Dot(pForward, qForward); //dot product para prediccion de direccion
        var toPursuer = (transform.position - target.position).normalized; //direccion al enemigo
        var forwardDot = Vector3.Dot(qForward, toPursuer); //dot product para direccion con prediccion

        if (forwardDot > 0 && relativeHeading < -0.95f) //si la direccion con prediccion es mayor a 0 y la prediccion es menor a -0.95
        {
            t = 0; //no hay prediccion
        }
        else //sino
        {
            if (relativeHeading < 0) t *= 1.5f; //Si prediccion menor a 0 => aumenta prediccion
            if (forwardDot < 0) t *= 1.2f; //Si direccion con prediccion => aumenta aun mas prediccion
        }

        var futurePosition = target.position + targetRB.linearVelocity * t; //Añade a la posicion enemiga la posicion del objetivo por la prediccion

        var dir = futurePosition - transform.position; //direccion prediciendo al objetivo
        var desired = dir.normalized * speed; //Direccion a la que va a ir el enemigo

        var avoidForce = ComputeAvoidance(); //Ejecución de Obstacle Avoidance

        Vector3 steer; //Inicializa el virado
        if (avoidForce.HasValue) //Si existe un obstáculo, obtiene la dirección de evasión
        {
            var evadeDesired = avoidForce.Value.normalized * speed; //Inicializa la evasión objetivo multiplicando la fuerza de evasión normalizada por la velocidad.
            steer = evadeDesired - currentSpeed; //El virado es equivalente a la diferencia entre la evasión objetivo y la dirección actual
        }
        else //Si no existe
        {
            steer = desired - currentSpeed; //El virado es equivalente a la dirección objetivo menos la actual.
        }

        steer = Vector3.ClampMagnitude(steer, maxForce); //Camplea la magnitud de la dirección entre si mismo y la potencia máxima de virado.
        currentSpeed += steer * Time.deltaTime; //le suma a la dirección actual el virado a lo largo del tiempo.
        currentSpeed = Vector3.ClampMagnitude(currentSpeed, speed); //Clampea la magnitud de la dirección actual entre si misma y la velocidad.
        currentSpeed.y = 0; //Neutraliza la altura de la dirección actual

        transform.position += currentSpeed * Time.deltaTime; //Suma a la posición.

        if (currentSpeed.sqrMagnitude > 0.001f) //Si la magnitud al cuadrado es menor al un número infimo
        {
            var targetRotation = Quaternion.LookRotation(currentSpeed.normalized); //Inicializa rotacion objetivo
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); //La iguala a la rotacion del transform
        }
    }

    public Vector3? ComputeAvoidance()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, avoidanceRadius, colliders, obsMask); //Detección de colisiones

        Collider nearestColl = null; //Inicializa en nulo la colision más cercana.
        float nearestDistance = float.MaxValue; //Inicializa en "infinito" la distancia a esa colisión.
        Vector3 nearestClosestPoint = Vector3.zero;//Inicializa en nulo la dirección a esa colisión.

        for (int i = 0; i < count; i++) //Recorre count
        {
            Vector3 closestPoint = colliders[i].ClosestPoint(transform.position); //Inicializa una direccion al punto más cercano de un collider
            closestPoint.y = transform.position.y; //Neutraliza la altura, para que no influya en el movimiento.

            Vector3 dirToColl = closestPoint - transform.position; //Inicializa una dirección equivalente a la diferencia al closestPoint
            float distance = dirToColl.magnitude; //Y almacena su magnitud, representando la distancia

            if (distance < nearestDistance) //Si la magnitud es menor al "infinito" ó al nearestDistance "mas cercano" previo. 
            {
                nearestColl = colliders[i]; //Se asigna el collider al que se considera "más cercano" por ahora.
                nearestDistance = distance; //Se almacena la distancia
                nearestClosestPoint = closestPoint; //Se almacena la direccion al punto más cercano de un collider
            }
        }

        if (nearestColl == null) return null; //Si no hay colliders, se devuelve null.

        Vector3 relativePos = transform.InverseTransformPoint(nearestClosestPoint); //Si sí hay colliders, convierte la dirección al punto más cercano de World a Local Space
        Vector3 dirToObstacle = (nearestClosestPoint - transform.position).normalized; //Inicializa la dirección normalizada al obstáculo
        Vector3 avoidDir = relativePos.x < 0 ?  //Evalua por qué lado rodear en funcion de la dirección en Local Space
            Vector3.Cross(transform.up, dirToObstacle) : -Vector3.Cross(transform.up, dirToObstacle);

        //Calcula la "Fuerza de la evasión" por medio de la diferencia entre el radio de evasión y un clampeo de la diferencia entre "más cercana" y "distancia mínima obligatoria" sobre radio de evasión
        float weight = (avoidanceRadius - Mathf.Clamp(nearestDistance - personalArea, 0, avoidanceRadius)) / avoidanceRadius;
        return avoidDir * weight; //Multiplica la dirección de evasión por la fuerza para respetar la distancia mínima obligatoria.
    }
    */
    #endregion
}
