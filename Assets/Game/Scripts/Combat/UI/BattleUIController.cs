using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleUIController : MonoBehaviour, IActionSelector
{
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private GameObject actionMenuPanel;
    [SerializeField] private GameObject targetSelectPanel;
    [SerializeField] private GameObject abilityMenuPanel;
    [SerializeField] private GameObject itemMenuPanel;
    [SerializeField] private GameObject overlayPanel;
    [SerializeField] private TextMeshProUGUI messageLogText;
    [SerializeField, Min(1)] private int maxLogLines = 6;

    private readonly Queue<string> logLines = new Queue<string>();

    private PlayerCharacter activePlayer;
    private ICombatAction pendingAction;

    private void OnDisable()
    {
        if (combatManager != null)
        {
            combatManager.CombatLog -= HandleCombatLog;
        }
    }

    public void RequestAction(PlayerCharacter player, CombatManager manager)
    {
        if (combatManager != null)
        {
            combatManager.CombatLog -= HandleCombatLog;
        }

        combatManager = manager;
        if (combatManager != null)
        {
            combatManager.CombatLog += HandleCombatLog;
        }

        activePlayer = player;
        pendingAction = null;
        ShowPanel(actionMenuPanel, true);
        ShowPanel(targetSelectPanel, false);
        ShowPanel(abilityMenuPanel, false);
        ShowPanel(itemMenuPanel, false);
    }

    public void OnAttackPressed()
    {
        if (activePlayer == null || combatManager == null)
        {
            return;
        }

        pendingAction = activePlayer.CreateBasicAttack();
        ShowTargetSelection(AbilityTargetType.SingleEnemy);
    }

    public void OnDefendPressed()
    {
        if (activePlayer == null || combatManager == null)
        {
            return;
        }

        SubmitAction(activePlayer.CreateDefend(), new List<ICombatant> { activePlayer });
    }

    public void OnFleePressed()
    {
        if (activePlayer == null || combatManager == null)
        {
            return;
        }

        SubmitAction(activePlayer.CreateFleeAction(combatManager), new List<ICombatant> { activePlayer });
    }

    public void OnAbilityPressed(AbilityData ability)
    {
        if (activePlayer == null || combatManager == null || ability == null)
        {
            return;
        }

        ICombatAction action = activePlayer.CreateAbility(ability);
        if (ability.targetType == AbilityTargetType.AllAllies || ability.targetType == AbilityTargetType.AllEnemies || ability.targetType == AbilityTargetType.Self)
        {
            IReadOnlyList<ICombatant> targets = combatManager.GetTargetsFor(ability.targetType, activePlayer);
            SubmitAction(action, targets);
            return;
        }

        pendingAction = action;
        ShowTargetSelection(ability.targetType);
    }

    public void OnItemPressed(ItemData item)
    {
        if (activePlayer == null || combatManager == null || item == null)
        {
            return;
        }

        pendingAction = activePlayer.CreateUseItem(item);
        ShowTargetSelection(AbilityTargetType.SingleAlly);
    }

    public void OnTargetSelected(BaseCharacter target)
    {
        if (pendingAction == null || combatManager == null || target == null)
        {
            return;
        }

        SubmitAction(pendingAction, new List<ICombatant> { target });
    }

    private void SubmitAction(ICombatAction action, IReadOnlyList<ICombatant> targets)
    {
        ShowPanel(actionMenuPanel, false);
        ShowPanel(targetSelectPanel, false);
        ShowPanel(abilityMenuPanel, false);
        ShowPanel(itemMenuPanel, false);
        combatManager.SubmitPlayerAction(action, targets);
        pendingAction = null;
    }

    private void ShowTargetSelection(AbilityTargetType targetType)
    {
        ShowPanel(targetSelectPanel, true);
        ShowPanel(actionMenuPanel, false);
        ShowPanel(abilityMenuPanel, false);
        ShowPanel(itemMenuPanel, false);
    }

    private void ShowPanel(GameObject panel, bool show)
    {
        if (panel != null)
        {
            panel.SetActive(show);
        }
    }

    private void HandleCombatLog(string message)
    {
        if (messageLogText != null)
        {
            if (maxLogLines <= 1)
            {
                messageLogText.text = message;
                return;
            }

            logLines.Enqueue(message);
            while (logLines.Count > maxLogLines)
            {
                logLines.Dequeue();
            }

            messageLogText.text = string.Join("\n", logLines);
        }
    }
}
