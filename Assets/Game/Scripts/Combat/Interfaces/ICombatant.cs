public interface ICombatant
{
    string Name { get; }
    bool IsAlive { get; }
    int Speed { get; }
    int Attack { get; }
    int Defense { get; }
    int Luck { get; }
    int MaxHealth { get; }
    int CurrentHealth { get; }

    /// <summary>Indica si el combatiente no puede actuar este turno (ej: parálisis).</summary>
    bool IsActionBlocked { get; }

    void TakeDamage(int amount);
    void Heal(int amount);
    void ModifyStat(StatType stat, int amount);
    void ChooseAction(CombatManager combatManager);
    void ApplyStatusEffect(IStatusEffect effect);
    void TickStatusEffects();
}
