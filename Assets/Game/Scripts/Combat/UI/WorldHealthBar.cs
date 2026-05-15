using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBar : MonoBehaviour
{
    [SerializeField] private BaseCharacter target;
    [SerializeField] private Image healthFill;
    [SerializeField] private Image damageFill;
    [SerializeField, Min(0f)] private float damageLagDelay = 0.25f;
    [SerializeField, Min(0.1f)] private float damageLerpSpeed = 4f;

    private Coroutine damageRoutine;

    private void Awake()
    {
        TryResolveTarget();
    }

    private void OnEnable()
    {
        if (target != null)
        {
            target.StatsChanged += HandleStatsChanged;
            target.HealthChanged += HandleHealthChanged;
        }

        RefreshInstant();
    }

    private void OnDisable()
    {
        if (target != null)
        {
            target.StatsChanged -= HandleStatsChanged;
            target.HealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleStatsChanged(ICombatant combatant)
    {
        RefreshInstant();
    }

    private void HandleHealthChanged(ICombatant combatant, int previousHealth, int currentHealth)
    {
        RefreshWithLag();
    }

    private void RefreshInstant()
    {
        float ratio = GetHealthRatio();
        if (healthFill != null)
        {
            healthFill.fillAmount = ratio;
        }

        if (damageFill != null)
        {
            damageFill.fillAmount = ratio;
        }
    }

    private void RefreshWithLag()
    {
        float ratio = GetHealthRatio();
        if (healthFill != null)
        {
            healthFill.fillAmount = ratio;
        }

        if (damageFill == null)
        {
            return;
        }

        if (ratio >= damageFill.fillAmount)
        {
            damageFill.fillAmount = ratio;
            return;
        }

        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
        }

        damageRoutine = StartCoroutine(AnimateDamageFill(ratio));
    }

    private IEnumerator AnimateDamageFill(float targetFill)
    {
        yield return new WaitForSeconds(damageLagDelay);

        while (damageFill != null && damageFill.fillAmount > targetFill)
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

    private void TryResolveTarget()
    {
        if (target != null)
        {
            return;
        }

        target = GetComponentInParent<BaseCharacter>();
        if (target == null)
        {
            target = GetComponentInChildren<BaseCharacter>();
        }
    }
}
