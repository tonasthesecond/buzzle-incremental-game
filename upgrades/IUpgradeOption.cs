using Godot;

[GlobalClass]
public abstract partial class IUpgradeOption : Resource, IHasHoverDescription
{
    [Signal]
    public delegate void AppliedEventHandler();

    [Export]
    public string Name { get; set; } = "Upgrade";

    [Export]
    public virtual int MaxLevel { get; set; } = -1; // -1 = infinite

    [Export]
    public IScaleModel CostScaler { get; set; }

    public int Level { get; set; } = 0; // How many times this upgrade has been bought

    public abstract string GetHoverDescription(); // description of upgrade
    public abstract void Apply(); // apply upgrade

    public virtual int GetCost() => (int)CostScaler.Get(Level);

    public bool IsEnough() => GetCost() <= GameStore.Honey;

    // Check if this upgrade can be bought, and if not, return a message
    public virtual bool FailCondition(out FailMessage? fail_message)
    {
        fail_message = null;
        return false;
    }

    public bool Buy(out FailMessage? failMessage)
    {
        // check if enough honey and max level
        if (MaxLevel != -1 && Level >= MaxLevel)
        {
            failMessage = new FailMessage("Already at max level!");
            return false;
        }
        int cost = GetCost();
        if (GameStore.Honey < cost)
        {
            failMessage = new FailMessage("Not enough honey!");
            return false;
        }

        // check any other conditions
        if (FailCondition(out failMessage))
            return false;

        // buy upgrade
        GameStore.Honey -= cost;
        Level++;
        Apply();
        EmitSignal(SignalName.Applied);

        failMessage = null;
        return true;
    }
}
