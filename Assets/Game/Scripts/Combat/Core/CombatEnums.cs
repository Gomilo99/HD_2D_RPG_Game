public enum StatType
{
    Cordura,
    Inteligencia,
    Memoria,
    Rapidez,
    Fealdad
}

public enum AbilityEffectType
{
    Damage,
    DebuffIntelligence,
    DebuffMemory,
    BuffMemory,
    Heal
}

public enum AbilityTargetType
{
    SingleEnemy,
    AllEnemies,
    SingleAlly,
    AllAllies,
    Self
}

public enum ItemEffectType
{
    Heal,
    BuffMemory,
    Revive
}

public enum CombatResult
{
    Ongoing,
    Victory,
    Defeat,
    Fled
}
