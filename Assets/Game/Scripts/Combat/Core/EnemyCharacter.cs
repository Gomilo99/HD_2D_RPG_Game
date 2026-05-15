using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : BaseCharacter
{
    [SerializeField] private MonoBehaviour aiControllerComponent;

    [Header("Progresión")]
    [SerializeField] private LootTable lootTable;
    [SerializeField] private bool destroyOnDefeat = true;

    private IAIController aiController;

    protected override void Awake()
    {
        base.Awake();
        aiController = aiControllerComponent as IAIController;
        Defeated += OnDefeated;
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

    // ── Privados ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Se invoca cuando el enemigo es derrotado.
    /// Evalúa la tabla de loot y distribuye experiencia al equipo.
    /// </summary>
    private void OnDefeated(ICombatant combatant)
    {
        if (lootTable != null)
        {
            lootTable.Evaluate();
        }

        int exp = lootTable != null ? lootTable.ExperienceReward : 0;
        if (exp > 0 && PlayerData.Instance != null)
        {
            // Distribuye la experiencia entre los miembros vivos del equipo.
            foreach (BaseCharacter member in PlayerData.Instance.PartyMembers)
            {
                if (member == null || !member.IsAlive)
                {
                    continue;
                }

                CharacterLevel levelComp = member.GetComponent<CharacterLevel>();
                if (levelComp != null)
                {
                    levelComp.GainExperience(exp);
                }
            }
        }

        if (destroyOnDefeat)
        {
            Destroy(gameObject);
        }
    }
}

