using UnityEngine;

/// <summary>
/// Detector de encuentros en el mundo.
/// Cuando el jugador colisiona con un enemigo, inicia la transición a la escena de combate.
/// Si se usa la escena de combate separada, la información del enemigo se pasa a través de
/// PlayerData o un contexto estático que el CombatManager cargará al iniciar.
///
/// Dependencias:
/// - Collider 3D en el objeto jugador (trigger o colisión física).
/// - Tag "Enemy" en los objetos enemigos.
/// - SceneTransitionManager (singleton, debe existir en la escena).
///
/// Corrida en frío:
/// 1. El jugador colisiona con un GameObject cuyo tag es "Enemy".
/// 2. Si la transición de escena está configurada → se llama GoToCombat().
/// 3. Si autoStart del CombatManager está habilitado (combate en la misma escena) →
///    se agrega el enemigo directamente y se inicia el combate.
/// </summary>
public class PlayerEncounter : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;

    [Tooltip("Si es true, inicia el combate en la misma escena (modo prototipo).\n" +
             "Si es false, delega a SceneTransitionManager para cargar la escena de combate.")]
    [SerializeField] private bool combatInSameScene = true;

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        if (combatInSameScene)
        {
            IniciarCombateEnMismaEscena(collision.gameObject);
        }
        else
        {
            IniciarTransicionACombate();
        }
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private void IniciarCombateEnMismaEscena(GameObject enemyObject)
    {
        if (combatManager == null)
        {
            Debug.LogWarning("PlayerEncounter: CombatManager no asignado.", this);
            return;
        }

        BaseCharacter enemyCharacter = enemyObject.GetComponent<BaseCharacter>();
        if (enemyCharacter != null)
        {
            combatManager.SetEnemyToList(enemyCharacter);
        }

        combatManager.StartCombat();
    }

    private void IniciarTransicionACombate()
    {
        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogWarning("PlayerEncounter: SceneTransitionManager no encontrado en la escena.", this);
            return;
        }

        SceneTransitionManager.Instance.GoToCombat();
    }
}
