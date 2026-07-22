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
    public List<RectTransform> gamePanels; //0: Pause, 1: Win, 2: Lose
    public TextMeshProUGUI enemiesLeft;
    public RectTransform resumeButton;
    public RectTransform resetButton;
    public RectTransform menuButton;

    #region Variables for Canvas Game Objects
    private Vector3 panelOffScreen = new Vector3(2000f, 0f, 0f);
    private Vector3 resumeButtonPosition = new Vector3(0f, 180f, 0f);
    private Vector3 resumeButtonOffScreen = new Vector3(0f, -275f, 0f);
    private Vector3 resetButtonPosition = new Vector3(0f, 120f, 0f);
    private Vector3 resetButtonOffScreen = new Vector3(0f, -275f, 0f);
    private Vector3 menuButtonPosition = new Vector3(0f, 60f, 0f);
    private Vector3 menuButtonOffScreen = new Vector3(0f, -275f, 0f);
    private RectTransform currentPanel;
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
        if (SceneManager.GetActiveScene().name == "Game")
        {
            CustomUpdateManager.Instance.Register(this);
            EventBus.Subscribe<OnWaveInit>(OnWaveInit);
            EventBus.Subscribe<UpdateTextEvent>(UpdateText);
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

    private void UnregisterEntity()
    {
        CustomUpdateManager.Instance.Unregister(this);
    }

    #region OnButtonClicked Methods 
    public void ResumeGame()
    {
        gameManager.OnResumeGame();
        resumeButton.anchoredPosition = resumeButtonOffScreen;
        resetButton.anchoredPosition = resetButtonOffScreen;
        menuButton.anchoredPosition = menuButtonOffScreen;
        currentPanel.anchoredPosition = panelOffScreen;
        currentPanel = null;
    }

    public void ResetGame()
    {
        UnregisterEntity();
        gameManager.OnResetGame();
    }

    public void ReturnToMenu()
    {
        UnregisterEntity();
        gameManager.OnReturnToMenu();
    }

    /// <summary>
    /// Refers to the Game Panel you want to access. 0: Pause, 1: Win, 2: Lose
    /// </summary>
    /// <param name="panelID"></param>
    public void MovePanel(int panelID)
    {
        gamePanels[panelID].anchoredPosition = Vector3.zero;
        currentPanel = gamePanels[panelID];
    }

    public void PauseButtons()
    {
        resumeButton.anchoredPosition = resumeButtonPosition;
        resetButton.anchoredPosition = resetButtonPosition;
        menuButton.anchoredPosition = menuButtonPosition;
    }
    
    public void EndingButtons()
    {
        resetButton.anchoredPosition = resetButtonPosition;
        menuButton.anchoredPosition = menuButtonPosition;
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
