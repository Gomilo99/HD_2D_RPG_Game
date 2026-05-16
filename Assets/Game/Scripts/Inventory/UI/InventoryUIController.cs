using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds inventory buttons at runtime and keeps the list in sync with PlayerInventory.
/// </summary>
public class InventoryUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemButtonContainer;
    [SerializeField] private Button itemButtonPrefab;
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Behavior")]
    [SerializeField] private bool consumeOnClick = false;
    [SerializeField] private bool closeOnUse = false;

    private readonly List<Button> spawnedButtons = new List<Button>();
    private bool isReady;

    public event Action<ItemData> ItemSelected;

    private void Awake()
    {
        isReady = ValidateReferences();
        if (!isReady)
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        Subscribe();
        Refresh();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Open()
    {
        if (!isReady)
        {
            return;
        }

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        Refresh();
    }

    public void Close()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    public void Refresh()
    {
        BuildItemButtons();
        UpdateMoney();
    }

    private void BuildItemButtons()
    {
        ClearButtons();

        if (PlayerInventory.Instance == null || itemButtonPrefab == null || itemButtonContainer == null)
        {
            return;
        }

        foreach (PlayerInventory.ConsumableEntry entry in PlayerInventory.Instance.Consumables)
        {
            if (entry == null || entry.item == null)
            {
                continue;
            }

            ItemData item = entry.item;
            int quantity = entry.quantity;

            Button button = Instantiate(itemButtonPrefab, itemButtonContainer);
            spawnedButtons.Add(button);

            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = $"{item.itemName} x{quantity}";
            }

            button.onClick.AddListener(() => HandleItemPressed(item));
        }
    }

    private void HandleItemPressed(ItemData item)
    {
        if (item == null)
        {
            return;
        }

        ItemSelected?.Invoke(item);

        if (consumeOnClick && PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.UseItem(item);
            Refresh();

            if (closeOnUse)
            {
                Close();
            }
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

    private void Subscribe()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.ItemAdded += HandleItemAdded;
            PlayerInventory.Instance.ItemUsed += HandleItemUsed;
        }

        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.MoneyChanged += HandleMoneyChanged;
        }
    }

    private void Unsubscribe()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.ItemAdded -= HandleItemAdded;
            PlayerInventory.Instance.ItemUsed -= HandleItemUsed;
        }

        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.MoneyChanged -= HandleMoneyChanged;
        }
    }

    private void HandleItemAdded(PlayerInventory.ConsumableEntry entry)
    {
        Refresh();
    }

    private void HandleItemUsed(ItemData item)
    {
        Refresh();
    }

    private void HandleMoneyChanged(int value)
    {
        UpdateMoney();
    }

    private void ClearButtons()
    {
        foreach (Button button in spawnedButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        spawnedButtons.Clear();
    }

    private bool ValidateReferences()
    {
        if (itemButtonPrefab == null || itemButtonContainer == null)
        {
            Debug.LogWarning("InventoryUIController: Missing itemButtonPrefab or itemButtonContainer.", this);
            return false;
        }

        if (inventoryPanel == null)
        {
            inventoryPanel = gameObject;
        }

        return true;
    }
}
