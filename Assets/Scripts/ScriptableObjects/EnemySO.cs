using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemySO", menuName = "ScriptableObjects/Enemy")]
public class EnemySO : ScriptableObject
{
    public GameObject prefab;
    public float speed;
    public float maxForce;
    public float rotationSpeed;
    public float predictionFactor;
}