public class StatModifierEffect : IStatusEffect
{
    private readonly StatType stat;
    private readonly int amount;
    private bool applied;

    public string Name { get; }
    public int RemainingTurns { get; private set; }

    public StatModifierEffect(string name, StatType stat, int amount, int durationTurns)
    {
        Name = string.IsNullOrWhiteSpace(name) ? stat.ToString() : name;
        this.stat = stat;
        this.amount = amount;
        RemainingTurns = durationTurns;
    }

    public void Apply(ICombatant target)
    {
        if (applied || target == null)
        {
            return;
        }

        target.ModifyStat(stat, amount);
        applied = true;
    }

    public void Tick(ICombatant target)
    {
        if (RemainingTurns <= 0)
        {
            return;
        }

        RemainingTurns -= 1;
    }

    public void Remove(ICombatant target)
    {
        if (!applied || target == null)
        {
            return;
        }

        target.ModifyStat(stat, -amount);
        applied = false;
    }
}
