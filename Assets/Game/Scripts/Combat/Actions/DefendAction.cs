using UnityEngine;

public class DefendAction : ICombatAction
{
    public string ActionName => "Defender";

    public void Execute(ICombatant user, ICombatant target)
    {
        if (user == null)
        {
            return;
        }

        int boost = Mathf.Max(1, user.Defense / 2);
        user.ApplyStatusEffect(new StatModifierEffect("Defensa", StatType.Memoria, boost, 1));

        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.LogEvent($"{user.Name} se defiende (+{boost} defensa).");
        }
    }
}
