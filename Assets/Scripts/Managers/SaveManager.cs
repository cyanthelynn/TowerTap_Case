using System.IO;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using VContainer;

public class SaveManager : MonoBehaviour
{
    [SerializeField, Required] private GameData _gameData;

    private IEventBus _eventBus;
    private string _savePath;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
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
        if (_gameData == null) return;
        EnsureSavePath();
        string json = JsonUtility.ToJson(_gameData);
        File.WriteAllText(_savePath, json);
    }

    private void Load()
    {
        if (_gameData == null) return;
        EnsureSavePath();
        if (!File.Exists(_savePath)) return;
        string json = File.ReadAllText(_savePath);
        JsonUtility.FromJsonOverwrite(json, _gameData);
    }

    private void EnsureSavePath()
    {
        if (string.IsNullOrEmpty(_savePath))
        {
            _savePath = Path.Combine(Application.persistentDataPath, "save.txt");
        }
    }

    [Button]
    private void ResetGameData()
    {
        if (_gameData == null) return;

        _gameData.highScore = 0;
        _gameData.isSoundActive = true;

        if (Application.isPlaying)
        {
            Save();
            Load();
        }
#if UNITY_EDITOR
        else
        {
            EditorUtility.SetDirty(_gameData);
            AssetDatabase.SaveAssets();
            EnsureSavePath();
            string json = JsonUtility.ToJson(_gameData);
            File.WriteAllText(_savePath, json);
        }
#endif
    }
}
