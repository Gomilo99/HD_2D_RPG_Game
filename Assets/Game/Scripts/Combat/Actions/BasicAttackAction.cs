using UnityEngine;

public class BasicAttackAction : ICombatAction
{
    public string ActionName => "Ataque Básico";

    public void Execute(ICombatant user, ICombatant target)
    {
        if (user == null || target == null)
        {
            return;
        }

        int baseDamage = user.Attack;
        int mitigation = target.Defense / 2;
        int damage = Mathf.Max(1, baseDamage - mitigation);

        float critChance = Mathf.Clamp01(user.Luck * 0.01f);
        bool isCrit = false;
        if (Random.value <= critChance)
        {
            damage = Mathf.CeilToInt(damage * 1.5f);
            isCrit = true;
        }

        target.TakeDamage(damage);

        if (CombatManager.Instance != null)
        {
            string critText = isCrit ? " (critico)" : string.Empty;
            CombatManager.Instance.LogEvent($"{user.Name} ataca a {target.Name} por {damage} de dano{critText}.");
        }
    }
}
