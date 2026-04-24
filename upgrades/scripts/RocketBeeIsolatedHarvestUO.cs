using Godot;

[GlobalClass]
public partial class RocketBeeIsolatedHarvestUO : IUpgradeOption
{
    public override string GetTechnicalText() =>
        $"{Style.CK("Jetpack bees", "noun_rocket")} prioritize harvesting isolated {Style.CK("flowers", "noun_flower")}";

    public override void Apply()
    {
        if (Level <= 0)
            return;
        GameStore.RocketBeeIsolatedHarvest = true;
    }
}
