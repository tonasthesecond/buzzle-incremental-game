using Godot;

[GlobalClass]
public partial class QueenBeeLeashRoseUO : IUpgradeOption
{
    public override string GetTechnicalText() =>
        $"{Style.CK("Queen bees", "noun_queen")} are drawn toward {Style.CK("Roses", "noun_rose")}";

    public override void Apply()
    {
        if (Level <= 0)
            return;
        GameStore.QueenBeeLeashRose = true;
    }
}
