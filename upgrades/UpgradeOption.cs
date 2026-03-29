using Godot;

public abstract partial class UpgradeOption : Resource
{
    [Signal]
    public delegate void AppliedEventHandler();

    [Export]
    public string Name { get; set; } = "Upgrade";

    [Export]
    public int MaxLevel { get; set; } = -1; // -1 = unlimited

    public int Level { get; set; } = 0; // How many times this upgrade has been bought

    public abstract string GetText(); // Text to display
    public abstract int GetCost(); // Cost to buy
    public abstract void Apply(); // Apply upgrade

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
        EmitSignal(SignalName.Applied);
        Apply();

        failMessage = null;
        return true;
    }
}
