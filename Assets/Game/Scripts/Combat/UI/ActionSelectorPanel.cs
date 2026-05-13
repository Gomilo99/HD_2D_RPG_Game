using UnityEngine;

public class ActionSelectorPanel : MonoBehaviour, IActionSelector
{
    [SerializeField] private BattleUIController battleUI;

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
    }

    public void SetBattleUI(BattleUIController ui)
    {
        battleUI = ui;
    }

    public void RequestAction(PlayerCharacter player, CombatManager combatManager)
    {
        if (battleUI == null)
        {
            Debug.LogWarning("ActionSelectorPanel: BattleUIController no asignado.", this);
            return;
        }

        battleUI.RequestAction(player, combatManager);
    }
}
