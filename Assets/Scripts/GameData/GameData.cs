using System.Collections.Generic;
using MissionSystem;
using UnityEngine;

namespace TowerTap
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Config/GameData")]
    public class GameData : ScriptableObject
    {
        public int highScore;
        public bool isSoundActive;
        public bool isHapticOn;
    
        public int gameCurrency;
    
        [Space]
        [Header("PERFECT & COMBO COUNTS")]
        public int totalPerfectCount;
        public int maxComboCount;
        [Space]
    
    
        [Header("MISSION DATA")]
        [Space]
        public string lastMissionResetDateString; // “yyyy-MM-dd”
        public List<MissionProgress> activeMissions = new List<MissionProgress>();
        public List<MissionProgress> completedMissions = new List<MissionProgress>();
    
    
        [Space]
        [Header("SHOP DATA")]
        [Space]
        public List<int> collectedShopItems = new List<int>();
        public int selectedSkinIndex = -1; 
    }
}
