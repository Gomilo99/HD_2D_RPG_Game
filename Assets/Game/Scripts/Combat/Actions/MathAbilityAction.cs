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
                CombatManager.Instance?.LogEvent($"{user.Name} usa {ability.abilityName} en {target.Name} por {damage} de dano.");
                break;
            case AbilityEffectType.DebuffIntelligence:
                target.ApplyStatusEffect(new StatModifierEffect(ability.abilityName, StatType.Inteligencia, -ability.power, ability.durationTurns));
                CombatManager.Instance?.LogEvent($"{user.Name} debuffea a {target.Name} (-{ability.power} inteligencia, {ability.durationTurns} turnos).");
                break;
            case AbilityEffectType.DebuffMemory:
                target.ApplyStatusEffect(new StatModifierEffect(ability.abilityName, StatType.Memoria, -ability.power, ability.durationTurns));
                CombatManager.Instance?.LogEvent($"{user.Name} debuffea a {target.Name} (-{ability.power} memoria, {ability.durationTurns} turnos).");
                break;
            case AbilityEffectType.BuffMemory:
                target.ApplyStatusEffect(new StatModifierEffect(ability.abilityName, StatType.Memoria, ability.power, ability.durationTurns));
                CombatManager.Instance?.LogEvent($"{user.Name} buffea a {target.Name} (+{ability.power} memoria, {ability.durationTurns} turnos).");
                break;
            case AbilityEffectType.Heal:
                target.Heal(ability.power);
                CombatManager.Instance?.LogEvent($"{user.Name} cura a {target.Name} por {ability.power}.");
                break;
            case AbilityEffectType.Poison:
                // Aplica veneno: daño por turno igual a power, durante durationTurns turnos.
                target.ApplyStatusEffect(new PoisonStatusEffect(ability.abilityName, ability.power, ability.durationTurns));
                CombatManager.Instance?.LogEvent($"{target.Name} queda envenenado ({ability.power} dano/turno, {ability.durationTurns} turnos).");
                break;
            case AbilityEffectType.Paralyze:
                // Aplica parálisis: el objetivo pierde durationTurns turnos de acción.
                target.ApplyStatusEffect(new ParalysisStatusEffect(ability.abilityName, ability.durationTurns));
                CombatManager.Instance?.LogEvent($"{target.Name} queda paralizado ({ability.durationTurns} turnos).");
                break;
        }
    }
}
