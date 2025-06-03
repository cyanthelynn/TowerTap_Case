using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Config/MissionData")]
public class MissionData : ScriptableObject
{
    public List<Mission> missionDefinitions = new List<Mission>();
}