public interface IAIController
{
    CombatDecision DecideAction(EnemyCharacter enemy, CombatManager combatManager);
}
