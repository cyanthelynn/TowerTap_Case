using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Config/GameData")]
public class GameData : ScriptableObject
{
    public int highScore;
    public bool isSoundActive;
    
    public int gameCurrency;
    public int totalPerfectCount;
    public int maxComboCount;
    
    public string lastMissionResetDateString; // “yyyy-MM-dd”
    public List<MissionProgress> activeMissions = new List<MissionProgress>();
    public List<MissionProgress> completedMissions = new List<MissionProgress>();
}
