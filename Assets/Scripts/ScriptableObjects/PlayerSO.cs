using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerSO", menuName = "ScriptableObjects/Player")]
public class PlayerSO : ScriptableObject
{
    public Rigidbody rbPrefab;
    public Transform spawnPoint;
    public float shootRange;
    public LayerMask enemyLayer;
    public AudioClip shootSound;
}
