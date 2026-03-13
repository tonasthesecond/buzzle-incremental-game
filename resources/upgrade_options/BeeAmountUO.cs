#nullable enable
using Godot;

[GlobalClass]
public partial class BeeAmountUO : UpgradeOption
{
    public override string GetText()
    {
        int amount = GameStore.BeeCount;
        if (amount == 1)
            return "1 bee ➞ 2 bees";
        return $"{amount} bees ➜ {amount + 1} bees";
    }

    public override int GetCost() => 10 + (int)Mathf.Pow(Level, 2);

    public override void Apply() => Services.Get<BeeSystem>().SpawnBeeAnywhere();

    public override bool FailCondition(out string? fail_message)
    {
        // TODO: check if there's enough space
        fail_message = null;
        return false;
    }
}
