using UnityEngine;
using VContainer;

namespace TowerTap
{
    public class GameManager : MonoBehaviour
    {
        private IEventBus _eventBus;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void StartGame()
        {
            _eventBus.Publish(new GameStartEvent());
        }

        public void EndGame()
        {
            _eventBus.Publish(new GameEndedEvent());
        }

        public void RestartGame()
        {
            _eventBus.Publish(new RestartGameEvent());
        }
    }
}