using System;
using UnityEngine;

/// <summary>
/// Una entrada en la tabla de crecimiento de nivel.
/// Define cuánta experiencia se necesita para alcanzar el nivel y cuánto
/// crecen las estadísticas al llegar a él.
/// </summary>
[Serializable]
public class LevelEntry
{
    [Tooltip("Nivel que esta entrada representa.")]
    public int level = 1;

    [Tooltip("Experiencia total acumulada necesaria para alcanzar este nivel.")]
    public int experienceRequired = 0;

    [Tooltip("Puntos de Cordura (vida máxima) que se suman al alcanzar el nivel.")]
    public int corduraGain = 5;

    [Tooltip("Puntos de Inteligencia (ataque) que se suman al alcanzar el nivel.")]
    public int inteligenciaGain = 1;

    [Tooltip("Puntos de Memoria (defensa) que se suman al alcanzar el nivel.")]
    public int memoriaGain = 1;

    [Tooltip("Puntos de Rapidez (velocidad) que se suman al alcanzar el nivel.")]
    public int rapidezGain = 0;

    [Tooltip("Puntos de Fealdad (suerte) que se suman al alcanzar el nivel.")]
    public int fealdadGain = 0;

    [Tooltip("Habilidades desbloqueadas al alcanzar este nivel (puede estar vacío).")]
    public AbilityData[] abilitiesUnlocked = new AbilityData[0];
}
