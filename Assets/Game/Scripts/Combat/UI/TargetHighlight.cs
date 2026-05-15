using UnityEngine;

public class TargetHighlight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color highlightColor = new Color(1f, 0.9f, 0.5f, 1f);
    [SerializeField] private Color hoverColor = new Color(1f, 0.7f, 0.2f, 1f);

    private Color baseColor = Color.white;
    private bool isHighlighted;
    private bool isHovered;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            baseColor = spriteRenderer.color;
        }
    }

    public void SetBaseColor(Color color)
    {
        baseColor = color;
        ApplyColor();
    }

    public void SetHighlighted(bool highlighted)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        isHighlighted = highlighted;
        ApplyColor();
    }

    public void SetHovered(bool hovered)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        isHovered = hovered;
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (isHovered)
        {
            spriteRenderer.color = hoverColor;
            return;
        }

        spriteRenderer.color = isHighlighted ? highlightColor : baseColor;
    }
}
