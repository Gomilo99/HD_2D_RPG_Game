using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject que define la curva de progresión de nivel para un personaje.
/// Contiene una lista ordenada de LevelEntry, una por nivel disponible.
///
/// Uso:
/// - Crea un asset con clic derecho → RPG/Level Growth Table
/// - Configura cada entrada con la experiencia requerida y el crecimiento de stats.
/// - Asigna este asset a CharacterLevel del personaje correspondiente.
///
/// Corrida en frío:
/// 1. CharacterLevel llama GetEntryForLevel(level) para obtener la entrada del nivel actual.
/// 2. GetNextLevelExperience(level) devuelve cuánta experiencia falta para subir.
/// 3. GetAbilitiesForLevel(level) devuelve las habilidades desbloqueadas al subir a ese nivel.
/// </summary>
[CreateAssetMenu(fileName = "NewLevelGrowthTable", menuName = "RPG/Level Growth Table")]
public class LevelGrowthTable : ScriptableObject
{
    [SerializeField] private List<LevelEntry> entries = new List<LevelEntry>();

    /// <summary>Devuelve la entrada de la tabla para el nivel indicado, o null si no existe.</summary>
    public LevelEntry GetEntryForLevel(int level)
    {
        foreach (LevelEntry entry in entries)
        {
            if (entry.level == level)
            {
                return entry;
            }
        }

        return null;
    }

    /// <summary>
    /// Devuelve la experiencia total necesaria para alcanzar el nivel siguiente al indicado.
    /// Retorna int.MaxValue si ya se está en el nivel máximo.
    /// </summary>
    public int GetNextLevelExperience(int currentLevel)
    {
        LevelEntry next = GetEntryForLevel(currentLevel + 1);
        return next != null ? next.experienceRequired : int.MaxValue;
    }

    /// <summary>
    /// Devuelve la lista de habilidades desbloqueadas al alcanzar el nivel indicado.
    /// </summary>
    public IReadOnlyList<AbilityData> GetAbilitiesForLevel(int level)
    {
        LevelEntry entry = GetEntryForLevel(level);
        if (entry == null || entry.abilitiesUnlocked == null)
        {
            return new AbilityData[0];
        }

        return entry.abilitiesUnlocked;
    }

    /// <summary>Número máximo de nivel definido en la tabla.</summary>
    public int MaxLevel
    {
        get
        {
            int max = 1;
            foreach (LevelEntry entry in entries)
            {
                if (entry.level > max)
                {
                    max = entry.level;
                }
            }

            return max;
        }
    }
}
