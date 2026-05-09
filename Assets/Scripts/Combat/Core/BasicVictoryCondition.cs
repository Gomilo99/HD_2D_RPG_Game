using System.Collections.Generic;
using System.Linq;

public class BasicVictoryCondition : IVictoryCondition
{
    public CombatResult Evaluate(IReadOnlyList<ICombatant> players, IReadOnlyList<ICombatant> enemies)
    {
        bool playersAlive = players != null && players.Any(p => p != null && p.IsAlive);
        bool enemiesAlive = enemies != null && enemies.Any(e => e != null && e.IsAlive);

        if (!playersAlive && !enemiesAlive)
        {
            return CombatResult.Defeat;
        }

        if (!enemiesAlive)
        {
            return CombatResult.Victory;
        }

        if (!playersAlive)
        {
            return CombatResult.Defeat;
        }

        return CombatResult.Ongoing;
    }
}
