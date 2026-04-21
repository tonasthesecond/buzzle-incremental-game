using Godot;

[GlobalClass]
public partial class Clover : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.CloverHoneyCost.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.CloverPollinationTime.Value);

    public Clover()
    {
        HoneyGain = new(() =>
        {
            if (GD.Randf() < GameStore.CloverJackpotChance.Value)
            {
                isJackpot = true;
                return GameStore.CloverJackpotHoneyGain.Value;
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
        desc +=
            $"{Style.CK("Regular Prod.")}: {Style.CK(GameStore.CloverRegularHoneyGain.Value.ToString("F0"))} honey\n";
        desc +=
            $"{Style.CK("Jackpot Chance")}: {Style.CKPercent(GameStore.CloverJackpotChance.Value)}\n";
        desc +=
            $"{Style.CK("Jackpot Prod.")}: {Style.CK(GameStore.CloverJackpotHoneyGain.Value.ToString("F0"))} honey\n";
        desc += $"{Style.CK("Pol. Cost")}: {Style.CK(HoneyCost.Value.ToString("F0"))} honey\n";
        desc +=
            $"{Style.CK("Pol. Time")}: {Style.CK(PollinationTime.Value.ToString("F1"))} seconds";
        return desc;
    }
}
