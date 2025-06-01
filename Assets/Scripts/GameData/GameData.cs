using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Config/GameData")]
public class GameData : ScriptableObject
{
    public int highScore;
    public bool isSoundActive;
    
    [Button]
    public void ResetGameData()
    {
        highScore = 0;
        isSoundActive = true;
    }
}
