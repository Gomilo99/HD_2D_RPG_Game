using System;
using System.Collections.Generic;

/// <summary>
/// Modelo de datos para el guardado de partida.
/// Serializable con JsonUtility para guardar en disco.
///
/// Sección de datos guardados:
/// - Posición del jugador en el mundo.
/// - Nombre de la escena/ciudad actual.
/// - Dinero acumulado.
/// - Tiempo de juego en segundos.
/// - Lista de personajes del equipo con sus estadísticas y nivel.
///
/// Nota de integridad:
/// - El campo checksum contiene un hash básico para detectar corrupción de datos.
///   Se valida al cargar en SaveManager.ValidateChecksum().
/// </summary>
[Serializable]
public class SaveData
{
    /// <summary>Versión del formato de guardado para migraciones futuras.</summary>
    public int version = 1;

    /// <summary>Identificador del slot (1–3).</summary>
    public int slotId = 1;

    /// <summary>Fecha y hora del guardado en formato ISO 8601.</summary>
    public string saveDate = string.Empty;

    // ── Mundo ─────────────────────────────────────────────────────────────────

    public float positionX = 0f;
    public float positionY = 0f;
    public float positionZ = 0f;

    /// <summary>Nombre de la escena donde se guardó.</summary>
    public string sceneName = string.Empty;

    /// <summary>Nombre de la ciudad o zona donde se guardó.</summary>
    public string cityName = string.Empty;

    // ── Recursos del jugador ──────────────────────────────────────────────────

    public int money = 0;

    /// <summary>Tiempo total de juego en segundos.</summary>
    public float playTimeSeconds = 0f;

    // ── Equipo de personajes ──────────────────────────────────────────────────

    public List<CharacterSaveData> party = new List<CharacterSaveData>();

    // ── Integridad ────────────────────────────────────────────────────────────

    /// <summary>Hash simple de integridad calculado al guardar.</summary>
    public int checksum = 0;
}

/// <summary>Datos de un personaje individual guardados en la partida.</summary>
[Serializable]
public class CharacterSaveData
{
    public string characterName = string.Empty;
    public int level = 1;
    public int totalExperience = 0;
    public int currentHealth = 1;
    public int maxHealth = 1;
    public int inteligencia = 10;
    public int memoria = 5;
    public int rapidez = 5;
    public int fealdad = 1;

    /// <summary>Nombres de las habilidades desbloqueadas por progresión.</summary>
    public List<string> unlockedAbilityNames = new List<string>();
}
