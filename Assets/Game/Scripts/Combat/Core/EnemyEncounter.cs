using System.Collections.Generic;
using UnityEngine;

public class EnemyEncounter : MonoBehaviour
{
    [SerializeField] private EnemyCharacter enemyPrefab;
    [SerializeField] private List<EnemyCharacter> enemyParty = new List<EnemyCharacter>();

    public EnemyCharacter EnemyPrefab => enemyPrefab;
    public IReadOnlyList<EnemyCharacter> EnemyParty => enemyParty;
}
