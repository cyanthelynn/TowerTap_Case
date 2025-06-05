using UnityEngine;

namespace GameConfig
{
    [CreateAssetMenu(fileName = "GameParameters", menuName = "Config/GameParameters")]
    public class GameParameters : ScriptableObject
    {
        public float blockHeight = 0.1f;
        public float movementRange = 3f;
        public float moveSpeed = 1.5f;
        public float minOverlapThreshold = 0.01f;
        public float perfectPlacementThreshold = 0.025f;
    
    
        [Header("Dynamic Difficulty Adjustment Settings")]
        public float minSpeed = 1.5f;
        public float maxSpeed = 4f;
        public int difficultyThreshold = 100;
        public AnimationCurve difficultyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }
}