using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interfaz para objetos del mundo con los que el jugador puede interactuar (cofres, NPCs, etc.).
/// Separa la responsabilidad de la interacción de la lógica específica del objeto.
/// </summary>
public interface IInteractable
{
    /// <summary>Texto o icono que se muestra como prompt al jugador.</summary>
    string InteractPrompt { get; }

    /// <summary>Indica si el objeto sigue siendo interactuable.</summary>
    bool CanInteract { get; }

    /// <summary>Ejecuta la interacción cuando el jugador lo solicita.</summary>
    void Interact(GameObject interactor);
}
