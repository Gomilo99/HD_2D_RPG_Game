using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles equipping/unequipping equipment and applying stat modifiers.
/// </summary>
public class EquipmentLoadout : MonoBehaviour
{
    private readonly List<EquipmentData> equipped = new List<EquipmentData>();
    private BaseCharacter owner;

    public IReadOnlyList<EquipmentData> Equipped => equipped;

    private void Awake()
    {
        owner = GetComponent<BaseCharacter>();
    }

    public bool Equip(EquipmentData data)
    {
        if (data == null || owner == null)
        {
            return false;
        }

        if (equipped.Contains(data))
        {
            return false;
        }

        ApplyModifiers(data, 1);
        equipped.Add(data);
        return true;
    }

    public bool Unequip(EquipmentData data)
    {
        if (data == null || owner == null)
        {
            return false;
        }

        if (!equipped.Remove(data))
        {
            return false;
        }

        ApplyModifiers(data, -1);
        return true;
    }

    private void ApplyModifiers(EquipmentData data, int sign)
    {
        owner.ModifyStat(StatType.Cordura, data.corduraModifier * sign);
        owner.ModifyStat(StatType.Inteligencia, data.inteligenciaModifier * sign);
        owner.ModifyStat(StatType.Memoria, data.memoriaModifier * sign);
        owner.ModifyStat(StatType.Rapidez, data.rapidezModifier * sign);
        owner.ModifyStat(StatType.Fealdad, data.fealdadModifier * sign);
    }
}
