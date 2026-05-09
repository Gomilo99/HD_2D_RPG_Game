using System.Collections.Generic;

public interface ITurnQueue
{
    IReadOnlyList<ICombatant> CurrentOrder { get; }

    void Initialize(IEnumerable<ICombatant> combatants);
    ICombatant GetNext();
    void RemoveDead();
}
