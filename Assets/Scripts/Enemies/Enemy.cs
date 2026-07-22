using Unity.VisualScripting;
using UnityEngine;

public class Enemy : IHealth, IUpdatable
{
    private float hp;
    private EnemyMovement movement;
    private EnemyAttack attack;
    private UnityEngine.Object thisInstance;
    private AudioClip destroySound;
    private bool isRegistered;

    // Se llama UNA sola vez, cuando se crea el wrapper (warm-up del pool)
    public Enemy()
    {
        movement = new EnemyMovement();
        attack = new EnemyAttack();
    }

    // Se llama cada vez que el enemigo reaparece: resetea el estado, no alloca
    public void Init(UnityEngine.Object instanceOfAnObject, Transform target, Rigidbody targetRB, EnemySO scriptObj)
    {
        thisInstance = instanceOfAnObject;
        hp = scriptObj.hp;
        destroySound = scriptObj.destroySound;

        Transform selfTransform = instanceOfAnObject.GameObject().transform;

        movement.Reset(selfTransform, target, targetRB, scriptObj.speed, scriptObj.maxForce,
                        scriptObj.rotationSpeed, scriptObj.predictionFactor, scriptObj.avoidanceData);
        attack.Reset(selfTransform, target);

        if (!isRegistered)
        {
            CustomUpdateManager.Instance.Register(this);
            isRegistered = true;
        }
    }

    public void Tick(float deltaTime)
    {
        movement.Execute(deltaTime);
        attack.Check();
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
            DestroyEnemy();
    }

    public void DestroyEnemy()
    {
        EventBus.Publish(new OnPlaySound(destroySound, thisInstance.GameObject().transform));
        EventBus.Publish(new DisableEntityEvent(thisInstance));
    }

    // Llamalo cuando el enemigo vuelve al pool, para permitir re-Register la próxima vez si hace falta
    public void OnReturnedToPool()
    {
        isRegistered = false;
    }
}