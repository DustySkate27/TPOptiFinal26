using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Enemy : IHealth, IUpdatable
{
    private float hp;
    private EnemyMovement movement;
    private UnityEngine.Object thisInstance;

    public Enemy(UnityEngine.Object instanceOfAnObject, Transform target, Rigidbody targetRB, EnemySO scriptObj)
    {
        CustomUpdateManager.Instance.Register(this);
        
        hp = scriptObj.hp;
        thisInstance = instanceOfAnObject;

        movement = new EnemyMovement(instanceOfAnObject.GameObject().transform, target, targetRB, scriptObj.speed, scriptObj.maxForce, scriptObj.rotationSpeed, scriptObj.predictionFactor);
    }

    public void Tick(float deltaTime)
    {
        movement.Pursuit();
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
            DestroyEnemy();
    }

    public void DestroyEnemy()
    {
        EventBus.Publish(new DeadEnemyEvent(thisInstance));
    }

}