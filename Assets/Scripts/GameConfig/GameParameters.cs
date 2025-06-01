using UnityEngine;

[CreateAssetMenu(fileName = "GameParameters", menuName = "Config/GameParameters")]
public class GameParameters : ScriptableObject
{
    public float blockHeight = 0.1f;
    public float movementRange = 3f;
    public float moveSpeed = 2f;
    public float minOverlapThreshold = 0.01f;
    public float perfectPlacementThreshold = 0.025f;
}