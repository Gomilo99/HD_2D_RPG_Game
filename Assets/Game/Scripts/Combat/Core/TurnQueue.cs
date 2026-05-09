using System.Collections.Generic;
using System.Linq;

public class TurnQueue : ITurnQueue
{
    private readonly List<ICombatant> queue = new List<ICombatant>();

    public IReadOnlyList<ICombatant> CurrentOrder => queue;

    public void Initialize(IEnumerable<ICombatant> combatants)
    {
        queue.Clear();
        if (combatants == null)
        {
            return;
        }

        queue.AddRange(combatants.Where(c => c != null));
        queue.Sort((a, b) => b.Speed.CompareTo(a.Speed));
    }

    public ICombatant GetNext()
    {
        if (queue.Count == 0)
        {
            return null;
        }

        ICombatant current = queue[0];
        queue.RemoveAt(0);
        queue.Add(current);
        return current;
    }

    public void RemoveDead()
    {
        queue.RemoveAll(c => c == null || !c.IsAlive);
    }
}
