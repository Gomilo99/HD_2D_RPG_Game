public interface ICombatAction
{
    string ActionName { get; }
    void Execute(ICombatant user, ICombatant target);
}
