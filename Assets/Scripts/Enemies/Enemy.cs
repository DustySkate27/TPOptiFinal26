
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : IHealth, IUpdatable
{
    private float hp;
    private EnemyMovement movement;
    private EnemyAttack attack;
    private UnityEngine.Object thisInstance;
    private AudioClip destroySound;

    public Enemy(UnityEngine.Object instanceOfAnObject, Transform target, Rigidbody targetRB, EnemySO scriptObj)
    {
        CustomUpdateManager.Instance.Register(this);
        
        hp = scriptObj.hp;
        thisInstance = instanceOfAnObject;
        destroySound = scriptObj.destroySound;
        
        movement = new EnemyMovement(instanceOfAnObject.GameObject().transform, target, targetRB, scriptObj.speed, scriptObj.maxForce, scriptObj.rotationSpeed, scriptObj.predictionFactor);
        attack = new EnemyAttack(instanceOfAnObject.GameObject().transform, target);
    }

    public void Tick(float deltaTime)
    {
        movement.Pursuit();
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

}