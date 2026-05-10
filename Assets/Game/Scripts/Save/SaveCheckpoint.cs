using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Punto de guardado interactuable en el mundo.
/// Implementa IInteractable para que el jugador pueda guardar su partida
/// al interactuar con él (ej: una estatua, una hoguera, un banco de trabajo).
///
/// Al interactuar:
/// - Guarda automáticamente en el slot activo (configurable en Inspector).
/// - Reproduce el efecto visual y/o sonoro asignado.
///
/// Implementa ICheckpointProvider para reportar la posición actual del jugador.
///
/// Corrida en frío:
/// 1. El jugador presiona "Interact" frente al checkpoint.
/// 2. Interact() registra este componente como ICheckpointProvider en SaveManager.
/// 3. SaveManager.Save(slot) captura los datos del juego y los escribe en disco.
/// </summary>
public class SaveCheckpoint : MonoBehaviour, IInteractable, ICheckpointProvider
{
    [SerializeField] private int saveSlot = 1;
    [SerializeField] private string cityName = "Ciudad desconocida";
    [SerializeField] private GameObject playerReference;
    [SerializeField] private AudioSource saveSound;

    // ── IInteractable ─────────────────────────────────────────────────────────

    public string InteractPrompt => "Guardar partida";
    public bool CanInteract => true;

    public void Interact(GameObject interactor)
    {
        // Asignar como proveedor de checkpoint activo.
        if (interactor != null)
        {
            playerReference = interactor;
        }

        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveCheckpoint: SaveManager no encontrado en la escena.", this);
            return;
        }

        SaveManager.Instance.SetCheckpointProvider(this);
        bool exito = SaveManager.Instance.Save(saveSlot);

        if (exito && saveSound != null)
        {
            saveSound.Play();
        }
    }

    // ── ICheckpointProvider ───────────────────────────────────────────────────

    public float PositionX => playerReference != null ? playerReference.transform.position.x : transform.position.x;
    public float PositionY => playerReference != null ? playerReference.transform.position.y : transform.position.y;
    public float PositionZ => playerReference != null ? playerReference.transform.position.z : transform.position.z;
    public string CurrentSceneName => SceneManager.GetActiveScene().name;
    public string CurrentCityName => cityName;
}
