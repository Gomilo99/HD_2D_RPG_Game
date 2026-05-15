using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona las transiciones entre escenas: mundo ↔ combate y hacia el menú principal.
/// Mantiene contexto de la escena origen para poder regresar correctamente tras la batalla.
/// Singleton persistente entre escenas.
///
/// Dependencias:
/// - SceneManager (Unity).
/// - CombatContextData (objeto ScriptableObject o clase estática que pasa datos de combate).
///
/// Corrida en frío — ir a combate:
/// 1. PlayerEncounter.OnCollisionEnter detecta al enemigo.
/// 2. Llama SceneTransitionManager.Instance.GoToCombat(enemyObject, worldSceneName).
/// 3. Se guarda el nombre de la escena actual como returnScene.
/// 4. Se almacena el contexto del enemigo en CombatContext.
/// 5. Se carga la escena de combate (con fundido si transitionCanvas está asignado).
///
/// Corrida en frío — regresar al mundo:
/// 1. CombatResultController llama ReturnToWorld().
/// 2. Se carga returnScene (la escena del mundo guardada en el paso 3).
///
/// Posibles errores:
/// - returnScene vacía: si ReturnToWorld() se llama sin haber pasado por GoToCombat(),
///   se redirige al menú principal como fallback.
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Nombres de escenas")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string combatSceneName = "CombatScene";

    [Header("Transición visual (opcional)")]
    [SerializeField] private CanvasGroup transitionCanvas;
    [SerializeField, Min(0f)] private float fadeDuration = 0.4f;

    private string returnScene = string.Empty;

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

    // ── API pública ───────────────────────────────────────────────────────────

    /// <summary>
    /// Inicia la transición hacia la escena de combate.
    /// Guarda la escena actual como destino de retorno.
    /// </summary>
    /// <param name="combatScene">Nombre de la escena de combate (opcional, usa el valor por defecto).</param>
    public void GoToCombat(string combatScene = null)
    {
        returnScene = SceneManager.GetActiveScene().name;
        string destino = string.IsNullOrEmpty(combatScene) ? combatSceneName : combatScene;
        StartCoroutine(TransitionTo(destino));
    }

    /// <summary>Regresa a la escena del mundo donde se inició el combate.</summary>
    public void ReturnToWorld()
    {
        if (string.IsNullOrEmpty(returnScene))
        {
            Debug.LogWarning("SceneTransitionManager: returnScene vacía, redirigiendo al menú principal.");
            GoToMainMenu();
            return;
        }

        StartCoroutine(TransitionTo(returnScene));
    }

    /// <summary>Reinicia la escena de combate actual.</summary>
    public void RetryCombat()
    {
        string current = SceneManager.GetActiveScene().name;
        StartCoroutine(TransitionTo(current));
    }

    /// <summary>Carga la escena del menú principal.</summary>
    public void GoToMainMenu()
    {
        returnScene = string.Empty;
        StartCoroutine(TransitionTo(mainMenuSceneName));
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private IEnumerator TransitionTo(string sceneName)
    {
        // Fundido de salida.
        if (transitionCanvas != null)
        {
            yield return StartCoroutine(Fade(0f, 1f));
        }

        SceneManager.LoadScene(sceneName);

        // Fundido de entrada (se ejecuta en la nueva escena).
        if (transitionCanvas != null)
        {
            yield return StartCoroutine(Fade(1f, 0f));
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        if (transitionCanvas == null)
        {
            yield break;
        }

        float elapsed = 0f;
        transitionCanvas.alpha = from;
        transitionCanvas.gameObject.SetActive(true);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            transitionCanvas.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        transitionCanvas.alpha = to;

        if (Mathf.Approximately(to, 0f))
        {
            transitionCanvas.gameObject.SetActive(false);
        }
    }
}
