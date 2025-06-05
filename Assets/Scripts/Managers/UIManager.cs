using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace TowerTap
{
    public class UIManager : MonoBehaviour
    {
        [Space]
        [Header("GameObjects REF's")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject tapToStartPanel;
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject shopMenuPanel;
        [Space]
        [Header("BUTTONS REF's")]
        [SerializeField] private Button tapToStartButton;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button closeShopButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button reloadLevelButton;
        [SerializeField] private Button missionMenuButton;
        [SerializeField] private Button missionMenuCloseButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button backToMainMenuButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button highScoreButton;
    
        [Space]
        [Header("TextMeshPro REF's")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI gameCurrencyText;
        [SerializeField] private TextMeshProUGUI shopMenuCurrencyText;
        [SerializeField] private TextMeshProUGUI gameOverScoreText;
        [SerializeField] private TextMeshProUGUI gameOverHighScoreText;
   
        [Space]
        [Header("RectTransform REF's")]
        [SerializeField] private RectTransform highScoreRect;
        [SerializeField] private RectTransform settingsRect;
        [SerializeField] private RectTransform missionMenuRect;

        private bool _isHighScoreOpen;
        private bool _isSettingsOpen;
        private IEventBus _eventBus;
        private GameManager _gameManager;
        [Inject]
        public void Construct(IEventBus eventBus,GameManager gameManager)
        {
            _eventBus = eventBus;
            _gameManager = gameManager;
        }

        private void OnEnable()
        {
            tapToStartButton.onClick.AddListener(TapToStart);
            highScoreButton.onClick.AddListener(HighScoreToggle);
            backToMainMenuButton.onClick.AddListener(BackToMainMenuInGame);
            missionMenuButton.onClick.AddListener(OpenMissionMenu);
            shopButton.onClick.AddListener(OpenShopMenu);
            mainMenuButton.onClick.AddListener(BackMainMenu);
            closeShopButton.onClick.AddListener(CloseShopMenu);
            startGameButton.onClick.AddListener(MenuStartGame);
            missionMenuCloseButton.onClick.AddListener(MissionMenuClose);
            settingsButton.onClick.AddListener(SettingsToggle);
            restartButton.onClick.AddListener(RestartGame);
            reloadLevelButton.onClick.AddListener(ReloadGame);
            _eventBus.Subscribe<GameStartEvent>(OnGameStart);
            _eventBus.Subscribe<GameEndedEvent>(OnGameEnded);
            _eventBus.Subscribe<RestartGameEvent>(OnGameRestarted);
        }
        private void OnDisable()
        {
            tapToStartButton.onClick.RemoveListener(TapToStart);
            highScoreButton.onClick.RemoveListener(HighScoreToggle);
            missionMenuButton.onClick.RemoveListener(OpenMissionMenu);
            shopButton.onClick.RemoveListener(OpenShopMenu);
            backToMainMenuButton.onClick.RemoveListener(BackToMainMenuInGame);
            mainMenuButton.onClick.RemoveListener(BackMainMenu);
            closeShopButton.onClick.RemoveListener(CloseShopMenu);
            startGameButton.onClick.RemoveListener(MenuStartGame);
            missionMenuCloseButton.onClick.RemoveListener(MissionMenuClose);
            settingsButton.onClick.RemoveListener(SettingsToggle);
            restartButton.onClick.RemoveListener(RestartGame);
            reloadLevelButton.onClick.RemoveListener(ReloadGame);
            _eventBus.Unsubscribe<GameStartEvent>(OnGameStart);
            _eventBus.Unsubscribe<GameEndedEvent>(OnGameEnded);
            _eventBus.Unsubscribe<RestartGameEvent>(OnGameRestarted);
        }

        private void BackToMainMenuInGame()
        {
            BackMainMenu();
        }

        private void HighScoreToggle()
        {
            if (!_isHighScoreOpen)
            {
                _isHighScoreOpen = true;
                highScoreRect.DOAnchorPosX(0, 1).SetEase(Ease.OutBounce);
            }
            else
            {
            
                highScoreRect.DOAnchorPosX(230, 1).SetEase(Ease.OutBounce).OnComplete((() => 
                    _isHighScoreOpen = false ));
            }
        }
        private void OpenMissionMenu()
        {
            missionMenuRect.DOAnchorPosX(-450, 1).SetEase(Ease.OutBounce);
            missionMenuButton.GetComponent<RectTransform>().DOAnchorPosX(200,1).SetEase(Ease.OutBounce);
        }
        private void MissionMenuClose()
        {
            missionMenuRect.DOAnchorPosX(0, 1).SetEase(Ease.OutBounce);
            missionMenuButton.GetComponent<RectTransform>().DOAnchorPosX(-7,1).SetEase(Ease.OutBounce);
        }
        private void SettingsToggle()
        {
            if (!_isSettingsOpen)
            {
                _isSettingsOpen = true;
                settingsRect.DOScaleY(2, 1).SetEase(Ease.OutBounce);
            }
            else
            {
            
                settingsRect.DOScaleY(0, 1).SetEase(Ease.OutBounce).OnComplete((() => 
                    _isSettingsOpen = false ));
            }
        }
    
        private void BackMainMenu()
        {
            CloseGameHUD();
            CloseShopMenu();
            missionMenuRect.gameObject.SetActive(false);
            gameOverPanel.SetActive(false);
            _eventBus.Publish(new BackMainMenuEvent());
        }

        private void OpenShopMenu()
        {
            mainMenuPanel.gameObject.SetActive(false);
            shopMenuPanel.SetActive(true);
        }
        private void CloseShopMenu()
        {
            mainMenuPanel.gameObject.SetActive(true);
            shopMenuPanel.SetActive(false);
        }

        private void CloseGameHUD()
        {
            hudPanel.SetActive(false);
        }
        private void MenuStartGame()
        {
            _eventBus.Publish(new StartFromMainMenuEvent());
            mainMenuPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            tapToStartPanel.transform.gameObject.SetActive(true);
        }

        private void ReloadGame()
        {
            _eventBus.Publish(new RestartGameEvent());
        }

        private void OnGameRestarted(RestartGameEvent evt)
        {
            gameOverPanel.SetActive(false);
            hudPanel.SetActive(false);
            tapToStartPanel.transform.gameObject.SetActive(true);
        }

        private void TapToStart()
        {
            tapToStartPanel.SetActive(false);
            _gameManager.StartGame();
        }
        private void RestartGame()
        {
            _gameManager.RestartGame();
      
        }
        private void OnGameStart(GameStartEvent evt)
        {
            hudPanel.SetActive(true);
            missionMenuRect.gameObject.SetActive(true);
            ResetScoreUI();
            gameOverPanel.SetActive(false);
        }

        private void OnGameEnded(GameEndedEvent evt)
        {
            hudPanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }
    
        public void UpdateScoreUI(int currentScore)
        {
            scoreText.text = currentScore.ToString();
        }
        public void UpdateHighScoreUI(int currentHighScore)
        {
            highScoreText.text = currentHighScore.ToString();
        }

        public void UpdateGameCurrencyUI(int currentGameCurrency)
        {
            gameCurrencyText.text = currentGameCurrency.ToString();
            shopMenuCurrencyText.text = currentGameCurrency.ToString();
        }

        public void UpdateGameOverTextsUI(int currentScore, int currentHighScore)
        {
            gameOverScoreText.text = currentScore.ToString();
            gameOverHighScoreText.text = currentHighScore.ToString();
        }
        private void ResetScoreUI()
        {
            scoreText.text = "0";
        }
    
        private void ReloadCurrentScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    
    }
}