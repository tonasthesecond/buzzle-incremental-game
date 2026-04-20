using Godot;

[GlobalClass]
public partial class QueenBeeLeashRoseUO : IUpgradeOption
{
    public override string GetTechnicalText() => "";

    public override void Apply() => GameStore.QueenBeeLeashRose = true;
}
