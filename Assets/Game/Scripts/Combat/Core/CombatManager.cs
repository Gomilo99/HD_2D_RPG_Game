using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour, IFleeHandler
{
    public static CombatManager Instance { get; private set; }
    [SerializeField] private List<BaseCharacter> playerParty = new List<BaseCharacter>();
    [SerializeField] private List<BaseCharacter> enemyParty = new List<BaseCharacter>();
    [SerializeField] private bool autoStart = true;
    [SerializeField, Range(0f, 1f)] private float baseFleeChance = 0.4f;

    private readonly List<ICombatant> players = new List<ICombatant>();
    private readonly List<ICombatant> enemies = new List<ICombatant>();

    private ITurnQueue turnQueue;
    private IActionResolver actionResolver;
    private IVictoryCondition victoryCondition;

    private ICombatant currentCombatant;
    private bool awaitingPlayerAction;
    private bool combatActive;

    public event Action<ICombatant> TurnStarted;
    public event Action<string> CombatLog;
    public event Action<CombatResult> CombatEnded;

    private void Awake()
    {
        Instance = this;
        turnQueue = new TurnQueue();
        actionResolver = new ActionResolver();
        victoryCondition = new BasicVictoryCondition();
    }

    private void Start()
    {
        if (autoStart)
        {
            StartCombat();
        }
    }

    public void StartCombat()
    {
        BuildCombatantLists();
        combatActive = true;
        awaitingPlayerAction = false;
        turnQueue.Initialize(GetAllCombatants());
        Log("Combate iniciado.");
        StartNextTurn();
    }

    public void SetPlayerParty(IEnumerable<BaseCharacter> party)
    {
        playerParty.Clear();
        if (party == null)
        {
            return;
        }

        foreach (BaseCharacter member in party)
        {
            if (member != null)
            {
                playerParty.Add(member);
            }
        }
    }

    public void SetEnemyParty(IEnumerable<BaseCharacter> party)
    {
        enemyParty.Clear();
        if (party == null)
        {
            return;
        }

        foreach (BaseCharacter member in party)
        {
            if (member != null)
            {
                enemyParty.Add(member);
            }
        }
    }

    // Método para agregar enemigos en el Combat Manager (Usar para generación dinámica)
    public void SetEnemyToList(BaseCharacter character)
    {
        enemyParty.Add(character);
    }
    public void ExecuteAction(ICombatant user, ICombatAction action, IReadOnlyList<ICombatant> targets)
    {
        if (!combatActive || user == null || action == null)
        {
            return;
        }

        if (currentCombatant != user)
        {
            return;
        }

        awaitingPlayerAction = false;
        actionResolver.Resolve(user, action, targets);
        LogAction(user, action, targets);
        EndTurn();
    }

    public void LogEvent(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        Log(message);
    }

    public void SubmitPlayerAction(ICombatAction action, IReadOnlyList<ICombatant> targets)
    {
        if (!combatActive || !awaitingPlayerAction)
        {
            return;
        }

        if (currentCombatant is not PlayerCharacter)
        {
            return;
        }

        ExecuteAction(currentCombatant, action, targets);
    }

    public void EndTurn()
    {
        if (!combatActive)
        {
            return;
        }

        StartNextTurn();
    }

    public IReadOnlyList<ICombatant> GetAlivePlayers()
    {
        return GetAliveFrom(players);
    }

    public IReadOnlyList<ICombatant> GetAliveEnemies()
    {
        return GetAliveFrom(enemies);
    }

    public ICombatant GetRandomPlayer()
    {
        IReadOnlyList<ICombatant> alive = GetAlivePlayers();
        return PickRandom(alive);
    }

    public ICombatant GetRandomEnemy()
    {
        IReadOnlyList<ICombatant> alive = GetAliveEnemies();
        return PickRandom(alive);
    }

    public IReadOnlyList<ICombatant> GetTargetsFor(AbilityTargetType targetType, ICombatant user)
    {
        bool isPlayer = players.Contains(user);
        IReadOnlyList<ICombatant> allies = isPlayer ? GetAlivePlayers() : GetAliveEnemies();
        IReadOnlyList<ICombatant> foes = isPlayer ? GetAliveEnemies() : GetAlivePlayers();

        switch (targetType)
        {
            case AbilityTargetType.SingleEnemy:
                return WrapSingle(PickRandom(foes));
            case AbilityTargetType.AllEnemies:
                return foes;
            case AbilityTargetType.SingleAlly:
                return WrapSingle(PickRandom(allies));
            case AbilityTargetType.AllAllies:
                return allies;
            case AbilityTargetType.Self:
                return WrapSingle(user);
            default:
                return WrapSingle(PickRandom(foes));
        }
    }

    public IReadOnlyList<ICombatant> GetValidTargets(AbilityTargetType targetType, ICombatant user)
    {
        bool isPlayer = players.Contains(user);
        IReadOnlyList<ICombatant> allies = isPlayer ? GetAlivePlayers() : GetAliveEnemies();
        IReadOnlyList<ICombatant> foes = isPlayer ? GetAliveEnemies() : GetAlivePlayers();

        switch (targetType)
        {
            case AbilityTargetType.SingleEnemy:
                return foes;
            case AbilityTargetType.AllEnemies:
                return foes;
            case AbilityTargetType.SingleAlly:
                return allies;
            case AbilityTargetType.AllAllies:
                return allies;
            case AbilityTargetType.Self:
                return WrapSingle(user);
            default:
                return foes;
        }
    }

    public bool TryFlee(ICombatant combatant)
    {
        if (!combatActive || combatant == null)
        {
            return false;
        }

        float chance = Mathf.Clamp01(combatant.Luck * 0.01f + baseFleeChance);
        bool success = UnityEngine.Random.value <= chance;

        if (success)
        {
            Log($"{combatant.Name} huyó del combate.");
            EndCombat(CombatResult.Fled);
        }
        else
        {
            Log($"{combatant.Name} falló al huir.");
        }

        return success;
    }

    private void StartNextTurn()
    {
        turnQueue.RemoveDead();

        CombatResult result = victoryCondition.Evaluate(players, enemies);
        if (result != CombatResult.Ongoing)
        {
            EndCombat(result);
            return;
        }

        currentCombatant = turnQueue.GetNext();
        if (currentCombatant == null)
        {
            EndCombat(CombatResult.Defeat);
            return;
        }

        currentCombatant.TickStatusEffects();

        // Si el combatiente está bloqueado (ej: parálisis), se omite su turno.
        if (currentCombatant.IsActionBlocked)
        {
            Log($"{currentCombatant.Name} está paralizado y pierde su turno.");
            TurnStarted?.Invoke(currentCombatant);
            EndTurn();
            return;
        }

        awaitingPlayerAction = currentCombatant is PlayerCharacter;
        TurnStarted?.Invoke(currentCombatant);
        currentCombatant.ChooseAction(this);
    }

    private void EndCombat(CombatResult result)
    {
        if (!combatActive)
        {
            return;
        }

        combatActive = false;
        CombatEnded?.Invoke(result);
        Log($"Combate finalizado: {result}.");
    }

    private void BuildCombatantLists()
    {
        players.Clear();
        enemies.Clear();

        foreach (BaseCharacter player in playerParty)
        {
            if (player != null)
            {
                players.Add(player);
            }
        }

        foreach (BaseCharacter enemy in enemyParty)
        {
            if (enemy != null)
            {
                enemies.Add(enemy);
            }
        }
    }

    private List<ICombatant> GetAllCombatants()
    {
        List<ICombatant> all = new List<ICombatant>(players.Count + enemies.Count);
        all.AddRange(players);
        all.AddRange(enemies);
        return all;
    }

    private IReadOnlyList<ICombatant> GetAliveFrom(List<ICombatant> source)
    {
        List<ICombatant> alive = new List<ICombatant>();
        foreach (ICombatant combatant in source)
        {
            if (combatant != null && combatant.IsAlive)
            {
                alive.Add(combatant);
            }
        }

        return alive;
    }

    private ICombatant PickRandom(IReadOnlyList<ICombatant> combatants)
    {
        if (combatants == null || combatants.Count == 0)
        {
            return null;
        }

        int index = UnityEngine.Random.Range(0, combatants.Count);
        return combatants[index];
    }

    private IReadOnlyList<ICombatant> WrapSingle(ICombatant combatant)
    {
        if (combatant == null)
        {
            return new List<ICombatant>();
        }

        return new List<ICombatant> { combatant };
    }

    private void Log(string message)
    {
        Debug.Log(message, this);
        CombatLog?.Invoke(message);
    }

    private void LogAction(ICombatant user, ICombatAction action, IReadOnlyList<ICombatant> targets)
    {
        if (user == null || action == null)
        {
            return;
        }

        if (targets != null && targets.Count > 0)
        {
            string targetNames = string.Join(", ", GetTargetNames(targets));
            Log($"{user.Name} usa {action.ActionName} en {targetNames}.");
            return;
        }

        Log($"{user.Name} usa {action.ActionName}.");
    }

    private IEnumerable<string> GetTargetNames(IReadOnlyList<ICombatant> targets)
    {
        foreach (ICombatant target in targets)
        {
            if (target != null)
            {
                yield return target.Name;
            }
        }
    }
}
