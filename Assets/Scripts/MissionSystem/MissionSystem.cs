using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using Managers;

public class MissionSystem : MonoBehaviour
{
    [Header("Mission Data Asset")]
    [SerializeField] private MissionData missionData;

    [Header("Mission UI")]
    [SerializeField] private MissionItem missionItemPrefab;
    [SerializeField] private Transform missionListParent;

    [Header("Daily Settings")]
    [SerializeField] private int maxActiveMissions = 3;

    [Inject] private IEventBus _eventBus;
    [Inject] private SaveManager _saveManager;
    [Inject] private ScoreManager _scoreManager;
    [Inject] private GameData _gameData;

    private List<MissionProgress> _activeMissions;
    private List<MissionProgress> _completedMissions;
    private HashSet<Mission> _missionSet;

    private void Awake()
    {
        _missionSet = new HashSet<Mission>(missionData.missionDefinitions);

        if (_gameData.activeMissions == null)
            _gameData.activeMissions = new List<MissionProgress>();
        if (_gameData.completedMissions == null)
            _gameData.completedMissions = new List<MissionProgress>();

        _activeMissions = _gameData.activeMissions;
        _completedMissions = _gameData.completedMissions;
    }
   
    private void Start()
    {
        CheckAndResetDailyMissions();
        UpdateMissionUI();
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void CheckAndResetDailyMissions()
    {
        DateTime lastReset = string.IsNullOrEmpty(_gameData.lastMissionResetDateString)
            ? DateTime.MinValue
            : DateTime.Parse(_gameData.lastMissionResetDateString);
        
        if (lastReset.Date < DateTime.Today)
        {
            _activeMissions.Clear();
            _completedMissions.Clear();

            var rnd = new System.Random();
            var randomThree = _missionSet.OrderBy(_ => rnd.Next())
                .Take(maxActiveMissions)
                .ToList();

            foreach (var m in randomThree)
            {
                var mp = new MissionProgress
                {
                    missionId = m.id,
                    AssignedDate = DateTime.Today,
                    isClaimed = false
                };
                _activeMissions.Add(mp);
            }

            _gameData.lastMissionResetDateString = DateTime.Today.ToString("yyyy-MM-dd");
            _saveManager.Save();
        }
    }



    private void SubscribeToEvents()
    {
        _eventBus.Subscribe<PerfectPlacementEvent>(OnPerfectPlacement);
        _eventBus.Subscribe<BlockPlacedEvent>(OnBlockPlaced);
    }
    
    private void UnsubscribeFromEvents()
    {
        _eventBus.Unsubscribe<PerfectPlacementEvent>(OnPerfectPlacement);
        _eventBus.Unsubscribe<BlockPlacedEvent>(OnBlockPlaced);
    }

    private void OnPerfectPlacement(PerfectPlacementEvent evt)
    {
        UpdateMissionCompletionFlags();
    }

    private void OnBlockPlaced(BlockPlacedEvent evt)
    {
        UpdateMissionCompletionFlags();
    }

    private void UpdateMissionCompletionFlags()
    {
        bool anyChange = false;
        foreach (var mp in _activeMissions)
        {
            var def = missionData.missionDefinitions.First(m => m.id == mp.missionId);
            bool isComplete = 
                _gameData.totalPerfectCount >= def.needPerfectCount &&
                _gameData.maxComboCount   >= def.needComboCount &&
                !mp.isClaimed;

            if (isComplete)
            {
                anyChange = true;
            }
        }

        if (anyChange)
            UpdateMissionUI();
    }

    private void UpdateMissionUI()
    {
        foreach (Transform child in missionListParent)
            Destroy(child.gameObject);

        foreach (var mp in _activeMissions)
        {
            var def = missionData.missionDefinitions.First(m => m.id == mp.missionId);
            bool isComplete = 
                _gameData.totalPerfectCount >= def.needPerfectCount &&
                _gameData.maxComboCount   >= def.needComboCount &&
                !mp.isClaimed;

            var item = Instantiate(missionItemPrefab, missionListParent);
            item.Setup(def, isComplete, mp.isClaimed, OnMissionClaimed);
        }
    }

    private void OnMissionClaimed(string missionId)
    {
        int idx = _activeMissions.FindIndex(x => x.missionId == missionId);
        if (idx < 0) return;

        var mp = _activeMissions[idx];
        var def = missionData.missionDefinitions.First(m => m.id == missionId);

        _gameData.gameCurrency += def.rewardAmount;

        mp.isClaimed = true;
        _completedMissions.Add(mp);
        _activeMissions.RemoveAt(idx);

        UpdateMissionUI();
        AssignNextMissionIfNeeded(); 
        _eventBus.Publish(new OnMissionClaimed());
        _saveManager.Save();
    }

    private void AssignNextMissionIfNeeded()
    {
        if (_activeMissions.Count >= maxActiveMissions) return;

        var next = missionData.missionDefinitions
            .Where(m => !_completedMissions.Any(cp => cp.missionId == m.id))
            .Where(m => !_activeMissions.Any(ap => ap.missionId == m.id))
            .FirstOrDefault();

        if (!string.IsNullOrEmpty(next.id))
        {
            var mp = new MissionProgress
            {
                missionId = next.id,
                AssignedDate = DateTime.Today,
                isClaimed = false
            };
            _activeMissions.Add(mp);
            UpdateMissionUI();
        }
    }
}

[Serializable]
public struct Mission
{
    public string id;
    public string description;
    public int needPerfectCount;
    public int needComboCount;
    public int rewardAmount;
}

[Serializable]
public struct MissionProgress
{
    public string missionId;
    public string assignedDateString;
    public bool isClaimed;

    public DateTime AssignedDate
    {
        get => DateTime.TryParse(assignedDateString, out var d) ? d : DateTime.MinValue;
        set => assignedDateString = value.ToString("yyyy-MM-dd");
    }
}
