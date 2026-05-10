using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Datos globales del jugador (party, dinero).
/// Singleton persistente entre escenas.
///
/// Responsabilidades:
/// - Mantener la referencia a todos los personajes del equipo (variables).
/// - Gestionar el dinero del jugador.
///
/// Corrida en frío:
/// 1. Al inicio del juego PlayerData.Instance se crea vía DontDestroyOnLoad.
/// 2. Los personajes del equipo se registran con RegisterPartyMember().
/// 3. Al ganar dinero en tienda/loot → AddMoney(amount).
/// 4. Al gastar → SpendMoney(amount) devuelve true si hay saldo suficiente.
/// </summary>
public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    [SerializeField, Min(0)] private int money = 0;
    [SerializeField] private List<BaseCharacter> partyMembers = new List<BaseCharacter>();

    /// <summary>Dinero actual del jugador.</summary>
    public int Money => money;

    /// <summary>Vista de solo lectura de los miembros del equipo.</summary>
    public IReadOnlyList<BaseCharacter> PartyMembers => partyMembers;

    /// <summary>Se dispara cuando el dinero cambia. Parámetro: cantidad nueva.</summary>
    public event Action<int> MoneyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Registra un personaje en el equipo del jugador.</summary>
    public void RegisterPartyMember(BaseCharacter character)
    {
        if (character != null && !partyMembers.Contains(character))
        {
            partyMembers.Add(character);
        }
    }

    /// <summary>Retira un personaje del equipo.</summary>
    public void RemovePartyMember(BaseCharacter character)
    {
        partyMembers.Remove(character);
    }

    /// <summary>Añade dinero al saldo. Ignora valores negativos.</summary>
    public void AddMoney(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        money += amount;
        MoneyChanged?.Invoke(money);
    }

    /// <summary>
    /// Intenta gastar la cantidad indicada. Retorna true si había saldo suficiente.
    /// </summary>
    public bool SpendMoney(int amount)
    {
        if (amount <= 0 || money < amount)
        {
            return false;
        }

        money -= amount;
        MoneyChanged?.Invoke(money);
        return true;
    }
}
