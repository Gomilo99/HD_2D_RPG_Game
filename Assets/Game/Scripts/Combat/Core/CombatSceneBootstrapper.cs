using System.Collections.Generic;
using UnityEngine;

public class CombatSceneBootstrapper : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private BattleUIController battleUI;
    [SerializeField] private ActionSelectorPanel actionSelectorPanel;

    [Header("Prefabs")]
    [SerializeField] private PlayerCharacter playerPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] playerSpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Fallback (si no hay contexto)")]
    [SerializeField] private List<CharacterStats> fallbackPlayerStats = new List<CharacterStats>();
    [SerializeField] private List<EnemyCharacter> fallbackEnemyPrefabs = new List<EnemyCharacter>();

    private bool isReady;

    private void Awake()
    {
        CacheReferences();
        isReady = ValidateReferences();
        if (!isReady)
        {
            enabled = false;
        }
    }

    private void Start()
    {
        if (!isReady)
        {
            return;
        }

        if (actionSelectorPanel != null && battleUI != null)
        {
            actionSelectorPanel.SetBattleUI(battleUI);
        }

        List<BaseCharacter> playerInstances = SpawnPlayers();
        List<BaseCharacter> enemyInstances = SpawnEnemies();


        combatManager.SetPlayerParty(playerInstances);
        combatManager.SetEnemyParty(enemyInstances);
        combatManager.StartCombat();

        CombatContext.Clear();
    }

    private void CacheReferences()
    {
        if (battleUI == null)
        {
            battleUI = FindFirstObjectByType<BattleUIController>();
        }

        if (actionSelectorPanel == null && battleUI != null)
        {
            actionSelectorPanel = FindFirstObjectByType<ActionSelectorPanel>();
        }
    }

    private bool ValidateReferences()
    {
        bool ok = true;

        if (combatManager == null)
        {
            Debug.LogWarning("CombatSceneBootstrapper: CombatManager no asignado.", this);
            ok = false;
        }
        if (actionSelectorPanel == null)
        {
            Debug.LogWarning("CombatSceneBootstrapper: Action Selector Panel no encontrado");
            ok = false;
        }
        if(battleUI == null)
        {
            Debug.LogWarning("CombatSceneBootstrapper: Battle UI no encontrado");
            ok = false;
        }

        if (playerPrefab == null)
        {
            Debug.LogWarning("CombatSceneBootstrapper: Player prefab no asignado.", this);
            ok = false;
        }

        if (playerSpawnPoints == null || playerSpawnPoints.Length == 0)
        {
            Debug.LogWarning("CombatSceneBootstrapper: playerSpawnPoints vacios.", this);
            ok = false;
        }

        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
        {
            Debug.LogWarning("CombatSceneBootstrapper: enemySpawnPoints vacios.", this);
        }

        return ok;
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
            if (PlayerData.Instance != null)
            {
                PlayerData.Instance.TryApplyStoredHealth(instance);
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
