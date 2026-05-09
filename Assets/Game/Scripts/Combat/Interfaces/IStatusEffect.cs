public interface IStatusEffect
{
    string Name { get; }
    int RemainingTurns { get; }

    void Apply(ICombatant target);
    void Tick(ICombatant target);
    void Remove(ICombatant target);
}
