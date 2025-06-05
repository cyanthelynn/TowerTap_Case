using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VContainer;
using Managers;
using Pooling;

public class ShopSystem : MonoBehaviour
{
    [Header("Shop Data Asset")]
    [SerializeField] private ShopData shopData;

    [Header("Shop UI")]
    [SerializeField] private ShopItem shopItemPrefab;
    [SerializeField] private Transform shopListParent;

    [Header("Preview Settings")]
    [SerializeField] private GameObject blockPreviewPrefab;
    [SerializeField] private Transform previewParent;
    
    [Inject] private IEventBus _eventBus;
    [Inject] private GameData.GameData _gameData;
    [Inject] private UIManager _uiManager;
    [Inject] private BlockPoolManager _blockPoolManager;

    private Dictionary<int, ShopItem> _spawnedItems = new Dictionary<int, ShopItem>();
    private GameObject _previewInstance;

    private void OnEnable()
    {
        _eventBus.Subscribe<DataChangedEvent>(OnDataChanged);
        _eventBus.Subscribe<StartFromMainMenuEvent>(StartGameEvent);
        _eventBus.Subscribe<BackMainMenuEvent>(BackToMenu);
    }
    private void OnDisable()
    {
        _eventBus.Unsubscribe<DataChangedEvent>(OnDataChanged);
        _eventBus.Unsubscribe<StartFromMainMenuEvent>(StartGameEvent);
        _eventBus.Unsubscribe<BackMainMenuEvent>(BackToMenu);
    }

    private void BackToMenu(BackMainMenuEvent obj)
    {
        if (_previewInstance != null)
            _previewInstance.SetActive(true);
    }

    private void StartGameEvent(StartFromMainMenuEvent obj)
    {
        if (_previewInstance != null)
            _previewInstance.SetActive(false);
    }

    private void Start()
    {
        GenerateShopUI();
        InstantiatePreview();
        ApplyEquippedColorToPreview();
    }

    private void GenerateShopUI()
    {
        RefreshShopList();

        for (int i = 0; i < shopData.shopDefinitions.Count; i++)
        {
            var def = shopData.shopDefinitions[i];
            bool isCollected = _gameData.collectedShopItems.Contains(i);
            bool isEquipped  = (_gameData.selectedSkinIndex == i);

            var item = Instantiate(shopItemPrefab, shopListParent);
            item.Setup(
                i,
                def.price,
                def.color, 
                isCollected,
                isEquipped,
                OnShopPurchase,
                OnShopEquip,
                _gameData.gameCurrency
            );
            _spawnedItems[i] = item;
        }
    }

    private void RefreshShopList()
    {
        foreach (Transform child in shopListParent)
            Destroy(child.gameObject);
        _spawnedItems.Clear();
    }

    private void OnShopPurchase(int index)
    {
        var def = shopData.shopDefinitions[index];
        if (_gameData.gameCurrency < def.price) 
            return;

        _gameData.gameCurrency -= def.price;
        _gameData.collectedShopItems.Add(index);
        _eventBus.Publish(new DataChangedEvent());
        _uiManager.UpdateGameCurrencyUI(_gameData.gameCurrency);
        
        if (_spawnedItems.TryGetValue(index, out var item))
        {
            item.Setup(index, def.price, def.color, true, false,  
                OnShopPurchase,
                OnShopEquip,
                _gameData.gameCurrency
            );
        }
    }

    private void OnShopEquip(int index)
    {
        _gameData.selectedSkinIndex = index;
        _eventBus.Publish(new DataChangedEvent());
        _uiManager.UpdateGameCurrencyUI(_gameData.gameCurrency);
        
        _eventBus.Publish(new BlockSkinChangedEvent());
        
        ApplyEquippedColorToPreview();
        
        GenerateShopUI();
    }

    private void OnDataChanged(DataChangedEvent evt)
    {
        foreach (var keyValuePair in _spawnedItems)
        {
            int index = keyValuePair.Key;
            var item = keyValuePair.Value;
            bool isCollected = _gameData.collectedShopItems.Contains(index);
            
            if (!isCollected)
            {
                item.UpdatePurchaseInteractable(_gameData.gameCurrency);
            }
        }
    }

    private void InstantiatePreview()
    {
        if (blockPreviewPrefab == null || previewParent == null) return;

        if (_previewInstance != null)
            Destroy(_previewInstance);
        
        _previewInstance = Instantiate(
            blockPreviewPrefab,
            previewParent.position,
            previewParent.rotation,
            previewParent
        );
        _previewInstance.transform.localPosition = new Vector3(0, 2, 0);
        _previewInstance.transform.localRotation = Quaternion.identity;
        
        _previewInstance.transform.DORotate(
            new Vector3(0f, 360f, 0f),
            8f,
            RotateMode.FastBeyond360
        ).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    private void ApplyEquippedColorToPreview()
    {
        if (_previewInstance == null) return;
        int equipIndex = _gameData.selectedSkinIndex;
        if (equipIndex < 0 || equipIndex >= shopData.shopDefinitions.Count) return;

        var def = shopData.shopDefinitions[equipIndex];
        var renderers = _previewInstance.GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            rend.material.color = def.color;
        }
    }
    
}
