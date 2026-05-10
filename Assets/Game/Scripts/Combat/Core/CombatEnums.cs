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
    Heal,
    /// <summary>Envenena al objetivo: aplica daño por turno durante varios turnos.</summary>
    Poison,
    /// <summary>Paraliza al objetivo: pierde su próximo turno de acción.</summary>
    Paralyze
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
