

using UnityEngine;

public class EnemyMovement
{
    Transform target;

    private Enemy enemy;
    private Transform transform;
    private Vector3 currentSpeed;

    private float speed = 5f;
    private float maxForce = 5f;
    private float rotationSpeed = 5f;
    private float predictionFactor = 0.05f;
    private float slowingRadius = 15f;

    private Collider[] colliders;
    private float personalArea;
    private float avoidanceRadius;
    private int colliderCapacity;
    private LayerMask obsMask;

    public EnemyMovement(Transform transform)
    {
        this.transform = transform;
    }

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

        var futurePosition = target.position + GameManager.playerRigidbody.linearVelocity * t; //Añade a la posicion enemiga la posicion del objetivo por la prediccion

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
}
