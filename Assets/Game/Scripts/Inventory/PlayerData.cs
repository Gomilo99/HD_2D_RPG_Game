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

    [Serializable]
    private class PartyMemberState
    {
        public CharacterStats stats;
        public int currentHealth = 1;
    }

    [SerializeField, Min(0)] private int money = 0;
    [SerializeField] private List<BaseCharacter> partyMembers = new List<BaseCharacter>();
    [SerializeField] private List<PartyMemberState> partyState = new List<PartyMemberState>();

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

    private void OnValidate()
    {
        SyncPartyStateFromMembers();
    }

    /// <summary>Registra un personaje en el equipo del jugador.</summary>
    public void RegisterPartyMember(BaseCharacter character)
    {
        if (character != null && !partyMembers.Contains(character))
        {
            partyMembers.Add(character);
            TryApplyStoredHealth(character);
        }
    }

    /// <summary>Retira un personaje del equipo.</summary>
    public void RemovePartyMember(BaseCharacter character)
    {
        partyMembers.Remove(character);
    }

    public void UpdatePartyState(IEnumerable<BaseCharacter> members)
    {
        if (members == null)
        {
            return;
        }

        foreach (BaseCharacter member in members)
        {
            if (member == null || member.Stats == null)
            {
                continue;
            }

            int health = Mathf.Max(1, member.CurrentHealth);
            PartyMemberState state = FindState(member.Stats);
            if (state == null)
            {
                state = new PartyMemberState { stats = member.Stats };
                partyState.Add(state);
            }

            state.currentHealth = health;
        }
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

    public bool TryApplyStoredHealth(BaseCharacter character)
    {
        if (character == null || character.Stats == null)
        {
            return false;
        }

        PartyMemberState state = FindState(character.Stats);
        if (state == null)
        {
            return false;
        }

        character.SetCurrentHealth(state.currentHealth, false);
        return true;
    }

    private void SyncPartyStateFromMembers()
    {
        if (partyMembers == null)
        {
            return;
        }

        if (partyState == null)
        {
            partyState = new List<PartyMemberState>();
        }

        List<CharacterStats> memberStats = new List<CharacterStats>();
        foreach (BaseCharacter member in partyMembers)
        {
            if (member == null || member.Stats == null)
            {
                continue;
            }

            if (!memberStats.Contains(member.Stats))
            {
                memberStats.Add(member.Stats);
            }
        }

        for (int i = partyState.Count - 1; i >= 0; i--)
        {
            PartyMemberState state = partyState[i];
            if (state == null || state.stats == null || !memberStats.Contains(state.stats))
            {
                partyState.RemoveAt(i);
            }
        }

        foreach (CharacterStats stats in memberStats)
        {
            PartyMemberState state = FindState(stats);
            if (state == null)
            {
                state = new PartyMemberState
                {
                    stats = stats,
                    currentHealth = stats != null ? Mathf.Max(1, stats.maxCordura) : 1
                };
                partyState.Add(state);
            }
        }

        foreach (PartyMemberState state in partyState)
        {
            if (state == null || state.stats == null)
            {
                continue;
            }

            state.currentHealth = Mathf.Clamp(state.currentHealth, 1, state.stats.maxCordura);
        }
    }

    private PartyMemberState FindState(CharacterStats stats)
    {
        foreach (PartyMemberState state in partyState)
        {
            if (state != null && state.stats == stats)
            {
                return state;
            }
        }

        return null;
    }
}
