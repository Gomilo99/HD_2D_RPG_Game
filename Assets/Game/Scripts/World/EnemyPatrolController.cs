using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador de patrulla y persecución de enemigos en el mundo.
/// El enemigo se mueve entre una serie de puntos de patrulla.
/// Cuando el jugador entra en el radio de detección, lo persigue.
/// Cuando el jugador sale del radio de agro (mayor), vuelve a patrullar.
///
/// Dependencias:
/// - Rigidbody (para movimiento físico, coherente con PlayerController).
/// - La detección usa distancia (sin NavMesh), sencillo y portable.
///
/// Corrida en frío:
/// 1. Awake configura el Rigidbody y establece el punto de patrulla actual.
/// 2. Update evalúa el estado (Patrolling / Chasing / Returning):
///    a. Patrolling: se mueve hacia el punto actual; al llegar espera waitTime y avanza.
///    b. Chasing: si encuentra al jugador en detectionRange, lo persigue.
///       Si el jugador sale de aggroRange → Returning.
///    c. Returning: regresa al último waypoint de patrulla antes de reanudar.
/// 3. Al colisionar con el jugador, PlayerEncounter gestiona el inicio del combate.
///
/// Posibles errores:
/// - waypoints vacío: el enemigo permanece estático y no patrulla.
/// - playerTag incorrecto: la detección no funciona; verificar el tag en el Inspector.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EnemyPatrolController : MonoBehaviour
{
    public enum PatrolState { Patrolling, Chasing, Returning }

    [Header("Patrulla")]
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField, Min(0f)] private float waypointWaitTime = 1.0f;
    [SerializeField, Min(0.1f)] private float waypointReachDistance = 0.3f;

    [Header("Detección del jugador")]
    [SerializeField, Min(0f)] private float detectionRange = 4.0f;
    [SerializeField, Min(0f)] private float aggroRange = 6.0f;
    [SerializeField] private string playerTag = "Player";

    [Header("Estado actual (solo lectura)")]
    [SerializeField] private PatrolState currentState = PatrolState.Patrolling;

    private Rigidbody rb;
    private Transform playerTransform;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    /// <summary>Estado actual de la máquina de estados de patrulla/persecución.</summary>
    public PatrolState CurrentState => currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void FixedUpdate()
    {
        ActualizarEstado();
        EjecutarMovimiento();
    }

    // ── Lógica de estado ──────────────────────────────────────────────────────

    private void ActualizarEstado()
    {
        if (playerTransform == null)
        {
            currentState = PatrolState.Patrolling;
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case PatrolState.Patrolling:
                if (distToPlayer <= detectionRange)
                {
                    currentState = PatrolState.Chasing;
                }
                break;

            case PatrolState.Chasing:
                if (distToPlayer > aggroRange)
                {
                    currentState = PatrolState.Returning;
                }
                break;

            case PatrolState.Returning:
                if (distToPlayer <= detectionRange)
                {
                    // El jugador se acercó de nuevo mientras regresaba.
                    currentState = PatrolState.Chasing;
                }
                break;
        }
    }

    private void EjecutarMovimiento()
    {
        switch (currentState)
        {
            case PatrolState.Patrolling:
                MoverEnPatrulla();
                break;
            case PatrolState.Chasing:
                MoverHacia(playerTransform.position);
                break;
            case PatrolState.Returning:
                MoverHaciaWaypoint();
                break;
        }
    }

    // ── Patrulla ──────────────────────────────────────────────────────────────

    private void MoverEnPatrulla()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            DetenerMovimiento();
            return;
        }

        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                AvanzarWaypoint();
            }

            DetenerMovimiento();
            return;
        }

        MoverHaciaWaypoint();
    }

    private void MoverHaciaWaypoint()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            return;
        }

        Transform target = waypoints[currentWaypointIndex];
        if (target == null)
        {
            AvanzarWaypoint();
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= waypointReachDistance)
        {
            isWaiting = true;
            waitTimer = waypointWaitTime;
            DetenerMovimiento();
            return;
        }

        MoverHacia(target.position);
    }

    private void MoverHacia(Vector3 destino)
    {
        Vector3 direccion = (destino - transform.position).normalized;
        direccion.y = 0f;
        rb.linearVelocity = new Vector3(direccion.x * moveSpeed, rb.linearVelocity.y, direccion.z * moveSpeed);
    }

    private void DetenerMovimiento()
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    private void AvanzarWaypoint()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            return;
        }

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
    }

    // ── Gizmos (editor) ───────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        if (waypoints == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null)
            {
                continue;
            }

            Gizmos.DrawSphere(waypoints[i].position, 0.15f);
            int next = (i + 1) % waypoints.Count;
            if (waypoints[next] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
            }
        }
    }
}
