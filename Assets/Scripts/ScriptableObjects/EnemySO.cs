using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemySO", menuName = "ScriptableObjects/Enemy")]
public class EnemySO : ScriptableObject
{
    public GameObject prefab;
    public float hp;
    public float speed;
    public float maxForce;
    public float rotationSpeed;
    public float predictionFactor;
    public AudioClip destroySound;

    public EnemyAvoidanceSO avoidanceData;
}