using UnityEngine;

public class ActionSelectorPanel : MonoBehaviour, IActionSelector
{
    [SerializeField] private BattleUIController battleUI;

    private bool isReady;

    private void Awake()
    {
        if (battleUI == null)
        {
            battleUI = GetComponentInParent<BattleUIController>();
        }

        if (battleUI == null)
        {
            battleUI = FindFirstObjectByType<BattleUIController>();
        }

        if (battleUI == null)
        {
            Debug.LogWarning("ActionSelectorPanel: BattleUIController no existe.", this);
            enabled = false;
            return;
        }

        isReady = true;
    }

    public void SetBattleUI(BattleUIController ui)
    {
        battleUI = ui;
    }

    public void RequestAction(PlayerCharacter player, CombatManager combatManager)
    {
        if (!isReady)
        {
            return;
        }

        battleUI.RequestAction(player, combatManager);
    }
}
