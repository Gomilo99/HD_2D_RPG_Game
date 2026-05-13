using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour, ICombatant
{
    [SerializeField] protected CharacterStats stats;
    [SerializeField] protected List<ItemData> startingItems = new List<ItemData>();

    [SerializeField] protected RuntimeStats runtimeStats;
    private readonly List<IStatusEffect> statusEffects = new List<IStatusEffect>();

    public event Action<ICombatant> StatsChanged;
    public event Action<ICombatant> Defeated;

    public string Name => stats != null && !string.IsNullOrWhiteSpace(stats.characterName)
        ? stats.characterName
        : gameObject.name;
    public bool IsAlive => runtimeStats != null && runtimeStats.CurrentHealth > 0;
    public int Speed => runtimeStats?.Speed ?? 0;
    public int Attack => runtimeStats?.Intelligence ?? 0;
    public int Defense => runtimeStats?.Memory ?? 0;
    public int Luck => runtimeStats?.Luck ?? 0;
    public int MaxHealth => runtimeStats?.MaxHealth ?? 0;
    public int CurrentHealth => runtimeStats?.CurrentHealth ?? 0;
    public IReadOnlyList<ItemData> Items => startingItems;
    public CharacterStats Stats => stats;

    public IReadOnlyList<IStatusEffect> GetStatusEffectsSnapshot()
    {
        return new List<IStatusEffect>(statusEffects);
    }

    /// <summary>
    /// Devuelve true si al menos un efecto de estado activo implementa IActionBlockingEffect,
    /// impidiendo que este combatiente actúe durante su turno.
    /// </summary>
    public bool IsActionBlocked
    {
        get
        {
            foreach (IStatusEffect effect in statusEffects)
            {
                if (effect is IActionBlockingEffect)
                {
                    return true;
                }
            }

            return false;
        }
    }

    protected virtual void Awake()
    {
        runtimeStats = new RuntimeStats(stats);
    }

    public void Initialize(CharacterStats newStats, IReadOnlyList<ItemData> itemsOverride = null)
    {
        stats = newStats;
        if (itemsOverride != null)
        {
            startingItems = new List<ItemData>(itemsOverride);
        }

        runtimeStats = new RuntimeStats(stats);
    }

    public void TakeDamage(int amount)
    {
        if (runtimeStats == null)
        {
            return;
        }

        runtimeStats.ApplyDamage(amount);
        StatsChanged?.Invoke(this);

        if (!IsAlive)
        {
            Defeated?.Invoke(this);
        }
    }

    public void Heal(int amount)
    {
        if (runtimeStats == null)
        {
            return;
        }

        runtimeStats.Heal(amount);
        StatsChanged?.Invoke(this);
    }

    public void ModifyStat(StatType stat, int amount)
    {
        if (runtimeStats == null)
        {
            return;
        }

        runtimeStats.ModifyStat(stat, amount);
        StatsChanged?.Invoke(this);
    }

    public void ApplyStatusEffect(IStatusEffect effect)
    {
        if (effect == null)
        {
            return;
        }

        effect.Apply(this);
        statusEffects.Add(effect);
        StatsChanged?.Invoke(this);
    }

    public void TickStatusEffects()
    {
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            IStatusEffect effect = statusEffects[i];
            effect.Tick(this);
            if (effect.RemainingTurns <= 0)
            {
                effect.Remove(this);
                statusEffects.RemoveAt(i);
            }
        }
    }

    public abstract void ChooseAction(CombatManager combatManager);
}