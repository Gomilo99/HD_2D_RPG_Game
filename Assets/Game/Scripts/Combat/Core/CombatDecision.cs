using System.Collections.Generic;

public readonly struct CombatDecision
{
    public ICombatAction Action { get; }
    public IReadOnlyList<ICombatant> Targets { get; }

    public CombatDecision(ICombatAction action, IReadOnlyList<ICombatant> targets)
    {
        Action = action;
        Targets = targets;
    }
}
