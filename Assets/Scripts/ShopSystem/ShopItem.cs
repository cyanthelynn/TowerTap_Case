using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI collectedText;
    [SerializeField] private Button equipButton;
    [SerializeField] private TextMeshProUGUI equippedText;
    [SerializeField] private RectTransform priceTextParent;

    private int _itemIndex;
    private int _price;
    private bool _isCollected;
    private bool _isEquipped;
    private Action<int> _onPurchase;
    private Action<int> _onEquip;
    
    public void Setup(
        int itemIndex,
        Sprite icon,
        int price,
        bool isCollected,
        bool isEquipped,
        Action<int> onPurchaseCallback,
        Action<int> onEquipCallback,
        int currentCurrency)
    {
        _itemIndex    = itemIndex;
        _price        = price;
        _isCollected  = isCollected;
        _isEquipped   = isEquipped;
        _onPurchase   = onPurchaseCallback;
        _onEquip      = onEquipCallback;
        
        iconImage.sprite = icon;
        priceText.text   = price.ToString();
        
        if (!_isCollected)
        {
            purchaseButton.gameObject.SetActive(true);
            collectedText.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(false);
            equippedText.gameObject.SetActive(false);

            purchaseButton.interactable = (currentCurrency >= price);
            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        }
        
        else if (_isCollected && !_isEquipped)
        {
            purchaseButton.gameObject.SetActive(false);
            priceText.gameObject.SetActive(false);

            collectedText.gameObject.SetActive(true);
            collectedText.text = "OWNED"; 
            priceTextParent.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(true);
            equippedText.gameObject.SetActive(false);

            equipButton.interactable = true;
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(OnEquipClicked);
        }
        
        else
        {
            purchaseButton.gameObject.SetActive(false);
            priceText.gameObject.SetActive(false);
            collectedText.gameObject.SetActive(false);

            equipButton.gameObject.SetActive(false);
            equippedText.gameObject.SetActive(true);
            priceTextParent.gameObject.SetActive(false);
            equippedText.text = "EQUIPPED";
        }
    }

    private void OnPurchaseClicked()
    {
        purchaseButton.interactable = false;
        _onPurchase?.Invoke(_itemIndex);
        priceTextParent.gameObject.SetActive(false);
    }

    private void OnEquipClicked()
    {
        equipButton.interactable = false;
        _onEquip?.Invoke(_itemIndex);
        priceTextParent.gameObject.SetActive(false);
    }
    
    public void UpdatePurchaseInteractable(int currentCurrency)
    {
        if (!_isCollected)
        {
            purchaseButton.interactable = (currentCurrency >= _price);
        }
    }
}
