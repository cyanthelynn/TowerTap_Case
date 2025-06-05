using GameConfig;
using UnityEngine;
using VContainer;

namespace TowerTap
{
    public class DifficultyManager : MonoBehaviour
    {
        [Inject] private GameParameters parameters;
        [Inject] private ScoreManager _scoreManager;
        [Inject] private IEventBus _eventBus;

        private void OnEnable()
        {
            _eventBus.Subscribe<BlockPlacedEvent>(OnBlockPlaced);
            _eventBus.Subscribe<PerfectPlacementEvent>(OnPerfectPlaced);
            _eventBus.Subscribe<GameStartEvent>(OnGameStarted);
        }
    
        private void OnGameStarted(GameStartEvent evt)
        {
            parameters.moveSpeed = parameters.minSpeed;
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<BlockPlacedEvent>(OnBlockPlaced);
            _eventBus.Unsubscribe<PerfectPlacementEvent>(OnPerfectPlaced);
            _eventBus.Unsubscribe<GameStartEvent>(OnGameStarted);
        }

        private void OnBlockPlaced(BlockPlacedEvent evt)
        {
            AdjustDifficulty();
        }
        private void OnPerfectPlaced(PerfectPlacementEvent evt)
        {
            AdjustDifficulty();
        }

        private void AdjustDifficulty()
        {
            float t = Mathf.Clamp01((float)_scoreManager.CurrentScore / parameters.difficultyThreshold);
            float curveValue = parameters.difficultyCurve.Evaluate(t);
            parameters.moveSpeed = Mathf.Lerp(parameters.minSpeed, parameters.maxSpeed, curveValue);
        }
    }
}