using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using VContainer;
#if UNITY_EDITOR
#endif

namespace Managers
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField, Required] private GameData.GameData _gameData;

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
            Load();
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<GameStartEvent>(OnGameStart);
            _eventBus.Subscribe<GameEndedEvent>(OnGameEnded);
            _eventBus.Subscribe<DataChangedEvent>(OnDataChanged);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<GameStartEvent>(OnGameStart);
            _eventBus.Unsubscribe<GameEndedEvent>(OnGameEnded);
            _eventBus.Unsubscribe<DataChangedEvent>(OnDataChanged);
        }

        private void OnDataChanged(DataChangedEvent obj)
        {
            Save();
        }

        private void OnGameStart(GameStartEvent evt)
        {
            Load();
        }

        private void OnGameEnded(GameEndedEvent evt)
        {
            Save();
        }

        public void Save()
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
            _gameData.isHapticOn = true;
            _gameData.gameCurrency = 0;
            _gameData.totalPerfectCount = 0;
            _gameData.maxComboCount = 0;
            _gameData.activeMissions.Clear();
            _gameData.completedMissions.Clear();
            _gameData.collectedShopItems.Clear();
            _gameData.selectedSkinIndex = -1;
            _gameData.lastMissionResetDateString = "";

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
                Debug.Log("SAVE DATA DELETED");
            }
#endif
        }
    }
}
