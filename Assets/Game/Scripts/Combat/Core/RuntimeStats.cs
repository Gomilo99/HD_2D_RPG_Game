using UnityEngine;

[System.Serializable]
public class RuntimeStats
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int baseIntelligence;
    [SerializeField] private int baseMemory;
    [SerializeField] private int baseSpeed;
    [SerializeField] private int baseLuck;

    [SerializeField] private int intelligenceModifier;
    [SerializeField] private int memoryModifier;
    [SerializeField] private int speedModifier;
    [SerializeField] private int luckModifier;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public int BaseIntelligence => baseIntelligence;
    public int BaseMemory => baseMemory;
    public int BaseSpeed => baseSpeed;
    public int BaseLuck => baseLuck;

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
        this.maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = this.maxHealth;
        baseIntelligence = Mathf.Max(0, intelligence);
        baseMemory = Mathf.Max(0, memory);
        baseSpeed = Mathf.Max(0, speed);
        baseLuck = Mathf.Max(0, luck);
    }

    public void ApplyDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
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
