/// <summary>
/// Interfaz marcadora para efectos de estado que bloquean la acción del combatiente durante un turno.
/// Cualquier IStatusEffect que implemente esta interfaz impedirá que el combatiente actúe
/// mientras el efecto esté activo.
/// </summary>
public interface IActionBlockingEffect : IStatusEffect
{
    // Interfaz marcadora — ningún miembro adicional requerido.
}
