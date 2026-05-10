using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Gestiona la transición entre presets de atmósfera de iluminación.
/// Controla el color ambiental de Unity, la niebla y solicita al VolumeManager
/// los cambios de post-proceso (Bloom, Viñeta, Color Grading) cuando se usa
/// URP con Volumes.
///
/// Diseño:
/// - No depende de luces 2D directamente (es agnóstico al tipo de luz).
/// - Para luces 2D de acento (antorchas, magia): añádelas en la escena y
///   configúralas en los presets de Unity.
/// - Para post-proceso, requiere un Volume Global en la escena con los overrides
///   de Bloom, Vignette y Color Adjustments activos.
///
/// Corrida en frío:
/// 1. LoadPreset(preset) se llama (ej.: al cambiar de zona).
/// 2. Si ya hay una coroutine de transición activa, se detiene.
/// 3. StartCoroutine(TransicionarPreset) interpola valores entre el estado
///    actual y el del nuevo preset durante transitionDuration segundos.
/// 4. RenderSettings.ambientLight se actualiza en tiempo real.
/// 5. Si hay un Volume Global con overrides, se actualiza vía reflexión
///    (o mediante clases auxiliares del VolumeManager de URP).
///
/// Posibles errores:
/// - presetActual nulo al inicio: se aplica el primer preset disponible en Start.
/// - Volume Global no configurado: el post-proceso no cambia, pero la luz ambiental sí.
/// </summary>
public class AtmosphereManager : MonoBehaviour
{
    public static AtmosphereManager Instance { get; private set; }

    [Header("Presets")]
    [SerializeField] private LightingPresetData presetPueblo;
    [SerializeField] private LightingPresetData presetInterior;
    [SerializeField] private LightingPresetData presetNoche;
    [SerializeField] private LightingPresetData presetInicial;

    [Header("Post-proceso")]
    [Tooltip("Volume Global de URP que contiene los overrides de post-proceso.")]
    [SerializeField] private Volume globalVolume;

    private LightingPresetData presetActual;
    private Coroutine transicionActiva;

    // Referencia a los overrides del Volume (soporta Bloom, Vignette, ColorAdjustments de URP).
    private UnityEngine.Rendering.Universal.Bloom bloomOverride;
    private UnityEngine.Rendering.Universal.Vignette vignetteOverride;
    private UnityEngine.Rendering.Universal.ColorAdjustments colorAdjOverride;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ObtenerOverridesDelVolume();

        LightingPresetData inicial = presetInicial != null ? presetInicial : presetPueblo;
        if (inicial != null)
        {
            AplicarInstantaneo(inicial);
        }
    }

    // ── API pública ───────────────────────────────────────────────────────────

    /// <summary>Carga el preset de pueblo (día exterior).</summary>
    public void LoadPresetPueblo() => LoadPreset(presetPueblo);

    /// <summary>Carga el preset de interior (edificios, cuevas).</summary>
    public void LoadPresetInterior() => LoadPreset(presetInterior);

    /// <summary>Carga el preset de noche.</summary>
    public void LoadPresetNoche() => LoadPreset(presetNoche);

    /// <summary>
    /// Carga un preset por referencia directa, con transición suave.
    /// Si el preset es el mismo que el actual, no hace nada.
    /// </summary>
    public void LoadPreset(LightingPresetData preset)
    {
        if (preset == null || preset == presetActual)
        {
            return;
        }

        if (transicionActiva != null)
        {
            StopCoroutine(transicionActiva);
        }

        transicionActiva = StartCoroutine(TransicionarPreset(preset));
    }

    // ── Transición ────────────────────────────────────────────────────────────

    private IEnumerator TransicionarPreset(LightingPresetData destino)
    {
        float duracion = destino.transitionDuration;
        float elapsed = 0f;

        Color ambienteOrigen = RenderSettings.ambientLight;
        bool nieblaOrigen = RenderSettings.fog;
        Color nieblaColorOrigen = RenderSettings.fogColor;
        float nieblaDensidadOrigen = RenderSettings.fogDensity;

        if (duracion <= 0f)
        {
            AplicarInstantaneo(destino);
            presetActual = destino;
            transicionActiva = null;
            yield break;
        }

        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duracion);

            RenderSettings.ambientLight = Color.Lerp(ambienteOrigen, destino.ambientColor, t);

            if (destino.fogEnabled)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = Color.Lerp(nieblaColorOrigen, destino.fogColor, t);
                RenderSettings.fogDensity = Mathf.Lerp(nieblaDensidadOrigen, destino.fogDensity, t);
            }
            else
            {
                RenderSettings.fog = false;
            }

            InterpolarPostProceso(t, destino);

            yield return null;
        }

        AplicarInstantaneo(destino);
        presetActual = destino;
        transicionActiva = null;
    }

    // ── Aplicación directa ────────────────────────────────────────────────────

    private void AplicarInstantaneo(LightingPresetData preset)
    {
        if (preset == null)
        {
            return;
        }

        RenderSettings.ambientLight = preset.ambientColor;
        RenderSettings.fog = preset.fogEnabled;
        RenderSettings.fogColor = preset.fogColor;
        RenderSettings.fogDensity = preset.fogDensity;

        AplicarPostProceso(preset);
        presetActual = preset;
    }

    private void AplicarPostProceso(LightingPresetData preset)
    {
        if (bloomOverride != null)
        {
            bloomOverride.active = preset.bloomEnabled;
            bloomOverride.threshold.value = preset.bloomThreshold;
            bloomOverride.intensity.value = preset.bloomIntensity;
        }

        if (vignetteOverride != null)
        {
            vignetteOverride.active = preset.vignetteEnabled;
            vignetteOverride.color.value = preset.vignetteColor;
            vignetteOverride.intensity.value = preset.vignetteIntensity;
            vignetteOverride.smoothness.value = preset.vignetteSmoothness;
        }

        if (colorAdjOverride != null)
        {
            colorAdjOverride.active = true;
            colorAdjOverride.colorFilter.value = preset.globalLightColor;
            colorAdjOverride.saturation.value = preset.saturation;
            colorAdjOverride.colorTemperature.value = preset.colorTemperature;
        }
    }

    private void InterpolarPostProceso(float t, LightingPresetData destino)
    {
        if (presetActual == null)
        {
            return;
        }

        if (bloomOverride != null && destino.bloomEnabled)
        {
            bloomOverride.active = true;
            bloomOverride.intensity.value = Mathf.Lerp(
                presetActual.bloomIntensity, destino.bloomIntensity, t);
        }

        if (vignetteOverride != null && destino.vignetteEnabled)
        {
            vignetteOverride.active = true;
            vignetteOverride.intensity.value = Mathf.Lerp(
                presetActual.vignetteIntensity, destino.vignetteIntensity, t);
        }

        if (colorAdjOverride != null)
        {
            colorAdjOverride.colorTemperature.value = Mathf.Lerp(
                presetActual.colorTemperature, destino.colorTemperature, t);
            colorAdjOverride.saturation.value = Mathf.Lerp(
                presetActual.saturation, destino.saturation, t);
        }
    }

    // ── Inicialización de overrides ───────────────────────────────────────────

    private void ObtenerOverridesDelVolume()
    {
        if (globalVolume == null)
        {
            return;
        }

        globalVolume.profile.TryGet(out bloomOverride);
        globalVolume.profile.TryGet(out vignetteOverride);
        globalVolume.profile.TryGet(out colorAdjOverride);
    }
}
