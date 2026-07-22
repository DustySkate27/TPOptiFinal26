using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyAvoidanceSO", menuName = "ScriptableObjects/EnemyAvoidance")]

public class EnemyAvoidanceSO : ScriptableObject
{
    public float ObstacleRadius;
    public float ObstacleAngle;
    public float ObstaclePersonalArea;
    public int ObstacleCount;
    public float targetWeight;
    public LayerMask ObstacleLayer;
}
