public class FleeAction : ICombatAction
{
    private readonly IFleeHandler fleeHandler;

    public string ActionName => "Huir";

    public FleeAction(IFleeHandler fleeHandler)
    {
        this.fleeHandler = fleeHandler;
    }

    public void Execute(ICombatant user, ICombatant target)
    {
        if (user == null || fleeHandler == null)
        {
            return;
        }

        fleeHandler.TryFlee(user);
    }
}
