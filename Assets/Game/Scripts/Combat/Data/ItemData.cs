using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "RPG/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public ItemEffectType effectType = ItemEffectType.Heal;
    public int power = 10;
}
