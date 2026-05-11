using UnityEngine;

public class CombatTargetSelectable : MonoBehaviour
{
    [SerializeField] private BattleUIController battleUI;
    [SerializeField] private BaseCharacter character;
    public void SetBattleUI(BattleUIController ui){
        battleUI = ui;
    }

    private void Awake()
    {
        if (character == null)
        {
            character = GetComponent<BaseCharacter>();
        }
    }

    private void Start()
    {
        if (battleUI == null)
        {
            battleUI = FindFirstObjectByType<BattleUIController>();
        }
    }

    private void OnMouseDown()
    {
        if (battleUI != null && character != null)
        {
            battleUI.OnTargetSelected(character);
        }
    }
}
