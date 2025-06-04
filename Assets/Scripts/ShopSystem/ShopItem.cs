// ShopItem.cs
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI collectedText;

    private int _itemIndex;
    private int _price;
    private bool _isCollected;
    private Action<int> _onPurchase;

    public void Setup(
        int itemIndex,
        Sprite icon,
        int price,
        bool isCollected,
        Action<int> onPurchaseCallback,
        int currentCurrency)
    {
        _itemIndex    = itemIndex;
        _price        = price;
        _isCollected  = isCollected;
        _onPurchase   = onPurchaseCallback;

        iconImage.sprite = icon;
        priceText.text   = price.ToString();
        collectedText.gameObject.SetActive(isCollected);
        purchaseButton.gameObject.SetActive(!isCollected);

        // Eğer henüz toplanmadıysa, butonu currentCurrency >= price koşuluna göre ayarla
        if (!isCollected)
        {
            purchaseButton.interactable = (currentCurrency >= price);
        }
        else
        {
            purchaseButton.interactable = false;
        }

        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(OnPurchaseClicked);
    }

    private void OnPurchaseClicked()
    {
        purchaseButton.interactable = false;
        _onPurchase?.Invoke(_itemIndex);
    }

    // --- Eklenen metot: sadece butonun etkileşimini günceller ---
    public void UpdateInteractable(int currentCurrency)
    {
        if (_isCollected)
        {
            purchaseButton.interactable = false;
        }
        else
        {
            purchaseButton.interactable = (currentCurrency >= _price);
        }
    }
}