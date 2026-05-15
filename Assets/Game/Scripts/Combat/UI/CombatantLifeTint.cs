using UnityEngine;

public class CombatantLifeTint : MonoBehaviour
{
    [SerializeField] private BaseCharacter target;
    [SerializeField] private TargetHighlight targetHighlight;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color downedColor = new Color(0.25f, 0.25f, 0.25f, 1f);

    private Color aliveColor = Color.white;

    private void Awake()
    {
        if (target == null)
        {
            target = GetComponentInParent<BaseCharacter>();
        }

        if (targetHighlight == null)
        {
            targetHighlight = GetComponentInChildren<TargetHighlight>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            aliveColor = spriteRenderer.color;
        }
    }

    private void OnEnable()
    {
        if (target != null)
        {
            target.HealthChanged += HandleHealthChanged;
        }

        ApplyTint();
    }

    private void OnDisable()
    {
        if (target != null)
        {
            target.HealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(ICombatant combatant, int previousHealth, int currentHealth)
    {
        ApplyTint();
    }

    private void ApplyTint()
    {
        if (target == null)
        {
            return;
        }

        Color baseColor = target.IsAlive ? aliveColor : downedColor;
        if (targetHighlight != null)
        {
            targetHighlight.SetBaseColor(baseColor);
            return;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = baseColor;
        }
    }
}
