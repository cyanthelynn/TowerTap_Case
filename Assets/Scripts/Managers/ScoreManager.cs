using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

public class ScoreManager : MonoBehaviour
{
    public int CurrentScore { get; private set; }

    private IEventBus _eventBus;
    [SerializeField,Required] private GameData _gameData;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    private void OnEnable()
    {
        _eventBus.Subscribe<BlockPlacedEvent>(OnBlockPlaced);
        _eventBus.Subscribe<GameEndedEvent>(OnGameEnded);
        _eventBus.Subscribe<GameStartEvent>(OnGameStart);
    }

    private void OnDisable()
    {
        _eventBus.Unsubscribe<BlockPlacedEvent>(OnBlockPlaced);
        _eventBus.Unsubscribe<GameEndedEvent>(OnGameEnded);
        _eventBus.Unsubscribe<GameStartEvent>(OnGameStart);
    }

    private void OnBlockPlaced(BlockPlacedEvent evt)
    {
        CurrentScore++;
    }

    private void OnGameEnded(GameEndedEvent evt)
    {
        if (CurrentScore > _gameData.highScore)
        {
            _gameData.highScore = CurrentScore;
        }
    }

    private void OnGameStart(GameStartEvent evt)
    {
        CurrentScore = 0;
    }
}