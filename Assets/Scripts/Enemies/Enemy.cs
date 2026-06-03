using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Enemy : IHealth, IUpdatable
{
    private float hp;
    private GameObject gameObject;
    private EnemyMovement movement;
    private Transform target;

    public Enemy(GameObject gameObject, Transform target)
    {
        this.gameObject = gameObject;
        movement = new EnemyMovement(gameObject.transform);
        this.target = target;
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