using System.Collections.Generic;

public interface IVictoryCondition
{
    CombatResult Evaluate(IReadOnlyList<ICombatant> players, IReadOnlyList<ICombatant> enemies);
}
