using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewItem", menuName = "RPG/Item")]
public class ItemData : ScriptableObject
{
    [FormerlySerializedAs("equipmentName")]
    public string itemName;
    [TextArea] public string description;
    public ItemEffectType effectType = ItemEffectType.Heal;
    public int power = 10;

    [Tooltip("Precio de venta/compra en la tienda.")]
    public int value = 50;

    [Header("Categoria")]
    public ItemCategory category = ItemCategory.Consumable;

    [Header("Uso")]
    public ItemUseContext useContext = ItemUseContext.BattleOnly;

    public bool CanUseInBattle => useContext == ItemUseContext.BattleOnly || useContext == ItemUseContext.BattleAndField;
    public bool CanUseInField => useContext == ItemUseContext.FieldOnly || useContext == ItemUseContext.BattleAndField;
}
