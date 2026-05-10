using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Servicio de telemetría simple para el juego.
/// Registra métricas de juego útiles para balanceo y depuración.
/// Los datos se almacenan en memoria durante la sesión y se pueden exportar a un
/// archivo de texto o enviarse a un servicio externo.
///
/// Métricas actuales:
/// - Número de combates iniciados.
/// - Número de victorias y derrotas.
/// - Número de huidas.
/// - Duración promedio de combate.
/// - Uso de habilidades (conteo por nombre).
/// - Número de muertes de personajes del jugador.
///
/// Uso:
/// - Añadir al mismo GameObject que CombatManager.
/// - Asignar el CombatManager en el Inspector.
/// - Al finalizar la sesión, llamar ExportarMetricas() para ver el resumen.
///
/// Corrida en frío:
/// 1. CombatManager dispara CombatEnded.
/// 2. TelemetryService actualiza victorias/derrotas y calcula duración.
/// 3. CombatManager dispara CombatLog por cada acción.
/// 4. TelemetryService analiza el mensaje para contabilizar uso de habilidades.
/// </summary>
public class TelemetryService : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;

    // ── Métricas ──────────────────────────────────────────────────────────────

    private int combatesIniciados = 0;
    private int victorias = 0;
    private int derrotas = 0;
    private int huidas = 0;
    private float tiempoInicioCombaTE = 0f;
    private float duracionTotalCombates = 0f;
    private int combatesFinalizados = 0;

    private readonly Dictionary<string, int> usoHabilidades = new Dictionary<string, int>();
    private readonly List<string> logSesion = new List<string>();

    // ── Ciclo de vida ─────────────────────────────────────────────────────────

    private void OnEnable()
    {
        if (combatManager != null)
        {
            combatManager.CombatEnded += OnCombatEnded;
            combatManager.CombatLog += OnCombatLog;
            combatManager.TurnStarted += OnTurnStarted;
        }
    }

    private void OnDisable()
    {
        if (combatManager != null)
        {
            combatManager.CombatEnded -= OnCombatEnded;
            combatManager.CombatLog -= OnCombatLog;
            combatManager.TurnStarted -= OnTurnStarted;
        }
    }

    private void OnApplicationQuit()
    {
        ExportarMetricas();
    }

    // ── API pública ───────────────────────────────────────────────────────────

    /// <summary>Notifica al servicio que un combate ha comenzado.</summary>
    public void RegistrarInicioCombate()
    {
        combatesIniciados++;
        tiempoInicioCombaTE = Time.realtimeSinceStartup;
    }

    /// <summary>Registra el uso de una habilidad o acción por nombre.</summary>
    public void RegistrarUsoHabilidad(string nombreHabilidad)
    {
        if (string.IsNullOrEmpty(nombreHabilidad))
        {
            return;
        }

        if (!usoHabilidades.ContainsKey(nombreHabilidad))
        {
            usoHabilidades[nombreHabilidad] = 0;
        }

        usoHabilidades[nombreHabilidad]++;
    }

    /// <summary>Imprime todas las métricas en la consola de Unity.</summary>
    public void ExportarMetricas()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("=== TELEMETRÍA DE SESIÓN ===");
        sb.AppendLine($"Combates iniciados:  {combatesIniciados}");
        sb.AppendLine($"Victorias:           {victorias}");
        sb.AppendLine($"Derrotas:            {derrotas}");
        sb.AppendLine($"Huidas:              {huidas}");

        if (combatesFinalizados > 0)
        {
            float promedio = duracionTotalCombates / combatesFinalizados;
            sb.AppendLine($"Duración promedio:   {promedio:F1}s por combate");
        }

        sb.AppendLine("\n--- Uso de habilidades ---");
        foreach (KeyValuePair<string, int> entry in usoHabilidades)
        {
            sb.AppendLine($"  {entry.Key}: {entry.Value} usos");
        }

        sb.AppendLine("===========================");
        Debug.Log(sb.ToString());
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private void OnCombatEnded(CombatResult result)
    {
        float duracion = Time.realtimeSinceStartup - tiempoInicioCombaTE;
        duracionTotalCombates += duracion;
        combatesFinalizados++;

        switch (result)
        {
            case CombatResult.Victory:
                victorias++;
                break;
            case CombatResult.Defeat:
                derrotas++;
                break;
            case CombatResult.Fled:
                huidas++;
                break;
        }
    }

    private void OnCombatLog(string message)
    {
        logSesion.Add(message);

        // Detectar uso de habilidades en el log de combate.
        if (message.Contains("usa "))
        {
            int inicio = message.IndexOf("usa ") + 4;
            string habilidad = message.Substring(inicio).Trim('.');
            RegistrarUsoHabilidad(habilidad);
        }
    }

    private void OnTurnStarted(ICombatant combatante)
    {
        if (combatante is PlayerCharacter && combatesIniciados == 0)
        {
            RegistrarInicioCombate();
        }
    }
}
