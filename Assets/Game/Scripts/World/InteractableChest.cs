using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cofre interactuable del mundo.
/// Contiene una lista de objetos predefinidos que se entregan al jugador la primera vez
/// que interactúa con él. Tras abrirse, el cofre queda vacío y no puede volverse a abrir.
///
/// Implementa IInteractable para integrarse con el sistema de interacción del jugador.
///
/// Dependencias:
/// - PlayerInventory (para añadir los objetos al inventario).
/// - Animator (opcional, para animación de apertura).
///
/// Corrida en frío:
/// 1. El jugador se acerca al cofre → el sistema de interacción del jugador detecta
///    IInteractable y muestra el prompt.
/// 2. El jugador presiona el botón de interacción → se llama Interact(player).
/// 3. El cofre verifica que CanInteract == true y entrega los objetos al inventario.
/// 4. Se dispara ChestOpened y se activa la animación de apertura.
/// 5. opened = true → CanInteract devuelve false; el cofre no puede abrirse de nuevo.
///
/// Posibles errores:
/// - PlayerInventory.Instance nulo: los objetos se pierden; asegurar que PlayerInventory
///   exista en la escena o persista con DontDestroyOnLoad.
/// </summary>
public class InteractableChest : MonoBehaviour, IInteractable
{
    [Header("Contenido del cofre")]
    [SerializeField] private List<ItemData> items = new List<ItemData>();
    [SerializeField, Min(0)] private int moneyReward = 0;

    [Header("Visual")]
    [SerializeField] private Animator chestAnimator;
    [SerializeField] private string openAnimationTrigger = "Open";

    private bool opened = false;

    /// <summary>Se dispara cuando el cofre se abre por primera vez.</summary>
    public event Action<InteractableChest> ChestOpened;

    // ── IInteractable ─────────────────────────────────────────────────────────

    public string InteractPrompt => opened ? "Vacío" : "Abrir cofre";
    public bool CanInteract => !opened;

    public void Interact(GameObject interactor)
    {
        if (opened)
        {
            return;
        }

        opened = true;
        EntregarContenido();
        ReproducirAnimacion();
        ChestOpened?.Invoke(this);
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private void EntregarContenido()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogWarning("InteractableChest: PlayerInventory no encontrado en la escena.", this);
            return;
        }

        foreach (ItemData item in items)
        {
            if (item != null)
            {
                PlayerInventory.Instance.AddItem(item, 1);
            }
        }

        if (moneyReward > 0 && PlayerData.Instance != null)
        {
            PlayerData.Instance.AddMoney(moneyReward);
        }
    }

    private void ReproducirAnimacion()
    {
        if (chestAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
        {
            chestAnimator.SetTrigger(openAnimationTrigger);
        }
    }
}
