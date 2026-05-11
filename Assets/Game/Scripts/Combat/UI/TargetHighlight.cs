using UnityEngine;

public class TargetHighlight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color highlightColor = new Color(1f, 0.9f, 0.5f, 1f);

    private Color originalColor = Color.white;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void SetHighlighted(bool highlighted)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.color = highlighted ? highlightColor : originalColor;
    }
}
