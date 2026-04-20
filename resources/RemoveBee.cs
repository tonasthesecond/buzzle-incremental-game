using Godot;

[GlobalClass]
public partial class RemoveBee : SelectedResource
{
    [Export]
    public string BeeTypeName { get; set; } = "";

    private Bee? bee;

    public override string GetHoverTitle() =>
        $"Remove {(BeeTypeName == "Rocket" ? "Jetpack" : BeeTypeName)} bee";

    public override string GetHoverDescription()
    {
        string color_code = "noun_" + BeeTypeName.ToLower();
        if (color_code == "noun_")
            color_code = "noun_bee";

        if (BeeTypeName == "Rocket")
            BeeTypeName = "Jetpack";

        return $"Remove a {Style.CK(BeeTypeName + " ", color_code)}bee from a hive.";
    }

    public override string GetHoverSubtitle() => "";

    public override int GetHoverCost() => 0;

    public override bool IsEnough() => true;
}
