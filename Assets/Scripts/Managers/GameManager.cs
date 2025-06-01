using Managers;
using UnityEngine;
using VContainer;

public class GameManager : MonoBehaviour
{
    private IEventBus _eventBus;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    private void Start()
    {
        _eventBus.Publish(new GameStartEvent());
    }

    public void EndGame()
    {
        _eventBus.Publish(new GameEndedEvent());
    }
}