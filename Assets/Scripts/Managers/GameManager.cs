using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Rigidbody playerRigidbody;

    public static Action OnWinCondition;

    public static Action PreComp; //Todo lo que necesite PreComp en pantalla de carga

    private void Awake()
    {
        OnWinCondition += StopTime;
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ResetScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    public static UnityEngine.Object CreateEntity(UnityEngine.Object entity, Transform transform)
    {
        return Instantiate(entity, transform);
    }
}
