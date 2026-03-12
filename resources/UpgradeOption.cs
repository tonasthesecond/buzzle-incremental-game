#nullable enable
using Godot;

[GlobalClass]
public abstract partial class UpgradeOption : Resource
{
    public int Level { get; set; } = 0; // How many times this upgrade has been bought
    public abstract string GetText(); // Text to display
    public abstract int GetCost(); // Cost to buy
    public abstract void Apply(); // Apply upgrade

    // Check if this upgrade can be bought, and if not, return a message
    public virtual bool FailCondition(out string? fail_message)
    {
        fail_message = null;
        return false;
    }

    public bool Buy(out string? fail_message)
    {
        // TODO: check if enough honey
        if (FailCondition(out fail_message))
            return false;

        Level++;
        // TODO: deduct cost
        Apply();
        fail_message = null;
        return true;
    }
}
