using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TargetHighlight), typeof(BaseCharacter))]
public class CombatTargetSelectable : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BattleUIController battleUI;
    [SerializeField] private BaseCharacter character;
    [SerializeField] private TargetHighlight targetHighlight;

    private int lastSelectFrame = -1;
    private bool isReady;

    public void SetBattleUI(BattleUIController ui)
    {
        battleUI = ui;
    }

    private void Awake()
    {
        character = GetComponent<BaseCharacter>();
        targetHighlight = GetComponent<TargetHighlight>();

    }

    private void Start()
    {
        if (battleUI == null)
        {
            battleUI = FindFirstObjectByType<BattleUIController>();
        }

        if (battleUI == null)
        {
            Debug.LogWarning("CombatTargetSelectable: BattleUIController no encontrado.", this);
            enabled = false;
            return;
        }

        isReady = true;
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
        if (!isReady)
        {
            return;
        }

        if (battleUI.CanSelectTarget(character))
        {
            targetHighlight.SetHovered(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetHighlight.SetHovered(false);
    }

    private void TrySelect()
    {
        if (!isReady || !battleUI.CanSelectTarget(character))
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
