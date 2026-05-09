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

    void TakeDamage(int amount);
    void Heal(int amount);
    void ChooseAction(CombatManager combatManager);
    void ApplyStatusEffect(IStatusEffect effect);
    void TickStatusEffects();
}
