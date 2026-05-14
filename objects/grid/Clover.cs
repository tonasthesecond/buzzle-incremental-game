using Godot;

[GlobalClass]
public partial class Clover : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.CloverHoneyCost.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.CloverPollinationTime.Value);
    public Stat JackpotChance { get; set; } = new(() => GameStore.CloverJackpotChance.Value);
    public Stat JackpotGain { get; set; } = new(() => GameStore.CloverJackpotHoneyGain.Value);

    public Clover()
    {
        HoneyGain = new(() =>
        {
            if (GD.Randf() < JackpotChance.Value)
            {
                isJackpot = true;
                return JackpotGain.Value;
            }
            isJackpot = false;
            return GameStore.CloverRegularHoneyGain.Value;
        });
    }

    private bool isJackpot { get; set; } = false;

    protected override void OnPollinated()
    {
        base.OnPollinated();
        if (isJackpot)
            sprite.Play("jackpot");
    }

    protected override string GetTechnicalText()
    {
        string desc = "";
        desc += $"{Style.CK("Jackpot Chance")}: {Style.CKPercent(JackpotChance.Value)}\n";
        desc +=
            $"{Style.CK("Jackpot Prod.")}: {Style.CK(JackpotGain.Value.ToString("F0"))} honey\n";
        desc += $"{Style.CK("Pol. Cost")}: {Style.CK(HoneyCost.Value.ToString("F0"))} honey\n";
        desc +=
            $"{Style.CK("Regular Prod.")}: {Style.CK(GameStore.CloverRegularHoneyGain.Value.ToString("F0"))} honey\n";
        desc +=
            $"{Style.CK("Pol. Time")}: {Style.CK(PollinationTime.Value.ToString("F1"))} seconds";
        if (Placed)
        {
            desc += GetTileStats();
            if (GameStore.GrassCloverJackpotChanceBonus.Value > 0f)
                desc +=
                    $"\n{Style.CK("Natural Habitat", "noun_grass")}: {Style.CKPercent(GameStore.GrassCloverJackpotChanceBonus.Value)}";
        }
        return desc;
    }
}
