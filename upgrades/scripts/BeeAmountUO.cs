using Godot;

[GlobalClass]
public partial class BeeAmountUO : IUpgradeOption
{
    public override string GetText()
    {
        int amount = Services.Get<BeeSystem>().GetBeeCount();
        return $"{amount} bees ➜ {amount + 1} bees";
    }

    public override int GetCost() => 10 + (int)Mathf.Pow(Level, 2);

    public override void Apply() => Services.Get<BeeSystem>().SpawnBeeAnywhere();

    public override bool FailCondition(out FailMessage? fail_message)
    {
        // TODO: check if there's enough space
        fail_message = null;
        return false;
    }
}
