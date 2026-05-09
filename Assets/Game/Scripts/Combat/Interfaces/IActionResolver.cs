using System.Collections.Generic;

public interface IActionResolver
{
    void Resolve(ICombatant user, ICombatAction action, IReadOnlyList<ICombatant> targets);
}
