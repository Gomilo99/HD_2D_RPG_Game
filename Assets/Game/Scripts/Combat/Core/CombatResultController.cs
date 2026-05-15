using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controlador de resultados de combate (victoria y derrota).
/// Se suscribe a los eventos de CombatManager y muestra el panel correcto al finalizar
/// el combate, con las opciones disponibles según el resultado.
///
/// Dependencias:
/// - CombatManager (evento CombatEnded)
/// - Paneles de UI: victoryPanel, defeatPanel
///
/// Corrida en frío:
/// 1. CombatManager.EndCombat() dispara el evento CombatEnded con el CombatResult.
/// 2. OnCombatEnded() recibe el resultado y llama ShowVictory() o ShowDefeat().
/// 3. Victory: muestra panel de victoria, espera confirmación del jugador, llama a
///    SceneTransitionManager.ReturnToWorld().
/// 4. Defeat: muestra panel de derrota con dos botones:
///    - "Reintentar" → llama a RetryCombat() para reiniciar la batalla.
///    - "Menú principal" → llama a SceneTransitionManager.GoToMainMenu().
///
/// Posibles errores:
/// - CombatManager nulo: suscribirse en Awake asegura que se configure antes del Start.
/// - Panel no asignado: se registra un warning en consola y no se activa el panel.
/// </summary>
public class CombatResultController : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private BattleUIController battleUIController;

    [Header("Panel de Victoria")]
    [SerializeField] private GameObject victoryPanel;

    [Header("Panel de Derrota")]
    [SerializeField] private GameObject defeatPanel;

    [Header("Transición")]
    [SerializeField] private SceneTransitionManager sceneTransitionManager;

    [Header("Pausa antes de mostrar resultado (segundos)")]
    [SerializeField, Min(0f)] private float resultDelay = 1.2f;

    private void Awake()
    {
        OcultarPaneles();

        if (sceneTransitionManager == null)
        {
            sceneTransitionManager = SceneTransitionManager.Instance;
        }
    }

    private void OnEnable()
    {
        if (sceneTransitionManager == null)
        {
            sceneTransitionManager = SceneTransitionManager.Instance;
        }

        if (combatManager != null)
        {
            combatManager.CombatEnded += OnCombatEnded;
        }
    }

    private void OnDisable()
    {
        if (combatManager != null)
        {
            combatManager.CombatEnded -= OnCombatEnded;
        }
    }

    // ── Eventos de botones ────────────────────────────────────────────────────

    /// <summary>Botón "Continuar" en la pantalla de victoria.</summary>
    public void OnContinuePressed()
    {
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.ReturnToWorld();
        }
    }

    /// <summary>Botón "Reintentar" en la pantalla de derrota.</summary>
    public void OnRetryPressed()
    {
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.RetryCombat();
        }
    }

    /// <summary>Botón "Menú principal" en la pantalla de derrota.</summary>
    public void OnMainMenuPressed()
    {
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.GoToMainMenu();
        }
    }

    // ── Lógica interna ────────────────────────────────────────────────────────

    private void OnCombatEnded(CombatResult result)
    {
        battleUIController.HideOverlay();
        if (PlayerData.Instance != null && combatManager != null)
        {
            PlayerData.Instance.UpdatePartyState(combatManager.GetPlayerPartySnapshot());
        }
        StartCoroutine(MostrarResultadoConPausa(result));
    }

    private IEnumerator MostrarResultadoConPausa(CombatResult result)
    {
        yield return new WaitForSeconds(resultDelay);

        switch (result)
        {
            case CombatResult.Victory:
                ShowVictory();
                break;
            case CombatResult.Defeat:
                ShowDefeat();
                break;
            case CombatResult.Fled:
                // Al huir, se regresa directamente al mundo sin pantalla de resultado.
                if (sceneTransitionManager != null)
                {
                    sceneTransitionManager.ReturnToWorld();
                }
                break;
        }
    }

    private void ShowVictory()
    {
        if (victoryPanel == null)
        {
            Debug.LogWarning("CombatResultController: victoryPanel no está asignado.", this);
            return;
        }

        victoryPanel.SetActive(true);
    }

    private void ShowDefeat()
    {
        if (defeatPanel == null)
        {
            Debug.LogWarning("CombatResultController: defeatPanel no está asignado.", this);
            return;
        }

        defeatPanel.SetActive(true);
    }

    private void OcultarPaneles()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (defeatPanel != null)
        {
            defeatPanel.SetActive(false);
        }
    }
}
