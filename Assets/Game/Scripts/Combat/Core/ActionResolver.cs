using System.Collections.Generic;

public class ActionResolver : IActionResolver
{
    public void Resolve(ICombatant user, ICombatAction action, IReadOnlyList<ICombatant> targets)
    {
        if (user == null || action == null)
        {
            return;
        }

        if (action is IMultiTargetCombatAction multiTargetAction)
        {
            multiTargetAction.Execute(user, targets ?? new List<ICombatant>());
            return;
        }

        if (targets == null || targets.Count == 0)
        {
            return;
        }

        action.Execute(user, targets[0]);
    }
}
