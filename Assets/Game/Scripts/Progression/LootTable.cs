using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject que define la tabla de loot de un enemigo.
/// Al ser activada (normalmente cuando el enemigo muere), evalúa cada entrada
/// y determina qué objetos y dinero se otorgan al jugador.
///
/// Uso:
/// - Crea un asset con clic derecho → RPG/Loot Table
/// - Añade entradas con los objetos posibles y sus probabilidades.
/// - También define la experiencia que otorga el enemigo.
/// - Asigna este asset al componente EnemyCharacter en el Inspector.
///
/// Corrida en frío:
/// 1. EnemyCharacter.OnDefeated() llama lootTable.Evaluate().
/// 2. Por cada LootEntry, se genera un valor Random.value entre 0 y 1.
/// 3. Si el valor <= dropChance → el ítem se añade al PlayerInventory.
/// 4. El dinero de todas las entradas activadas se suma y se entrega con AddMoney().
/// </summary>
[CreateAssetMenu(fileName = "NewLootTable", menuName = "RPG/Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField] private List<LootEntry> entries = new List<LootEntry>();

    [Tooltip("Experiencia que otorga el enemigo al ser derrotado.")]
    [SerializeField, Min(0)] private int experienceReward = 50;

    /// <summary>Experiencia que se entrega al equipo al derrotar a este enemigo.</summary>
    public int ExperienceReward => experienceReward;

    /// <summary>
    /// Evalúa la tabla de loot y entrega los objetos y dinero resultantes
    /// al inventario y al saldo del jugador.
    ///
    /// Retorna la lista de objetos efectivamente soltados (puede estar vacía).
    /// </summary>
    public IReadOnlyList<ItemData> Evaluate()
    {
        List<ItemData> dropped = new List<ItemData>();

        if (entries == null)
        {
            return dropped;
        }

        int totalMoney = 0;

        foreach (LootEntry entry in entries)
        {
            if (entry == null || entry.item == null)
            {
                continue;
            }

            if (Random.value <= entry.dropChance)
            {
                int quantity = Random.Range(entry.minQuantity, entry.maxQuantity + 1);

                if (PlayerInventory.Instance != null)
                {
                    PlayerInventory.Instance.AddItem(entry.item, quantity);
                }

                dropped.Add(entry.item);
                totalMoney += entry.moneyDrop;
            }
        }

        if (totalMoney > 0 && PlayerData.Instance != null)
        {
            PlayerData.Instance.AddMoney(totalMoney);
        }

        return dropped;
    }
}
