/// <summary>
/// Interfaz para componentes que proveen datos de checkpoint.
/// Permite al SaveManager obtener información actualizada de posición y escena
/// sin acoplarse a implementaciones específicas.
///
/// Uso:
/// - Implementar en el componente del jugador o en un GameManager de escena.
/// - Registrarse en SaveManager vía SaveManager.Instance.SetCheckpointProvider(this).
/// </summary>
public interface ICheckpointProvider
{
    /// <summary>Posición X del jugador en el mundo.</summary>
    float PositionX { get; }

    /// <summary>Posición Y del jugador en el mundo.</summary>
    float PositionY { get; }

    /// <summary>Posición Z del jugador en el mundo.</summary>
    float PositionZ { get; }

    /// <summary>Nombre de la escena activa donde se encuentra el jugador.</summary>
    string CurrentSceneName { get; }

    /// <summary>Nombre de la ciudad o zona actual.</summary>
    string CurrentCityName { get; }
}
