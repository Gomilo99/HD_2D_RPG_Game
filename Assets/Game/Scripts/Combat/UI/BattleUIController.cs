using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Transform abilityButtonContainer;
    [SerializeField] private Transform itemButtonContainer;
    [SerializeField] private Button actionButtonPrefab;

    private readonly Queue<string> logLines = new Queue<string>();
    private readonly List<Button> spawnedButtons = new List<Button>();
    private readonly List<TargetHighlight> highlightedTargets = new List<TargetHighlight>();

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
        ClearTargetPreview();
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

    public void OnOpenAbilityMenu()
    {
        if (activePlayer == null)
        {
            return;
        }

        BuildAbilityButtons();
        ShowPanel(abilityMenuPanel, true);
        ShowPanel(actionMenuPanel, false);
        ShowPanel(targetSelectPanel, false);
        ShowPanel(itemMenuPanel, false);
    }

    public void OnOpenItemMenu()
    {
        if (activePlayer == null)
        {
            return;
        }

        BuildItemButtons();
        ShowPanel(itemMenuPanel, true);
        ShowPanel(actionMenuPanel, false);
        ShowPanel(targetSelectPanel, false);
        ShowPanel(abilityMenuPanel, false);
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
        ClearTargetPreview();
        combatManager.SubmitPlayerAction(action, targets);
        pendingAction = null;
    }

    private void ShowTargetSelection(AbilityTargetType targetType)
    {
        ShowPanel(targetSelectPanel, true);
        ShowPanel(actionMenuPanel, false);
        ShowPanel(abilityMenuPanel, false);
        ShowPanel(itemMenuPanel, false);
        PreviewTargets(targetType);
    }

    private void ShowPanel(GameObject panel, bool show)
    {
        if (panel != null)
        {
            panel.SetActive(show);
        }
    }

    private void BuildAbilityButtons()
    {
        ClearButtons(abilityButtonContainer);
        if (actionButtonPrefab == null || abilityButtonContainer == null)
        {
            return;
        }

        foreach (AbilityData ability in activePlayer.Abilities)
        {
            AbilityData abilityLocal = ability;
            Button button = Instantiate(actionButtonPrefab, abilityButtonContainer);
            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = abilityLocal != null ? abilityLocal.abilityName : "Habilidad";
            }

            button.onClick.AddListener(() => OnAbilityPressed(abilityLocal));
            spawnedButtons.Add(button);
        }
    }

    private void BuildItemButtons()
    {
        ClearButtons(itemButtonContainer);
        if (actionButtonPrefab == null || itemButtonContainer == null)
        {
            return;
        }

        foreach (ItemData item in activePlayer.Items)
        {
            ItemData itemLocal = item;
            Button button = Instantiate(actionButtonPrefab, itemButtonContainer);
            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = itemLocal != null ? itemLocal.itemName : "Objeto";
            }

            button.onClick.AddListener(() => OnItemPressed(itemLocal));
            spawnedButtons.Add(button);
        }
    }

    private void ClearButtons(Transform container)
    {
        if (container == null)
        {
            return;
        }

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        spawnedButtons.Clear();
    }

    private void PreviewTargets(AbilityTargetType targetType)
    {
        ClearTargetPreview();
        if (combatManager == null || activePlayer == null)
        {
            return;
        }

        IReadOnlyList<ICombatant> targets = combatManager.GetValidTargets(targetType, activePlayer);
        foreach (ICombatant combatant in targets)
        {
            BaseCharacter character = combatant as BaseCharacter;
            if (character == null)
            {
                continue;
            }

            TargetHighlight highlight = character.GetComponentInChildren<TargetHighlight>();
            if (highlight != null)
            {
                highlight.SetHighlighted(true);
                highlightedTargets.Add(highlight);
            }
        }
    }

    private void ClearTargetPreview()
    {
        foreach (TargetHighlight highlight in highlightedTargets)
        {
            if (highlight != null)
            {
                highlight.SetHighlighted(false);
            }
        }

        highlightedTargets.Clear();
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
