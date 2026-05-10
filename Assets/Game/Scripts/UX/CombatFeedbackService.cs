using System.Collections;
using UnityEngine;

/// <summary>
/// Servicio de feedback visual y sonoro para el combate.
/// Se suscribe a los eventos de CombatManager y reproduce efectos ante:
/// - Ataque recibido (flash de daño en el sprite del objetivo).
/// - Curación (efecto de partícula verde).
/// - Derrota de un combatiente (efecto de muerte).
/// - Victoria / Derrota del combate (música o jingle).
///
/// Diseño (S de SOLID):
/// - Solo gestiona feedback; no tiene lógica de combate.
/// - Cada efecto es independiente y se activa vía métodos públicos.
///
/// Uso:
/// - Añadir al mismo GameObject que CombatManager.
/// - Asignar los clips de sonido y prefabs de VFX en el Inspector.
///
/// Corrida en frío — Daño:
/// 1. CombatManager ejecuta una acción de daño.
/// 2. El objetivo llama TakeDamage() → su evento StatsChanged se dispara.
/// 3. Si el feedback está suscrito a StatsChanged, reproduce el flash de daño.
///
/// Nota: la suscripción a StatsChanged de cada combatiente se hace en
/// RegistrarCombatientes(), llamado al inicio del combate.
/// </summary>
public class CombatFeedbackService : MonoBehaviour
{
    [Header("Sonidos")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private AudioClip healSFX;
    [SerializeField] private AudioClip defeatSFX;
    [SerializeField] private AudioClip victorySFX;
    [SerializeField] private AudioClip statusEffectSFX;

    [Header("VFX Prefabs")]
    [SerializeField] private GameObject hitVFXPrefab;
    [SerializeField] private GameObject healVFXPrefab;
    [SerializeField] private GameObject defeatVFXPrefab;

    [Header("Flash de daño")]
    [SerializeField, Min(0f)] private float flashDuration = 0.12f;

    [SerializeField] private CombatManager combatManager;

    private void OnEnable()
    {
        if (combatManager != null)
        {
            combatManager.CombatEnded += OnCombatEnded;
            combatManager.CombatLog += OnCombatLog;
        }
    }

    private void OnDisable()
    {
        if (combatManager != null)
        {
            combatManager.CombatEnded -= OnCombatEnded;
            combatManager.CombatLog -= OnCombatLog;
        }
    }

    // ── API pública ───────────────────────────────────────────────────────────

    /// <summary>Reproduce feedback visual y sonoro de impacto en el objetivo indicado.</summary>
    public void PlayHitFeedback(Transform targetTransform)
    {
        ReproducirSonido(attackSFX);
        SpawnVFX(hitVFXPrefab, targetTransform);

        if (targetTransform != null)
        {
            SpriteRenderer sr = targetTransform.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                StartCoroutine(FlashDamage(sr));
            }
        }
    }

    /// <summary>Reproduce feedback de curación.</summary>
    public void PlayHealFeedback(Transform targetTransform)
    {
        ReproducirSonido(healSFX);
        SpawnVFX(healVFXPrefab, targetTransform);
    }

    /// <summary>Reproduce feedback de aplicación de efecto de estado.</summary>
    public void PlayStatusEffectFeedback()
    {
        ReproducirSonido(statusEffectSFX);
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private void OnCombatEnded(CombatResult result)
    {
        switch (result)
        {
            case CombatResult.Victory:
                ReproducirSonido(victorySFX);
                break;
            case CombatResult.Defeat:
                ReproducirSonido(defeatSFX);
                break;
        }
    }

    private void OnCombatLog(string message)
    {
        // Se puede extender para detectar palabras clave en el log y disparar efectos.
        // Por ejemplo: si el mensaje contiene "daño" → play hit SFX.
    }

    private IEnumerator FlashDamage(SpriteRenderer sr)
    {
        if (sr == null)
        {
            yield break;
        }

        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        if (sr != null)
        {
            sr.color = original;
        }
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void SpawnVFX(GameObject prefab, Transform origin)
    {
        if (prefab == null || origin == null)
        {
            return;
        }

        GameObject vfx = Instantiate(prefab, origin.position, Quaternion.identity);
        Destroy(vfx, 3f); // Destruir automáticamente para no acumular objetos.
    }
}
