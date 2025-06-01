using System.IO;
using Managers;
using UnityEngine;
using VContainer;

public class SaveManager : MonoBehaviour
{
    private IEventBus _eventBus;
    private GameData _gameData;
    private string _savePath;

    [Inject]
    public void Construct(IEventBus eventBus, GameData gameData)
    {
        _eventBus = eventBus;
        _gameData = gameData;
    }

    private void Awake()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "save.txt");
    }

    private void OnEnable()
    {
        _eventBus.Subscribe<GameStartEvent>(OnGameStart);
        _eventBus.Subscribe<GameEndedEvent>(OnGameEnded);
    }

    private void OnDisable()
    {
        _eventBus.Unsubscribe<GameStartEvent>(OnGameStart);
        _eventBus.Unsubscribe<GameEndedEvent>(OnGameEnded);
    }

    private void OnGameStart(GameStartEvent evt)
    {
        Load();
    }

    private void OnGameEnded(GameEndedEvent evt)
    {
        Save();
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(_gameData);
        File.WriteAllText(_savePath, json);
    }

    private void Load()
    {
        if (!File.Exists(_savePath)) return;
        string json = File.ReadAllText(_savePath);
        JsonUtility.FromJsonOverwrite(json, _gameData);
    }
}