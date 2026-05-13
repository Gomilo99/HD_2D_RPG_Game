using UnityEngine;

/// <summary>
/// Efecto de estado: Veneno.
/// Aplica una cantidad fija de daño al objetivo al inicio de cada uno de sus turnos,
/// durante una cantidad determinada de turnos.
///
/// Corrida en frío:
/// 1. Se crea: new PoisonStatusEffect("Veneno", dañoPorTurno: 5, turnos: 3)
/// 2. Apply(objetivo) → registra el efecto, no hace daño inmediato.
/// 3. Cada vez que CombatManager llama TickStatusEffects() en el turno del objetivo:
///    a. Tick(objetivo) → objetivo.TakeDamage(dañoPorTurno), luego RemainingTurns--
///    b. Si RemainingTurns == 0 → Remove(objetivo) → el efecto se borra.
/// </summary>
public class PoisonStatusEffect : IStatusEffect
{
    private readonly int damagePerTurn;

    public string Name { get; }
    public int RemainingTurns { get; private set; }

    /// <param name="name">Nombre visible del efecto.</param>
    /// <param name="damagePerTurn">Daño que inflige por turno.</param>
    /// <param name="durationTurns">Número de turnos que dura el efecto.</param>
    public PoisonStatusEffect(string name, int damagePerTurn, int durationTurns)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Veneno" : name;
        this.damagePerTurn = Mathf.Max(1, damagePerTurn);
        RemainingTurns = Mathf.Max(1, durationTurns);
    }

    /// <summary>Aplicación inicial: no causa daño inmediato.</summary>
    public void Apply(ICombatant target) { }

    /// <summary>Inflige daño al inicio del turno del objetivo y reduce la duración.</summary>
    public void Tick(ICombatant target)
    {
        if (RemainingTurns <= 0 || target == null)
        {
            return;
        }

        target.TakeDamage(damagePerTurn);
        CombatManager.Instance?.LogEvent($"{target.Name} sufre {damagePerTurn} de dano por veneno.");
        RemainingTurns -= 1;
    }

    /// <summary>El veneno no revierte ningún modificador al expirar.</summary>
    public void Remove(ICombatant target) { }
}
