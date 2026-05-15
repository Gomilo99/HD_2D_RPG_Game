using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : BaseCharacter
{
    [SerializeField] private MonoBehaviour actionSelectorComponent;

    private IActionSelector actionSelector;

    public IReadOnlyList<AbilityData> Abilities => stats != null ? stats.startingAbilities : new List<AbilityData>();

    protected override void Awake()
    {
        base.Awake();
        actionSelector = actionSelectorComponent as IActionSelector;
    }

    public void SetActionSelector(IActionSelector selector)
    {
        actionSelector = selector;
        actionSelectorComponent = selector as MonoBehaviour;
    }

    public override void ChooseAction(CombatManager combatManager)
    {
        if (combatManager == null)
        {
            return;
        }

        if (actionSelector == null)
        {
            ICombatant target = combatManager.GetRandomEnemy();
            if (target != null)
            {
                combatManager.ExecuteAction(this, new BasicAttackAction(), new List<ICombatant> { target });
            }
            else
            {
                combatManager.EndTurn();
            }
            return;
        }

        actionSelector.RequestAction(this, combatManager);
    }

    public ICombatAction CreateBasicAttack()
    {
        return new BasicAttackAction();
    }

    public ICombatAction CreateDefend()
    {
        return new DefendAction();
    }

    public ICombatAction CreateUseItem(ItemData item)
    {
        return new UseItemAction(item);
    }

    public ICombatAction CreateAbility(AbilityData ability)
    {
        return new MathAbilityAction(ability);
    }

    public ICombatAction CreateFleeAction(IFleeHandler fleeHandler)
    {
        return new FleeAction(fleeHandler);
    }
}
