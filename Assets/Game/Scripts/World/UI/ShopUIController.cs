using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds shop buttons at runtime for buy/sell lists.
/// </summary>
public class ShopUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform buyButtonContainer;
    [SerializeField] private Transform sellButtonContainer;
    [SerializeField] private Button itemButtonPrefab;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI shopNameText;

    private ShopNPC currentShop;
    private bool isReady;

    private void Awake()
    {
        isReady = ValidateReferences();
        if (!isReady)
        {
            enabled = false;
        }
    }

    public void Open(ShopNPC shop)
    {
        if (!isReady)
        {
            return;
        }

        currentShop = shop;

        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        Refresh();
    }

    public void Close()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        currentShop = null;
    }

    public void Refresh()
    {
        BuildBuyButtons();
        BuildSellButtons();
        UpdateMoney();
        UpdateShopName();
    }

    private void BuildBuyButtons()
    {
        ClearContainer(buyButtonContainer);

        if (currentShop == null || itemButtonPrefab == null || buyButtonContainer == null)
        {
            return;
        }

        foreach (ItemData item in currentShop.CatalogItems)
        {
            if (item == null)
            {
                continue;
            }

            Button button = Instantiate(itemButtonPrefab, buyButtonContainer);
            SetButtonLabel(button, $"{item.itemName} - $ {item.value}");
            button.onClick.AddListener(() => TryBuy(item));
        }
    }

    private void BuildSellButtons()
    {
        ClearContainer(sellButtonContainer);

        if (currentShop == null || itemButtonPrefab == null || sellButtonContainer == null)
        {
            return;
        }

        if (PlayerInventory.Instance == null)
        {
            return;
        }

        foreach (EquipmentData equipment in PlayerInventory.Instance.Equipment)
        {
            if (equipment == null)
            {
                continue;
            }

            Button button = Instantiate(itemButtonPrefab, sellButtonContainer);
            SetButtonLabel(button, $"{equipment.itemName} +$ {equipment.value}");
            button.onClick.AddListener(() => TrySell(equipment));
        }

        foreach (PlayerInventory.ConsumableEntry entry in PlayerInventory.Instance.Consumables)
        {
            if (entry == null || entry.item == null)
            {
                continue;
            }

            ItemData item = entry.item;
            string label = $"{item.itemName} x{entry.quantity} +$ {item.value}";

            Button button = Instantiate(itemButtonPrefab, sellButtonContainer);
            SetButtonLabel(button, label);
            button.onClick.AddListener(() => TrySell(item));
        }
    }

    private void TryBuy(ItemData item)
    {
        if (currentShop == null || item == null)
        {
            return;
        }

        if (currentShop.BuyItem(item))
        {
            Refresh();
        }
    }

    private void TrySell(ItemData item)
    {
        if (currentShop == null || item == null)
        {
            return;
        }

        if (currentShop.SellItem(item))
        {
            Refresh();
        }
    }

    private void UpdateMoney()
    {
        if (moneyText == null || PlayerData.Instance == null)
        {
            return;
        }

        moneyText.text = $"$ {PlayerData.Instance.Money}";
    }

    private void UpdateShopName()
    {
        if (shopNameText == null || currentShop == null)
        {
            return;
        }

        shopNameText.text = currentShop.name;
    }

    private void ClearContainer(Transform container)
    {
        if (container == null)
        {
            return;
        }

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }

    private void SetButtonLabel(Button button, string label)
    {
        if (button == null)
        {
            return;
        }

        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = label;
        }
    }

    private bool ValidateReferences()
    {
        if (itemButtonPrefab == null)
        {
            Debug.LogWarning("ShopUIController: Missing itemButtonPrefab.", this);
            return false;
        }

        if (shopPanel == null)
        {
            shopPanel = gameObject;
        }

        return true;
    }
}
