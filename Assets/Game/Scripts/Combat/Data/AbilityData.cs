using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "RPG/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    public AbilityEffectType effectType = AbilityEffectType.Damage;
    public AbilityTargetType targetType = AbilityTargetType.SingleEnemy;
    public int power = 5;
    public int durationTurns = 1;
}
