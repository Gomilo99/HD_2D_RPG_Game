using UnityEngine;

/// <summary>
/// Detector de interacción del jugador con objetos IInteractable del mundo
/// (cofres, NPCs, portales, etc.).
/// Detecta el objeto más cercano dentro del radio de interacción y permite al
/// jugador interactuar con él al presionar el botón de acción.
///
/// Corrida en frío:
/// 1. FixedUpdate hace un OverlapSphere centrado en el jugador con interactionRadius.
/// 2. Se selecciona el IInteractable más cercano que tenga CanInteract == true.
/// 3. Si el jugador presiona el botón de interacción (Input "Interact"):
///    a. Se llama currentTarget.Interact(gameObject).
///    b. El objeto ejecuta su lógica (abrir cofre, mostrar tienda, etc.).
///
/// Para configurar el botón "Interact":
/// - Ve a Project Settings → Input Manager y añade un eje "Interact" con la tecla E.
/// - O usa el nuevo Input System con una Action "Interact".
///
/// Posibles errores:
/// - interactionRadius muy pequeño: aumentarlo en el Inspector si no detecta objetos.
/// - Capas no configuradas: usar interactionLayer para filtrar correctamente.
/// </summary>
public class PlayerInteractionDetector : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float interactionRadius = 1.5f;
    [SerializeField] private LayerMask interactionLayer = ~0;

    private IInteractable currentTarget;

    private void Update()
    {
        ActualizarObjetivoMasCercano();
        ProcesarInput();
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private void ActualizarObjetivoMasCercano()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius, interactionLayer);
        IInteractable closest = null;
        float closestDist = float.MaxValue;

        foreach (Collider col in colliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable == null || !interactable.CanInteract)
            {
                continue;
            }

            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = interactable;
            }
        }

        currentTarget = closest;
    }

    private void ProcesarInput()
    {
        if (currentTarget == null)
        {
            return;
        }

        // Acepta tanto el Input Manager clásico como el Input System vía polling.
        if (Input.GetButtonDown("Interact"))
        {
            currentTarget.Interact(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
