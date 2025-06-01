using Managers;
using UnityEngine;
using VContainer;

public class ScoreManager : MonoBehaviour
{
    public static int CurrentScore { get; private set; }

    private IEventBus _eventBus;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    private void OnEnable()
    {
        _eventBus.Subscribe<BlockPlacedEvent>(OnBlockPlaced);
    }

    private void OnDisable()
    {
        _eventBus.Unsubscribe<BlockPlacedEvent>(OnBlockPlaced);
    }

    private void OnBlockPlaced(BlockPlacedEvent evt)
    {
        CurrentScore++;
    }

    public static void ResetScore()
    {
        CurrentScore = 0;
    }
}