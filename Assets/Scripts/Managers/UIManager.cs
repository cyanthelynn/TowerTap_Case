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
    private ScoreManager _scoreManager;
    private GameManager _gameManager;
    [Inject]
    public void Construct(IEventBus eventBus, ScoreManager scoreManager,GameManager gameManager)
    {
        _eventBus = eventBus;
        _scoreManager = scoreManager;
        _gameManager = gameManager;
    }

    private void OnEnable()
    {
        tapToStartButton.onClick.AddListener(TapToStart);
        restartButton.onClick.AddListener(RestartGame);
        _eventBus.Subscribe<GameStartEvent>(OnGameStart);
        _eventBus.Subscribe<GameEndedEvent>(OnGameEnded);
        _eventBus.Subscribe<RestartGameEvent>(OnGameRestarted);
        _eventBus.Subscribe<BlockPlacedEvent>(OnBlockPlaced);
    }

    private void OnDisable()
    {
        tapToStartButton.onClick.RemoveListener(TapToStart);
        restartButton.onClick.RemoveListener(RestartGame);
        _eventBus.Unsubscribe<GameStartEvent>(OnGameStart);
        _eventBus.Unsubscribe<GameEndedEvent>(OnGameEnded);
        _eventBus.Unsubscribe<RestartGameEvent>(OnGameRestarted);
        _eventBus.Unsubscribe<BlockPlacedEvent>(OnBlockPlaced);
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
        gameOverPanel.SetActive(false);
        UpdateScoreUI();
        UpdateHighScoreUI();
    }

    private void OnGameEnded(GameEndedEvent evt)
    {
        UpdateHighScoreUI();
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private void OnBlockPlaced(BlockPlacedEvent evt)
    {
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = _scoreManager.CurrentScore.ToString();
    }

    private void UpdateHighScoreUI()
    {
        highScoreText.text = _scoreManager.GetCurrentHighScoreValue().ToString();
    }

    private void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}