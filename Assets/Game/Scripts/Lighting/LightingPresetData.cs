using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// ScriptableObject que define un preset de atmósfera de iluminación para el mundo HD-2D.
/// Cada preset controla luz ambiental, color de cielo y parámetros de post-proceso
/// (color grading, viñeta, bloom).
///
/// Presets recomendados para el vertical slice:
/// - Pueblo (día): luz cálida, intensidad alta, bloom mínimo.
/// - Interior:     luz tenue, warmth bajo, viñeta pronunciada.
/// - Noche:        luz azulada oscura, bloom tenue para efectos mágicos.
///
/// Uso:
/// - Crea un asset con clic derecho → RPG/Lighting Preset
/// - Configura los parámetros deseados.
/// - Asigna al AtmosphereManager en el Inspector o llama LoadPreset() por código.
/// </summary>
[CreateAssetMenu(fileName = "NewLightingPreset", menuName = "RPG/Lighting Preset")]
public class LightingPresetData : ScriptableObject
{
    [Header("Identificación")]
    public string presetName = "Preset";

    [Header("Luz global / ambiental")]
    [Tooltip("Color de la luz global (luz direccional principal o ambient light).")]
    public Color globalLightColor = Color.white;

    [Tooltip("Intensidad de la luz global (0 = oscuridad total, 1 = máxima).")]
    [Range(0f, 1f)]
    public float globalLightIntensity = 0.8f;

    [Header("Color ambiental de Unity (RenderSettings.ambientLight)")]
    public Color ambientColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    [Header("Niebla")]
    public bool fogEnabled = false;
    public Color fogColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [Range(0f, 0.1f)] public float fogDensity = 0.01f;

    [Header("Post-proceso: Color Grading")]
    [Tooltip("Temperatura de color (-100 frío → +100 cálido).")]
    [Range(-100f, 100f)]
    public float colorTemperature = 0f;

    [Tooltip("Saturación de la imagen (-100 = sin color → +100 muy saturado).")]
    [Range(-100f, 100f)]
    public float saturation = 0f;

    [Header("Post-proceso: Bloom")]
    public bool bloomEnabled = false;
    [Range(0f, 1f)] public float bloomThreshold = 0.9f;
    [Range(0f, 1f)] public float bloomIntensity = 0.2f;

    [Header("Post-proceso: Viñeta")]
    public bool vignetteEnabled = false;
    public Color vignetteColor = Color.black;
    [Range(0f, 1f)] public float vignetteIntensity = 0.3f;
    [Range(0.01f, 1f)] public float vignetteSmoothness = 0.2f;

    [Header("Duración de transición (segundos)")]
    [Min(0f)] public float transitionDuration = 1.0f;
}
