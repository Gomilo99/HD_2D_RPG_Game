using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "RPG/Character Stats")]
public class CharacterStats : ScriptableObject
{
    public string characterName;
    public int maxCordura = 100;
    public int inteligencia = 10;
    public int memoria = 5;
    public int rapidez = 5;
    public int fealdad = 1;

    public List<AbilityData> startingAbilities = new List<AbilityData>();
}
