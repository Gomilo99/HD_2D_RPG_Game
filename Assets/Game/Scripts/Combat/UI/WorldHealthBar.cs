using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BaseCharacter))]
public class WorldHealthBar : MonoBehaviour
{
    [SerializeField] private BaseCharacter target;
    [SerializeField] private Image healthFill;
    [SerializeField] private Image damageFill;
    [SerializeField, Min(0f)] private float damageLagDelay = 0.25f;
    [SerializeField, Min(0.1f)] private float damageLerpSpeed = 4f;

    private Coroutine damageRoutine;
    private bool isReady;

    private void Awake()
    {
        isReady = ValidateReferences();
        if (!isReady)
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (!isReady)
        {
            return;
        }

        target.HealthChanged += HandleHealthChanged;

        RefreshInstant();
    }

    private void OnDisable()
    {
        if (!isReady)
        {
            return;
        }

        target.HealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(ICombatant combatant, int previousHealth, int currentHealth)
    {
        RefreshWithLag();
    }

    private void RefreshInstant()
    {
        float ratio = GetHealthRatio();
        healthFill.fillAmount = ratio;
        damageFill.fillAmount = ratio;
    }

    private void RefreshWithLag()
    {
        float ratio = GetHealthRatio();
        healthFill.fillAmount = ratio;

        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
        }

        damageRoutine = StartCoroutine(AnimateDamageFill(ratio));
    }

    private IEnumerator AnimateDamageFill(float targetFill)
    {
        yield return new WaitForSeconds(damageLagDelay);

        while (damageFill != null && !Mathf.Approximately(damageFill.fillAmount, targetFill))
        {
            damageFill.fillAmount = Mathf.MoveTowards(damageFill.fillAmount, targetFill, damageLerpSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private float GetHealthRatio()
    {
        if (target == null || target.MaxHealth <= 0)
        {
            return 0f;
        }

        return Mathf.Clamp01((float)target.CurrentHealth / target.MaxHealth);
    }

    private bool ValidateReferences()
    {
        if (target == null)
        {
            Debug.LogWarning("WorldHealthBar: BaseCharacter no asignado.", this);
            return false;
        }

        if (healthFill == null)
        {
            Debug.LogWarning("WorldHealthBar: healthFill no asignado.", this);
            return false;
        }
        if (damageFill == null)
        {
            Debug.LogWarning("WorldHealthBar: damageFill no asignado.", this);
            return false;
        }

        return true;
    }
}
