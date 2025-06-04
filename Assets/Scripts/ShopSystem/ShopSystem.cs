// ShopSystem.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Managers;

public class ShopSystem : MonoBehaviour
{
    [Header("Shop Data Asset")]
    [SerializeField] private ShopData shopData;

    [Header("Shop UI")]
    [SerializeField] private ShopItem shopItemPrefab;
    [SerializeField] private Transform shopListParent;

    [Inject] private IEventBus _eventBus;
    [Inject] private GameData _gameData;
    [Inject] private UIManager _uiManager;

    // index → ilgili ShopItem
    private Dictionary<int, ShopItem> _spawnedItems = new Dictionary<int, ShopItem>();

    private void OnEnable()
    {
        _eventBus.Subscribe<DataChangedEvent>(OnDataChanged);
    }

    private void OnDisable()
    {
        _eventBus.Unsubscribe<DataChangedEvent>(OnDataChanged);
    }

    private void Start()
    {
        GenerateShopUI();
    }
    
    private void GenerateShopUI()
    {
        foreach (Transform child in shopListParent)
            Destroy(child.gameObject);

        _spawnedItems.Clear();

        for (int i = 0; i < shopData.shopDefinitions.Count; i++)
        {
            var def = shopData.shopDefinitions[i];
            bool isCollected = _gameData.collectedShopItems.Contains(i);
            var item = Instantiate(shopItemPrefab, shopListParent);

            // Setup sırasında currentCurrency bilgisi aktarılıyor
            item.Setup(
                i,
                def.icon,
                def.price,
                isCollected,
                OnShopPurchase,
                _gameData.gameCurrency
            );

            _spawnedItems[i] = item;
        }
    }

    private void OnShopPurchase(int index)
    {
        var def = shopData.shopDefinitions[index];
        if (_gameData.gameCurrency < def.price) return;

        _gameData.gameCurrency -= def.price;
        _gameData.collectedShopItems.Add(index);

        // Veriyi güncelleme eventi tetikle
        _eventBus.Publish(new DataChangedEvent());
        _uiManager.UpdateGameCurrencyUI(_gameData.gameCurrency);

        // Bu öğe artık satın alındı → butonu kapatıp “collected” yazısını göster
        if (_spawnedItems.TryGetValue(index, out var item))
        {
            item.Setup(
                index,
                def.icon,
                def.price,
                true,
                OnShopPurchase,
                _gameData.gameCurrency
            );
        }
    }

    private void OnDataChanged(DataChangedEvent evt)
    {
        // Her öğe için, güncel para miktarını ileterek buton etkileşimini güncelle
        foreach (var kvp in _spawnedItems)
        {
            int idx = kvp.Key;
            var def = shopData.shopDefinitions[idx];
            var item = kvp.Value;

            bool isCollected = _gameData.collectedShopItems.Contains(idx);
            // Eğer zaten satın alınmamışsa, mevcut para miktarıyla kontrol et
            if (!isCollected)
            {
                item.UpdateInteractable(_gameData.gameCurrency);
            }
        }
    }
}
