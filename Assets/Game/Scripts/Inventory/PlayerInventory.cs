using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona el inventario del jugador: objetos consumibles y de equipamiento.
/// Diseñado como singleton persistente entre escenas (DontDestroyOnLoad).
///
/// Responsabilidades (S de SOLID):
/// - Almacenar y recuperar consumibles e ítems de equipamiento.
/// - Exponer operaciones de añadir, usar, comprar y vender objetos.
///
/// Corrida en frío — Añadir objeto:
/// 1. PlayerInventory.Instance.AddItem(itemData) es llamado.
/// 2. Se busca si ya existe una entrada con el mismo ItemData.
/// 3. Si existe → quantity++; si no → se crea una nueva entrada.
/// 4. ItemAdded se dispara para que la UI se actualice.
///
/// Corrida en frío — Usar objeto:
/// 1. UseItem(itemData) busca la entrada correspondiente.
/// 2. Si quantity > 0 → quantity--; si llega a 0 la entrada se elimina.
/// 3. ItemUsed se dispara.
///
/// Posibles errores:
/// - Instancia duplicada: el check en Awake destruye duplicados.
/// - Intentar usar un ítem que no existe en el inventario: UseItem retorna false.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    /// <summary>Entrada del inventario para un consumible.</summary>
    [Serializable]
    public class ConsumableEntry
    {
        public ItemData item;
        public int quantity;

        public ConsumableEntry(ItemData item, int quantity)
        {
            this.item = item;
            this.quantity = Mathf.Max(0, quantity);
        }
    }

    public static PlayerInventory Instance { get; private set; }

    private readonly List<ConsumableEntry> consumables = new List<ConsumableEntry>();
    private readonly List<EquipmentData> equipment = new List<EquipmentData>();

    /// <summary>Se dispara cuando se añade un consumible. Parámetro: entrada actualizada.</summary>
    public event Action<ConsumableEntry> ItemAdded;

    /// <summary>Se dispara cuando se usa un consumible. Parámetro: ItemData usado.</summary>
    public event Action<ItemData> ItemUsed;

    /// <summary>Se dispara cuando se añade equipamiento.</summary>
    public event Action<EquipmentData> EquipmentAdded;

    /// <summary>Vista de solo lectura de los consumibles actuales.</summary>
    public IReadOnlyList<ConsumableEntry> Consumables => consumables;

    /// <summary>Vista de solo lectura del equipamiento actual.</summary>
    public IReadOnlyList<EquipmentData> Equipment => equipment;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Consumibles ───────────────────────────────────────────────────────────

    /// <summary>Añade la cantidad indicada de un consumible al inventario.</summary>
    public void AddItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
        {
            return;
        }

        ConsumableEntry entry = FindEntry(item);
        if (entry == null)
        {
            entry = new ConsumableEntry(item, 0);
            consumables.Add(entry);
        }

        entry.quantity += quantity;
        ItemAdded?.Invoke(entry);
    }

    /// <summary>
    /// Intenta usar un consumible del inventario. Retorna true si tuvo éxito.
    /// </summary>
    public bool UseItem(ItemData item)
    {
        if (item == null)
        {
            return false;
        }

        ConsumableEntry entry = FindEntry(item);
        if (entry == null || entry.quantity <= 0)
        {
            return false;
        }

        entry.quantity -= 1;
        if (entry.quantity == 0)
        {
            consumables.Remove(entry);
        }

        ItemUsed?.Invoke(item);
        return true;
    }

    /// <summary>Verifica si el inventario contiene al menos una unidad del ítem indicado.</summary>
    public bool HasItem(ItemData item)
    {
        ConsumableEntry entry = FindEntry(item);
        return entry != null && entry.quantity > 0;
    }

    // ── Equipamiento ──────────────────────────────────────────────────────────

    /// <summary>Añade un objeto de equipamiento al inventario.</summary>
    public void AddEquipment(EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            return;
        }

        equipment.Add(equipmentData);
        EquipmentAdded?.Invoke(equipmentData);
    }

    /// <summary>Retira un objeto de equipamiento del inventario. Retorna true si existía.</summary>
    public bool RemoveEquipment(EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            return false;
        }

        return equipment.Remove(equipmentData);
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private ConsumableEntry FindEntry(ItemData item)
    {
        foreach (ConsumableEntry entry in consumables)
        {
            if (entry.item == item)
            {
                return entry;
            }
        }

        return null;
    }
}
