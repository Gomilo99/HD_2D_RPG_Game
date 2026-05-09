using UnityEngine;

[System.Serializable]
public class RuntimeStats
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    public int BaseIntelligence { get; private set; }
    public int BaseMemory { get; private set; }
    public int BaseSpeed { get; private set; }
    public int BaseLuck { get; private set; }

    private int intelligenceModifier;
    private int memoryModifier;
    private int speedModifier;
    private int luckModifier;

    public int Intelligence => Mathf.Max(0, BaseIntelligence + intelligenceModifier);
    public int Memory => Mathf.Max(0, BaseMemory + memoryModifier);
    public int Speed => Mathf.Max(0, BaseSpeed + speedModifier);
    public int Luck => Mathf.Max(0, BaseLuck + luckModifier);

    public RuntimeStats(CharacterStats stats)
    {
        if (stats == null)
        {
            Initialize(1, 1, 1, 1, 1);
            return;
        }

        Initialize(
            Mathf.Max(1, stats.maxCordura),
            Mathf.Max(0, stats.inteligencia),
            Mathf.Max(0, stats.memoria),
            Mathf.Max(0, stats.rapidez),
            Mathf.Max(0, stats.fealdad));
    }

    public RuntimeStats(int maxHealth, int intelligence, int memory, int speed, int luck)
    {
        Initialize(maxHealth, intelligence, memory, speed, luck);
    }

    private void Initialize(int maxHealth, int intelligence, int memory, int speed, int luck)
    {
        MaxHealth = Mathf.Max(1, maxHealth);
        CurrentHealth = MaxHealth;
        BaseIntelligence = Mathf.Max(0, intelligence);
        BaseMemory = Mathf.Max(0, memory);
        BaseSpeed = Mathf.Max(0, speed);
        BaseLuck = Mathf.Max(0, luck);
    }

    public void ApplyDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
    }

    public void ModifyStat(StatType stat, int amount)
    {
        switch (stat)
        {
            case StatType.Inteligencia:
                intelligenceModifier += amount;
                break;
            case StatType.Memoria:
                memoryModifier += amount;
                break;
            case StatType.Rapidez:
                speedModifier += amount;
                break;
            case StatType.Fealdad:
                luckModifier += amount;
                break;
        }
    }
}
