using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject tapToStartPanel;
    [SerializeField] private Button tapToStartButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [SerializeField] private Button restartButton;

    private IEventBus _eventBus;
    private GameManager _gameManager;
    [Inject]
    public void Construct(IEventBus eventBus,GameManager gameManager)
    {
        _eventBus = eventBus;
        _gameManager = gameManager;
    }

    private void OnEnable()
    {
        tapToStartButton.onClick.AddListener(TapToStart);
        restartButton.onClick.AddListener(RestartGame);
        _eventBus.Subscribe<GameStartEvent>(OnGameStart);
        _eventBus.Subscribe<GameEndedEvent>(OnGameEnded);
        _eventBus.Subscribe<RestartGameEvent>(OnGameRestarted);
    }
    
    private void OnDisable()
    {
        tapToStartButton.onClick.RemoveListener(TapToStart);
        restartButton.onClick.RemoveListener(RestartGame);
        _eventBus.Unsubscribe<GameStartEvent>(OnGameStart);
        _eventBus.Unsubscribe<GameEndedEvent>(OnGameEnded);
        _eventBus.Unsubscribe<RestartGameEvent>(OnGameRestarted);
    }

    private void OnGameRestarted(RestartGameEvent evt)
    {
        gameOverPanel.SetActive(false);
        hudPanel.SetActive(false);
        tapToStartPanel.transform.gameObject.SetActive(true);
    }

    private void TapToStart()
    {
        tapToStartPanel.SetActive(false);
        _gameManager.StartGame();
    }
    private void RestartGame()
    {
      _gameManager.RestartGame();
      
    }
    private void OnGameStart(GameStartEvent evt)
    {
        hudPanel.SetActive(true);
        ResetScoreUI();
        gameOverPanel.SetActive(false);
    }

    private void OnGameEnded(GameEndedEvent evt)
    {
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
    
    public void UpdateScoreUI(int currentScore)
    {
        scoreText.text = currentScore.ToString();
    }
    public void UpdateHighScoreUI(int currentHighScore)
    {
        highScoreText.text = currentHighScore.ToString();
    }
    private void ResetScoreUI()
    {
        scoreText.text = "0";
    }
    
    private void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}