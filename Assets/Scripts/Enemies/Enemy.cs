using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Enemy : IHealth, IUpdatable
{
    private float hp;
    private Object gameObject;
    private EnemyMovement movement;

    public Enemy(Object gameObject, Transform target, Rigidbody targetRB, EnemySO scriptObj)
    {
        CustomUpdateManager.Instance.Register(this);
        
        this.gameObject = gameObject;
        hp = scriptObj.hp;
        movement = new EnemyMovement(this.gameObject.GameObject().transform, target, targetRB, scriptObj.speed, scriptObj.maxForce, scriptObj.rotationSpeed, scriptObj.predictionFactor);
    }

    public void Tick(float deltaTime)
    {
        movement.Pursuit();
    }


    public float TakeDamage(float damage)
    {
        hp -= damage;

        return hp;
    }

}