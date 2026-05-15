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
    Paralyze,
    /// <summary>Revive a un aliado con vida en 0.</summary>
    Revive
}

public enum AbilityTargetType
{
    SingleEnemy,
    AllEnemies,
    SingleAlly,
    AllAllies,
    Self,
    /// <summary>Selecciona aliados derrotados (vida 0).</summary>
    SingleDownedAlly,
    /// <summary>Selecciona todos los aliados derrotados (vida 0).</summary>
    AllDownedAllies
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
