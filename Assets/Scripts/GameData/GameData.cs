using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Config/GameData")]
public class GameData : ScriptableObject
{
    public int highScore;
    public bool isSoundActive;
}
