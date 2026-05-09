using System.Collections.Generic;
using UnityEngine;

public class MathAbilityAction : ICombatAction, IMultiTargetCombatAction
{
    private readonly AbilityData ability;

    public string ActionName => ability != null ? ability.abilityName : "Habilidad";
    public AbilityTargetType TargetType => ability != null ? ability.targetType : AbilityTargetType.SingleEnemy;

    public MathAbilityAction(AbilityData ability)
    {
        this.ability = ability;
    }

    public void Execute(ICombatant user, ICombatant target)
    {
        ApplyEffect(user, target);
    }

    public void Execute(ICombatant user, IReadOnlyList<ICombatant> targets)
    {
        if (targets == null)
        {
            return;
        }

        foreach (ICombatant target in targets)
        {
            ApplyEffect(user, target);
        }
    }

    private void ApplyEffect(ICombatant user, ICombatant target)
    {
        if (ability == null || user == null || target == null)
        {
            return;
        }

        switch (ability.effectType)
        {
            case AbilityEffectType.Damage:
                int baseDamage = ability.power + user.Attack;
                int mitigation = target.Defense / 2;
                int damage = Mathf.Max(1, baseDamage - mitigation);
                target.TakeDamage(damage);
                break;
            case AbilityEffectType.DebuffIntelligence:
                target.ApplyStatusEffect(new StatModifierEffect(ability.abilityName, StatType.Inteligencia, -ability.power, ability.durationTurns));
                break;
            case AbilityEffectType.DebuffMemory:
                target.ApplyStatusEffect(new StatModifierEffect(ability.abilityName, StatType.Memoria, -ability.power, ability.durationTurns));
                break;
            case AbilityEffectType.BuffMemory:
                target.ApplyStatusEffect(new StatModifierEffect(ability.abilityName, StatType.Memoria, ability.power, ability.durationTurns));
                break;
            case AbilityEffectType.Heal:
                target.Heal(ability.power);
                break;
        }
    }
}
