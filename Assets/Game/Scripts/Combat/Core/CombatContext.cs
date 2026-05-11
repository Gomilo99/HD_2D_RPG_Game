using System.Collections.Generic;
using System.Linq;

public static class CombatContext
{
    private static readonly List<CharacterStats> playerPartyStats = new List<CharacterStats>();
    private static readonly List<EnemyCharacter> enemyPrefabs = new List<EnemyCharacter>();

    public static bool HasContext => playerPartyStats.Count > 0 || enemyPrefabs.Count > 0;

    public static IReadOnlyList<CharacterStats> PlayerPartyStats => playerPartyStats;
    public static IReadOnlyList<EnemyCharacter> EnemyPrefabs => enemyPrefabs;

    public static void SetContext(IEnumerable<CharacterStats> players, IEnumerable<EnemyCharacter> enemies)
    {
        playerPartyStats.Clear();
        enemyPrefabs.Clear();

        if (players != null)
        {
            playerPartyStats.AddRange(players.Where(stats => stats != null));
        }

        if (enemies != null)
        {
            enemyPrefabs.AddRange(enemies.Where(prefab => prefab != null));
        }
    }

    public static void Clear()
    {
        playerPartyStats.Clear();
        enemyPrefabs.Clear();
    }
}
