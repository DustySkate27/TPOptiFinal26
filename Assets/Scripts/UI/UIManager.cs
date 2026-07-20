using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-2)]
public class UIManager : MonoBehaviour, IUpdatable
{
    public List<GameObject> waveMovableObjects;
    public List<Transform> gamePanels; //0: Pause, 1: Win, 2: Lose
    public TextMeshProUGUI enemiesLeft;
    public Transform buttons;
    public Transform resumeButton;

    #region Variables for Canvas Game Objects
    private Vector3 buttonsPositions = new Vector3(294.5f, 132.368744f, 0.736083984f);
    private Vector3 buttonsOffScreen = new Vector3(294.5f, -33.2874908f, 0.736083984f);
    private Vector3 resumePosition = new Vector3(294.5f, 132.368744f, 0.736083984f);
    private Vector3 resumeOffScreen = new Vector3(294.5f, -33.2874908f, 0.736083984f);
    private Vector3 panelPosition = new Vector3(294.5f, 165.5f, 0f);
    private Vector3 panelOffScreen = new Vector3(896.752502f, 165.5f, 0f);
    private Transform currentPanel;
    #endregion

    private GameManager gameManager;
    private float movingTime = 0;
    private float limitTime = 3;
    private float speed = -500;
    private bool act = false;
    private int currentWave;
    private int currentSize;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "Game")
        {
            CustomUpdateManager.Instance.Register(this);
            EventBus.Subscribe<OnWaveInit>(OnWaveInit);
            EventBus.Subscribe<UpdateTextEvent>(UpdateText);
            EventBus.Subscribe<UnregisterEntitys>(UnregisterEntity);
            ServiceLocator.Register(this);
        }
    }

    public void GetGameManager(GameManager gameMan)
    {
        gameManager = gameMan;
    }

    public void Tick(float deltaTime)
    {
        if (act)
        {
            movingTime += deltaTime;
            WaveTransition(currentWave, currentSize);
        }
        else
        {
            movingTime = 0;
        }
    }

    public void OnWaveInit(OnWaveInit init)
    {
        currentWave = init.wave;
        currentSize = init.waveSize;
        act = true;
    }

    public void UpdateText(UpdateTextEvent update)
    {
        enemiesLeft.text = $"Enemies killed: {update.kills} / {update.total}";
    }

    private void WaveTransition(int newWave, int waveSize)
    {
        if (movingTime < limitTime)
        {
            waveMovableObjects[newWave].transform.Translate(Vector2.up * Time.deltaTime * speed);
        }
        else
        {
            act = false;
        }
    }

    private void UnregisterEntity(UnregisterEntitys unregisterEvent)
    {
        CustomUpdateManager.Instance.Unregister(this);
    }

    #region OnButton Methods
    public void ResumeGame()
    {
        gameManager.OnResumeGame();
        buttons.transform.position = buttonsOffScreen;
        resumeButton.transform.position = resumeOffScreen;
        currentPanel.position = panelOffScreen;
        currentPanel = null;
    }

    public void ResetGame()
    {
        buttons.position = buttonsOffScreen; //CAPAZ SE PUEDE SACAR POR EL RESET DE ESCENA
        currentPanel.position = panelOffScreen;
        currentPanel = null;
        gameManager.OnResetGame();
    }

    /// <summary>
    /// Refers to the Game Panel you want to access. 0: Pause, 1: Win, 2: Lose
    /// </summary>
    /// <param name="panelID"></param>
    public void MovePanel(int panelID)
    {
        gamePanels[panelID].position = panelPosition;
        currentPanel = gamePanels[panelID];
    }

    public void PauseButtons()
    {
        resumeButton.position = resumePosition;
        buttons.position = buttonsPositions;
    }
    
    public void EndingButtons()
    {
        buttons.position = buttonsPositions;
    }

    public void ReturnToMenu()
    {
        gameManager.OnReturnToMenu();
    }

    public void OnPlayClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    #endregion
}
