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
    [SerializeField] private TextMeshProUGUI turnInfoText;
    [SerializeField, Min(1)] private int maxLogLines = 6;
    [SerializeField] private Transform abilityButtonContainer;
    [SerializeField] private Transform itemButtonContainer;
    [SerializeField] private Button actionButtonPrefab;

    private readonly Queue<string> logLines = new Queue<string>();
    private readonly List<Button> spawnedButtons = new List<Button>();
    private readonly List<TargetHighlight> highlightedTargets = new List<TargetHighlight>();
    private readonly HashSet<BaseCharacter> validTargets = new HashSet<BaseCharacter>();

    private PlayerCharacter activePlayer;
    private ICombatAction pendingAction;

    public bool IsTargetSelectionActive => pendingAction != null;
    public bool CanSelectTarget(BaseCharacter target)
    {
        return pendingAction != null && target != null && validTargets.Contains(target);
    }

    private void OnDisable()
    {
        if (combatManager != null)
        {
            combatManager.CombatLog -= HandleCombatLog;
            combatManager.TurnStarted -= HandleTurnStarted;
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
            combatManager.TurnStarted -= HandleTurnStarted;
            combatManager.TurnStarted += HandleTurnStarted;
        }

        activePlayer = player;
        pendingAction = null;
        ClearTargetPreview();
        ShowPanel(actionMenuPanel, true);
        ShowPanel(targetSelectPanel, false);
        ShowPanel(abilityMenuPanel, false);
        ShowPanel(itemMenuPanel, false);

        HandleTurnStarted(activePlayer);

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

        if (PlayerInventory.Instance == null || !PlayerInventory.Instance.HasItem(item))
        {
            return;
        }

        pendingAction = activePlayer.CreateUseItem(item);
        AbilityTargetType targetType = item.effectType == ItemEffectType.Revive
            ? AbilityTargetType.SingleDownedAlly
            : AbilityTargetType.SingleAlly;
        ShowTargetSelection(targetType);
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

        if (!validTargets.Contains(target))
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
        SetPanelRaycast(targetSelectPanel, false);
        PreviewTargets(targetType);
    }

    private void ShowPanel(GameObject panel, bool show)
    {
        if (panel != null)
        {
            panel.SetActive(show);
        }
    }

    private void SetPanelRaycast(GameObject panel, bool blockRaycasts)
    {
        if (panel == null)
        {
            return;
        }

        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }

        canvasGroup.blocksRaycasts = blockRaycasts;
        canvasGroup.interactable = blockRaycasts;
    }

    private void BuildAbilityButtons()
    {
        ClearButtons(abilityButtonContainer);
        if (actionButtonPrefab == null || abilityButtonContainer == null)
        {
            Debug.LogWarning("BattleUIController: actionButtonPrefab o abilityButtonContainer no asignado.", this);
            return;
        }

        if (activePlayer == null || activePlayer.Abilities.Count == 0)
        {
            Debug.LogWarning("BattleUIController: el jugador no tiene habilidades en CharacterStats.", this);
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
            Debug.LogWarning("BattleUIController: actionButtonPrefab o itemButtonContainer no asignado.", this);
            return;
        }

        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null)
        {
            Debug.LogWarning("BattleUIController: PlayerInventory no encontrado.", this);
            return;
        }

        if (inventory.Consumables == null || inventory.Consumables.Count == 0)
        {
            Debug.LogWarning("BattleUIController: inventario sin consumibles.", this);
            return;
        }


        foreach (PlayerInventory.ConsumableEntry entry in inventory.Consumables)
        {
            if (entry == null || entry.item == null || entry.quantity <= 0)
            {
                continue;
            }

            ItemData itemLocal = entry.item;
            Button button = Instantiate(actionButtonPrefab, itemButtonContainer);
            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = itemLocal != null
                    ? $"{itemLocal.itemName} x{entry.quantity}"
                    : "Objeto";
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

            validTargets.Add(character);
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
        validTargets.Clear();
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

    private void HandleTurnStarted(ICombatant combatant)
    {
        if (turnInfoText != null)
        {
            turnInfoText.text = combatant != null ? $"Turno: {combatant.Name}" : string.Empty;
        }

    }

    public void HideOverlay(){
        messageLogText.gameObject.SetActive(false);
        turnInfoText.gameObject.SetActive(false);
        overlayPanel.gameObject.SetActive(false);
    }
}
