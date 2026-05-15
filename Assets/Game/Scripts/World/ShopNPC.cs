using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC de tienda que permite al jugador comprar y vender objetos consumibles.
/// Implementa IInteractable para integrarse con el sistema de interacción del jugador.
///
/// Responsabilidades:
/// - Exponer el catálogo de objetos a la venta.
/// - Procesar transacciones de compra (quitar dinero, añadir objeto al inventario).
/// - Procesar transacciones de venta (quitar objeto del inventario, añadir dinero).
///
/// Dependencias:
/// - PlayerInventory (para dar/quitar objetos).
/// - PlayerData (para gestionar el dinero).
///
/// Corrida en frío — Compra:
/// 1. Jugador interactúa con el NPC → la UI de tienda se activa.
/// 2. UI llama BuyItem(itemData).
/// 3. Se verifica que el jugador tenga suficiente dinero.
/// 4. PlayerData.SpendMoney() deduce el valor.
/// 5. PlayerInventory.AddItem() añade el objeto.
/// 6. ItemPurchased se dispara para actualizar la UI.
///
/// Corrida en frío — Venta:
/// 1. UI llama SellItem(itemData).
/// 2. PlayerInventory.UseItem() retira el objeto.
/// 3. PlayerData.AddMoney(item.value) añade el dinero.
/// 4. ItemSold se dispara para actualizar la UI.
/// </summary>
public class ShopNPC : MonoBehaviour, IInteractable
{
    [Header("Catálogo de la tienda")]
    [SerializeField] private List<ItemData> catalogItems = new List<ItemData>();

    [Header("UI de la tienda")]
    [SerializeField] private GameObject shopUIPanel;

    [Header("Nombre del NPC")]
    [SerializeField] private string npcName = "Vendedor";

    /// <summary>Se dispara cuando se completa una compra. Parámetro: ítem comprado.</summary>
    public System.Action<ItemData> ItemPurchased;

    /// <summary>Se dispara cuando se completa una venta. Parámetro: ítem vendido.</summary>
    public System.Action<ItemData> ItemSold;

    // ── IInteractable ─────────────────────────────────────────────────────────

    public string InteractPrompt => $"Hablar con {npcName}";
    public bool CanInteract => true;

    public void Interact(GameObject interactor)
    {
        AbrirTienda();
    }

    // ── API pública (llamada desde la UI de tienda) ────────────────────────────

    /// <summary>Vista de solo lectura del catálogo de la tienda.</summary>
    public IReadOnlyList<ItemData> CatalogItems => catalogItems;

    /// <summary>
    /// Intenta comprar un ítem del catálogo. Retorna true si la transacción fue exitosa.
    /// </summary>
    public bool BuyItem(ItemData item)
    {
        if (item == null || !catalogItems.Contains(item))
        {
            return false;
        }

        if (PlayerData.Instance == null || !PlayerData.Instance.SpendMoney(item.value))
        {
            Debug.Log($"ShopNPC: El jugador no tiene suficiente dinero para comprar {item.itemName}.");
            return false;
        }

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddItem(item, 1);
        }

        ItemPurchased?.Invoke(item);
        return true;
    }

    /// <summary>
    /// Intenta vender un ítem del inventario del jugador.
    /// El precio de venta es item.value.
    /// Retorna true si la transacción fue exitosa.
    /// </summary>
    public bool SellItem(ItemData item)
    {
        if (item == null)
        {
            return false;
        }

        if (PlayerInventory.Instance == null || !PlayerInventory.Instance.UseItem(item))
        {
            Debug.Log($"ShopNPC: El jugador no tiene {item.itemName} para vender.");
            return false;
        }

        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.AddMoney(item.value);
        }

        ItemSold?.Invoke(item);
        return true;
    }

    /// <summary>Cierra el panel de tienda.</summary>
    public void CerrarTienda()
    {
        if (shopUIPanel != null)
        {
            shopUIPanel.SetActive(false);
        }
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private void AbrirTienda()
    {
        if (shopUIPanel != null)
        {
            shopUIPanel.SetActive(true);
        }
    }
}
