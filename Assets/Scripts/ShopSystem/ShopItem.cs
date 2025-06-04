using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Image colorSwatch;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private RectTransform priceTextParent;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI collectedText;
    [SerializeField] private Button equipButton;
    [SerializeField] private TextMeshProUGUI equippedText;

    private int _itemIndex;
    private int _price;
    private Color _color;
    private bool _isCollected;
    private bool _isEquipped;
    private Action<int> _onPurchase;
    private Action<int> _onEquip;

    private enum ItemState
    {
        ForSale,  
        Owned,      
        Equipped    
    }

    public void Setup(int itemIndex, int price, Color color, bool isCollected, bool isEquipped, Action<int> onPurchaseCallback, Action<int> onEquipCallback,
        int currentCurrency)
    {
        _itemIndex   = itemIndex;
        _price       = price;
        _color       = color;
        _isCollected = isCollected;
        _isEquipped  = isEquipped;
        _onPurchase  = onPurchaseCallback;
        _onEquip     = onEquipCallback;

        ConfigureColor();
        ConfigurePriceText();
        ApplyState(DetermineState(), currentCurrency);
    }

    private ItemState DetermineState()
    {
        if (!_isCollected) return ItemState.ForSale;
        return _isEquipped ? ItemState.Equipped : ItemState.Owned;
    }

    private void ConfigureColor()
    {
        var c = _color;
        c.a = 1f;
        colorSwatch.color = c;
    }

    private void ConfigurePriceText()
    {
        priceText.text = _price.ToString();
        priceTextParent.gameObject.SetActive(true);
    }

    private void ApplyState(ItemState state, int currentCurrency)
    {
        HideAll();

        switch (state)
        {
            case ItemState.ForSale:
                ShowForSale(currentCurrency);
                break;
            case ItemState.Owned:
                ShowOwned();
                break;
            case ItemState.Equipped:
                ShowEquipped();
                break;
        }
    }

    private void HideAll()
    {
        purchaseButton.gameObject.SetActive(false);
        priceText.gameObject.SetActive(false);
        collectedText.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        equippedText.gameObject.SetActive(false);
    }

    private void ShowForSale(int currentCurrency)
    {
        purchaseButton.gameObject.SetActive(true);
        priceText.gameObject.SetActive(true);

        purchaseButton.interactable = (currentCurrency >= _price);
        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(OnPurchaseClicked);
    }

    private void ShowOwned()
    {
        collectedText.gameObject.SetActive(true);
        collectedText.text = "OWNED";
        priceTextParent.gameObject.SetActive(false);

        equipButton.gameObject.SetActive(true);
        equipButton.interactable = true;
        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(OnEquipClicked);
    }

    private void ShowEquipped()
    {
        equippedText.gameObject.SetActive(true);
        equippedText.text = "EQUIPPED";
        priceTextParent.gameObject.SetActive(false);
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
