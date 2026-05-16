using UnityEngine;

/// <summary>
using UnityEngine;

/// <summary>
/// Objeto de equipamiento que hereda de ItemData.
/// Al equiparse a un personaje, modifica estadísticas base mientras está equipado.
/// La identificación, descripción, precio y reglas de uso están en ItemData.
[CreateAssetMenu(fileName = "NewEquipment", menuName = "RPG/Equipment Data")]
public class EquipmentData : ItemData
{
    [Header("Modificadores de estadísticas")]
    [Tooltip("Modifica la Cordura máxima (vida).")]
    public int corduraModifier = 0;

    [Tooltip("Modifica la Inteligencia (ataque).")]
    public int inteligenciaModifier = 0;

    [Tooltip("Modifica la Memoria (defensa).")]
    public int memoriaModifier = 0;

    [Tooltip("Modifica la Rapidez (velocidad).")]
    public int rapidezModifier = 0;

    [Tooltip("Modifica la Fealdad (suerte).")]
    public int fealdadModifier = 0;

    [Header("Efecto adicional sobre habilidades")]
    [Tooltip("Porcentaje de potenciación al poder de habilidades (0 = sin efecto).")]
    [Range(0f, 2f)]
    public float abilityPowerMultiplier = 0f;

    [Tooltip("Turnos adicionales que se suman a la duración de buffs/debuffs.")]
    [Min(0)]
    public int abilityDurationBonus = 0;

    private void OnValidate()
    {
        category = ItemCategory.Equipment;
    }
}
