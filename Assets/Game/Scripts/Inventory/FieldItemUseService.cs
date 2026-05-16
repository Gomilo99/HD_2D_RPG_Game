using UnityEngine;

/// <summary>
/// Applies consumable item effects outside of battle.
/// </summary>
public static class FieldItemUseService
{
    public static bool TryUse(ItemData item, BaseCharacter target)
    {
        if (item == null || target == null)
        {
            return false;
        }

        if (item.category != ItemCategory.Consumable || !item.CanUseInField)
        {
            return false;
        }

        if (item.effectType != ItemEffectType.Heal && item.effectType != ItemEffectType.Revive)
        {
            return false;
        }

        if (PlayerInventory.Instance == null || !PlayerInventory.Instance.UseItem(item))
        {
            return false;
        }

        target.Heal(item.power);
        return true;
    }
}
