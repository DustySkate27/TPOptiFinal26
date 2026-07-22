using UnityEngine;

public class EnemyAttack
{
    private Transform attacker;
    private Transform target;

    private float range = 2f;

    public EnemyAttack(Transform attacker, Transform target)
    {
        this.attacker = attacker;
        this.target = target;
    }

    public void Check()
    {
        if (Vector3.Distance(target.position, attacker.position) < range)
        {
            EventBus.Publish(new EndGameEvent(2));
        }
    }
}
