using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Enemy : IHealth, IUpdatable
{
    private float hp;
    private Object gameObject;
    private EnemyMovement movement;
    private Transform target;

    public Enemy(Object gameObject, Transform target, Rigidbody targetRB, EnemySO scriptObj)
    {
        this.gameObject = gameObject;
        movement = new EnemyMovement(gameObject.GameObject().transform, target, targetRB, scriptObj.speed, scriptObj.maxForce, scriptObj.rotationSpeed, scriptObj.predictionFactor);
        CustomUpdateManager.Instance.Register(this);
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