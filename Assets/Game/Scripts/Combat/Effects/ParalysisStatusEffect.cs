using UnityEngine;

/// <summary>
/// Efecto de estado: Parálisis.
/// Impide que el objetivo actúe durante un número determinado de turnos.
/// Implementa IActionBlockingEffect para que CombatManager detecte el bloqueo
/// y omita el turno del combatiente afectado.
///
/// Corrida en frío:
/// 1. Se crea: new ParalysisStatusEffect("Parálisis", turnos: 1)
/// 2. Apply(objetivo) → registra el efecto.
/// 3. En StartNextTurn(), CombatManager llama currentCombatant.TickStatusEffects():
///    a. Tick(objetivo) → RemainingTurns--
///    b. Si RemainingTurns == 0 → Remove(objetivo) → efecto se borra.
/// 4. Después de TickStatusEffects(), CombatManager comprueba IsActionBlocked:
///    - Si true → el turno se registra como "perdido" y pasa al siguiente combatiente.
///    - Si false → el combatiente actúa con normalidad.
/// </summary>
public class ParalysisStatusEffect : IActionBlockingEffect
{
    public string Name { get; }
    public int RemainingTurns { get; private set; }

    /// <param name="name">Nombre visible del efecto.</param>
    /// <param name="durationTurns">Turnos que el combatiente pierde.</param>
    public ParalysisStatusEffect(string name, int durationTurns)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Parálisis" : name;
        RemainingTurns = Mathf.Max(1, durationTurns);
    }

    /// <summary>Aplicación inicial: solo registra el efecto.</summary>
    public void Apply(ICombatant target) { }

    /// <summary>Reduce la duración en cada turno del objetivo paralizado.</summary>
    public void Tick(ICombatant target)
    {
        if (RemainingTurns > 0)
        {
            RemainingTurns -= 1;
        }
    }

    /// <summary>La parálisis no revierte ningún modificador de estadísticas.</summary>
    public void Remove(ICombatant target) { }
}
