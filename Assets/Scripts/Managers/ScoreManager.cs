using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

public class ScoreManager : MonoBehaviour
{
    public int CurrentScore { get; private set; }
    public int CurrentComboCount => _currentComboCount;

    private int _currentComboCount;
    private IEventBus _eventBus;
    private UIManager _uiManager;
    [SerializeField, Required] private GameData _gameData;

    [Inject]
    public void Construct(IEventBus eventBus,UIManager uiManager)
    {
        _eventBus = eventBus;
        _uiManager = uiManager;
    }

    private void OnEnable()
    {
        _eventBus.Subscribe<BlockPlacedEvent>(OnBlockPlaced);
        _eventBus.Subscribe<PerfectPlacementEvent>(OnPerfectPlacement);
        _eventBus.Subscribe<GameEndedEvent>(OnGameEnded);
        _eventBus.Subscribe<GameStartEvent>(OnGameStart);
    }

    private void OnDisable()
    {
        _eventBus.Unsubscribe<BlockPlacedEvent>(OnBlockPlaced);
        _eventBus.Unsubscribe<PerfectPlacementEvent>(OnPerfectPlacement);
        _eventBus.Unsubscribe<GameEndedEvent>(OnGameEnded);
        _eventBus.Unsubscribe<GameStartEvent>(OnGameStart);
    }

    private void OnBlockPlaced(BlockPlacedEvent evt)
    {
        CurrentScore++;
        _currentComboCount = 0;
        _uiManager.UpdateScoreUI(CurrentScore);
    }

    private void OnPerfectPlacement(PerfectPlacementEvent evt)
    {
        CurrentScore++;
        _uiManager.UpdateScoreUI(CurrentScore);
        _currentComboCount = (_currentComboCount > 0) ? _currentComboCount+1 : 1;
    }

    private void OnGameEnded(GameEndedEvent evt)
    {
        if (CurrentScore > _gameData.highScore)
        {
            _gameData.highScore = CurrentScore;
        }
        
        _uiManager.UpdateHighScoreUI(_gameData.highScore);
    }

    private void OnGameStart(GameStartEvent evt)
    {
        CurrentScore = 0;
        _currentComboCount = 0;
        _uiManager.UpdateHighScoreUI(_gameData.highScore);
    }
   
    public int GetCurrentHighScoreValue()
    {
        return _gameData.highScore;
    }
}