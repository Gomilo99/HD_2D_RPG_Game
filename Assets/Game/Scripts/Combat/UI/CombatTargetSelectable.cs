using UnityEngine;
using UnityEngine.EventSystems;

public class CombatTargetSelectable : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BattleUIController battleUI;
    [SerializeField] private BaseCharacter character;
    [SerializeField] private TargetHighlight targetHighlight;

    private int lastSelectFrame = -1;

    public void SetBattleUI(BattleUIController ui)
    {
        battleUI = ui;
    }

    private void Awake()
    {
        if (character == null)
        {
            character = GetComponent<BaseCharacter>();
        }

        if (targetHighlight == null)
        {
            targetHighlight = GetComponentInChildren<TargetHighlight>();
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
        TrySelect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TrySelect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetHighlight != null && battleUI != null && battleUI.IsTargetSelectionActive)
        {
            targetHighlight.SetHovered(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetHighlight != null)
        {
            targetHighlight.SetHovered(false);
        }
    }

    private void TrySelect()
    {
        if (battleUI == null || character == null || !battleUI.IsTargetSelectionActive)
        {
            return;
        }

        if (Time.frameCount == lastSelectFrame)
        {
            return;
        }

        lastSelectFrame = Time.frameCount;
        battleUI.OnTargetSelected(character);
    }
}
