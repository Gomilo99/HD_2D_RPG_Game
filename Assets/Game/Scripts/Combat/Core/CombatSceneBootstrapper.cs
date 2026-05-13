using System.Collections.Generic;
using UnityEngine;

public class CombatSceneBootstrapper : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private BattleUIController battleUI;
    [SerializeField] private ActionSelectorPanel actionSelectorPanel;
    [SerializeField] private bool debugLogs = false;

    [Header("Prefabs")]
    [SerializeField] private PlayerCharacter playerPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] playerSpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Fallback (si no hay contexto)")]
    [SerializeField] private List<CharacterStats> fallbackPlayerStats = new List<CharacterStats>();
    [SerializeField] private List<EnemyCharacter> fallbackEnemyPrefabs = new List<EnemyCharacter>();

    private void Start()
    {
        if (combatManager == null)
        {
            Debug.LogWarning("CombatSceneBootstrapper: CombatManager no asignado.", this);
            return;
        }

        if (battleUI == null)
        {
            battleUI = FindFirstObjectByType<BattleUIController>();
        }

        if (actionSelectorPanel == null && battleUI != null)
        {
            actionSelectorPanel = battleUI.GetComponentInChildren<ActionSelectorPanel>(true);
        }

        if (actionSelectorPanel != null && battleUI != null)
        {
            actionSelectorPanel.SetBattleUI(battleUI);
        }

        if (battleUI == null)
        {
            Debug.LogWarning("CombatSceneBootstrapper: BattleUIController no encontrado. Los jugadores actuaran en automatico.", this);
        }
        else if (debugLogs)
        {
            Debug.Log("CombatSceneBootstrapper: BattleUIController encontrado.", this);
        }

        List<BaseCharacter> playerInstances = SpawnPlayers();
        List<BaseCharacter> enemyInstances = SpawnEnemies();

        if (debugLogs)
        {
            Debug.Log($"CombatSceneBootstrapper: Players={playerInstances.Count}, Enemies={enemyInstances.Count}.", this);
        }

        combatManager.SetPlayerParty(playerInstances);
        combatManager.SetEnemyParty(enemyInstances);
        combatManager.StartCombat();

        CombatContext.Clear();
    }

    private List<BaseCharacter> SpawnPlayers()
    {
        List<BaseCharacter> spawned = new List<BaseCharacter>();
        IReadOnlyList<CharacterStats> partyStats = CombatContext.HasContext && CombatContext.PlayerPartyStats.Count > 0
            ? CombatContext.PlayerPartyStats
            : fallbackPlayerStats;

        if (playerPrefab == null || playerSpawnPoints == null)
        {
            return spawned;
        }

        int count = Mathf.Min(partyStats.Count, playerSpawnPoints.Length);
        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = playerSpawnPoints[i];
            if (spawnPoint == null)
            {
                continue;
            }

            PlayerCharacter instance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            instance.Initialize(partyStats[i]);
            IActionSelector selector = actionSelectorPanel != null && battleUI != null ? actionSelectorPanel : battleUI;
            if (selector != null)
            {
                instance.SetActionSelector(selector);
            }
            WireTargetSelectable(instance);
            spawned.Add(instance);
        }

        return spawned;
    }

    private List<BaseCharacter> SpawnEnemies()
    {
        List<BaseCharacter> spawned = new List<BaseCharacter>();
        IReadOnlyList<EnemyCharacter> enemies = CombatContext.HasContext && CombatContext.EnemyPrefabs.Count > 0
            ? CombatContext.EnemyPrefabs
            : fallbackEnemyPrefabs;

        if (enemySpawnPoints == null)
        {
            return spawned;
        }

        int count = Mathf.Min(enemies.Count, enemySpawnPoints.Length);
        for (int i = 0; i < count; i++)
        {
            EnemyCharacter enemyPrefab = enemies[i];
            Transform spawnPoint = enemySpawnPoints[i];
            if (enemyPrefab == null || spawnPoint == null)
            {
                continue;
            }
            EnemyCharacter instance = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            WireTargetSelectable(instance);
            spawned.Add(instance);
        }

        return spawned;
    }

    private void WireTargetSelectable(BaseCharacter character)
    {
        if (battleUI == null || character == null)
        {
            return;
        }

        CombatTargetSelectable selectable = character.GetComponentInChildren<CombatTargetSelectable>();
        if (selectable != null)
        {
            selectable.SetBattleUI(battleUI);
        }
    }
}
