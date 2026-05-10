using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Componente de progresión de nivel para un personaje (variable).
/// Gestiona la experiencia acumulada, el nivel actual, el crecimiento de estadísticas
/// y el desbloqueo de habilidades nuevas al subir de nivel.
///
/// Dependencias:
/// - BaseCharacter (para aplicar el crecimiento de stats)
/// - LevelGrowthTable (define la curva de progresión)
///
/// Uso típico:
/// - Añadir al mismo GameObject que BaseCharacter/PlayerCharacter.
/// - Asignar un LevelGrowthTable en el Inspector.
/// - Llamar a GainExperience(amount) tras ganar una batalla.
///
/// Corrida en frío:
/// 1. El jugador derrota a un enemigo → CombatManager dispara Victoria.
/// 2. Se llama GainExperience(100) en cada CharacterLevel del equipo.
/// 3. Se suma la experiencia a totalExperience.
/// 4. Se compara con GetNextLevelExperience(currentLevel):
///    a. Si totalExperience >= experiencia requerida → LevelUp().
///    b. LevelUp() incrementa currentLevel, aplica el gain de stats y desbloquea habilidades.
///    c. Se repite mientras la experiencia permita más de un nivel.
/// 5. LeveledUp se dispara para que la UI o efectos reaccionen.
///
/// Posibles errores:
/// - growthTable nulo: sin tabla no puede subir de nivel (se registra un warning).
/// - baseCharacter nulo: se busca en el mismo GameObject en Awake().
/// </summary>
[RequireComponent(typeof(BaseCharacter))]
public class CharacterLevel : MonoBehaviour
{
    [SerializeField] private LevelGrowthTable growthTable;
    [SerializeField, Min(1)] private int currentLevel = 1;
    [SerializeField, Min(0)] private int totalExperience = 0;

    private BaseCharacter baseCharacter;
    private readonly List<AbilityData> unlockedAbilities = new List<AbilityData>();

    /// <summary>Nivel actual del personaje.</summary>
    public int CurrentLevel => currentLevel;

    /// <summary>Experiencia total acumulada.</summary>
    public int TotalExperience => totalExperience;

    /// <summary>
    /// Experiencia requerida para alcanzar el siguiente nivel.
    /// Retorna int.MaxValue si se está en el nivel máximo.
    /// </summary>
    public int NextLevelExperience =>
        growthTable != null ? growthTable.GetNextLevelExperience(currentLevel) : int.MaxValue;

    /// <summary>Lista de habilidades desbloqueadas por progresión (no las iniciales).</summary>
    public IReadOnlyList<AbilityData> UnlockedAbilities => unlockedAbilities;

    /// <summary>Se dispara cuando el personaje sube de nivel. Parámetro: nuevo nivel.</summary>
    public event Action<int> LeveledUp;

    private void Awake()
    {
        baseCharacter = GetComponent<BaseCharacter>();
    }

    /// <summary>
    /// Otorga la cantidad de experiencia indicada y activa la subida de nivel
    /// si se supera el umbral.
    /// </summary>
    /// <param name="amount">Puntos de experiencia a añadir (debe ser positivo).</param>
    public void GainExperience(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        totalExperience += amount;
        VerificarSubidaDENivel();
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    /// <summary>Comprueba si se debe subir uno o más niveles con la experiencia actual.</summary>
    private void VerificarSubidaDENivel()
    {
        if (growthTable == null)
        {
            Debug.LogWarning($"CharacterLevel en {gameObject.name}: growthTable no asignado.", this);
            return;
        }

        // Permite subir múltiples niveles de golpe si la experiencia lo permite.
        while (totalExperience >= growthTable.GetNextLevelExperience(currentLevel))
        {
            LevelUp();
        }
    }

    /// <summary>Aplica los beneficios de subir un nivel.</summary>
    private void LevelUp()
    {
        int nextLevel = currentLevel + 1;
        LevelEntry entry = growthTable.GetEntryForLevel(nextLevel);

        if (entry == null)
        {
            return; // Nivel máximo alcanzado.
        }

        currentLevel = nextLevel;

        AplicarCrecimientoStats(entry);
        DesbloquearHabilidades(entry);

        LeveledUp?.Invoke(currentLevel);
        Debug.Log($"{gameObject.name} subió al nivel {currentLevel}.", this);
    }

    /// <summary>Aplica el crecimiento de estadísticas definido en la entrada de nivel.</summary>
    private void AplicarCrecimientoStats(LevelEntry entry)
    {
        if (baseCharacter == null || entry == null)
        {
            return;
        }

        if (entry.corduraGain > 0)
        {
            baseCharacter.Heal(entry.corduraGain);
        }

        if (entry.inteligenciaGain != 0)
        {
            baseCharacter.ModifyStat(StatType.Inteligencia, entry.inteligenciaGain);
        }

        if (entry.memoriaGain != 0)
        {
            baseCharacter.ModifyStat(StatType.Memoria, entry.memoriaGain);
        }

        if (entry.rapidezGain != 0)
        {
            baseCharacter.ModifyStat(StatType.Rapidez, entry.rapidezGain);
        }

        if (entry.fealdadGain != 0)
        {
            baseCharacter.ModifyStat(StatType.Fealdad, entry.fealdadGain);
        }
    }

    /// <summary>Agrega a la lista las habilidades desbloqueadas por el nivel.</summary>
    private void DesbloquearHabilidades(LevelEntry entry)
    {
        if (entry == null || entry.abilitiesUnlocked == null)
        {
            return;
        }

        foreach (AbilityData ability in entry.abilitiesUnlocked)
        {
            if (ability != null && !unlockedAbilities.Contains(ability))
            {
                unlockedAbilities.Add(ability);
                Debug.Log($"{gameObject.name} desbloqueó la habilidad: {ability.abilityName}.", this);
            }
        }
    }
}
