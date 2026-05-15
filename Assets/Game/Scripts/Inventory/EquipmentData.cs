using UnityEngine;

/// <summary>
/// ScriptableObject que define un objeto de equipamiento.
/// Al equiparse a un personaje, modifica una o más estadísticas base de manera permanente
/// mientras está equipado. También puede tener efectos adicionales sobre habilidades.
///
/// Uso:
/// - Crea un asset con clic derecho → RPG/Equipment Data
/// - Configura los modificadores de estadísticas.
/// - Asigna al PlayerInventory para que el jugador pueda equiparlo.
/// </summary>
[CreateAssetMenu(fileName = "NewEquipment", menuName = "RPG/Equipment Data")]
public class EquipmentData : ScriptableObject
{
    [Header("Identificación")]
    public string equipmentName;
    [TextArea] public string description;

    [Header("Precio")]
    [Tooltip("Precio de venta/compra en la tienda.")]
    public int value = 100;

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
}
