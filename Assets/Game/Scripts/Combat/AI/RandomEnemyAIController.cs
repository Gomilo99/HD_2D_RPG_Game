using System.Collections.Generic;
using UnityEngine;

public class RandomEnemyAIController : MonoBehaviour, IAIController
{
    [SerializeField] private List<AbilityData> abilities = new List<AbilityData>();
    [SerializeField, Range(0f, 1f)] private float abilityChance = 0.25f;

    public CombatDecision DecideAction(EnemyCharacter enemy, CombatManager combatManager)
    {
        if (enemy == null || combatManager == null)
        {
            return default;
        }

        ICombatAction action = new BasicAttackAction();
        AbilityTargetType targetType = AbilityTargetType.SingleEnemy;

        if (abilities.Count > 0 && Random.value <= abilityChance)
        {
            AbilityData ability = abilities[Random.Range(0, abilities.Count)];
            action = new MathAbilityAction(ability);
            targetType = ability.targetType;
        }

        IReadOnlyList<ICombatant> targets = combatManager.GetTargetsFor(targetType, enemy);
        return new CombatDecision(action, targets);
    }
}
