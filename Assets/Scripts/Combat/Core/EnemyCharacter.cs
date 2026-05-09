using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : BaseCharacter
{
    [SerializeField] private MonoBehaviour aiControllerComponent;

    private IAIController aiController;

    protected override void Awake()
    {
        base.Awake();
        aiController = aiControllerComponent as IAIController;
    }

    public override void ChooseAction(CombatManager combatManager)
    {
        if (combatManager == null)
        {
            return;
        }

        if (aiController == null)
        {
            ICombatant target = combatManager.GetRandomPlayer();
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

        CombatDecision decision = aiController.DecideAction(this, combatManager);
        if (decision.Action == null || decision.Targets == null || decision.Targets.Count == 0)
        {
            combatManager.EndTurn();
            return;
        }

        combatManager.ExecuteAction(this, decision.Action, decision.Targets);
    }
}
