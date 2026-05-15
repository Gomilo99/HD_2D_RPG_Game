public class UseItemAction : ICombatAction
{
    private readonly ItemData item;

    public string ActionName => item != null ? item.itemName : "Objeto";

    public UseItemAction(ItemData item)
    {
        this.item = item;
    }

    public void Execute(ICombatant user, ICombatant target)
    {
        if (item == null || target == null)
        {
            return;
        }

        if (PlayerInventory.Instance == null || !PlayerInventory.Instance.UseItem(item))
        {
            return;
        }

        switch (item.effectType)
        {
            case ItemEffectType.Heal:
                target.Heal(item.power);
                CombatManager.Instance?.LogEvent($"{user.Name} usa {item.itemName} en {target.Name} y cura {item.power}.");
                break;
            case ItemEffectType.BuffMemory:
                target.ApplyStatusEffect(new StatModifierEffect(item.itemName, StatType.Memoria, item.power, 2));
                CombatManager.Instance?.LogEvent($"{user.Name} usa {item.itemName} en {target.Name} (+{item.power} memoria).");
                break;
            case ItemEffectType.Revive:
                target.Heal(item.power);
                CombatManager.Instance?.LogEvent($"{user.Name} revive/curacion en {target.Name} por {item.power}.");
                break;
        }
    }
}
