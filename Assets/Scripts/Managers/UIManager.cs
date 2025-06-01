using Managers;
using TMPro;
using UnityEngine;
using VContainer;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText;

    private IEventBus _eventBus;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    private void OnEnable()
    {
        _eventBus.Subscribe<GameStartEvent>(OnGameStart);
        _eventBus.Subscribe<GameEndedEvent>(OnGameEnded);
        _eventBus.Subscribe<BlockPlacedEvent>(OnBlockPlaced);
    }

    private void OnDisable()
    {
        _eventBus.Unsubscribe<GameStartEvent>(OnGameStart);
        _eventBus.Unsubscribe<GameEndedEvent>(OnGameEnded);
        _eventBus.Unsubscribe<BlockPlacedEvent>(OnBlockPlaced);
    }

    private void OnGameStart(GameStartEvent evt)
    {
        ScoreManager.ResetScore();
        hudPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        UpdateScoreUI();
    }

    private void OnGameEnded(GameEndedEvent evt)
    {
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private void OnBlockPlaced(BlockPlacedEvent evt)
    {
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = ScoreManager.CurrentScore.ToString();
    }
}