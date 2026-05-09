using System.Collections.Generic;

public interface IMultiTargetCombatAction
{
    void Execute(ICombatant user, IReadOnlyList<ICombatant> targets);
}
