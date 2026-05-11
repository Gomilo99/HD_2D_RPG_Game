using UnityEngine;

public class EnemyEncounter : MonoBehaviour
{
    [SerializeField] private EnemyCharacter enemyPrefab;

    public EnemyCharacter EnemyPrefab => enemyPrefab;
}
