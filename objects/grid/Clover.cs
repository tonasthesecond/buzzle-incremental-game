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
        // if (isJackpot) { }
    }

    public override string GetHoverDescription()
    {
        string desc =
            $"{Style.CK("Flower", "noun_flower")} that has a  {Style.CKPercent(1f - GameStore.CloverJackpotChance.Value)} chance of producing {Style.CK(GameStore.CloverRegularHoneyGain.Value.ToString("F0"))} honey and a {Style.CKPercent(GameStore.CloverJackpotChance.Value)} chance of jackpot, producing {Style.CK(GameStore.CloverJackpotHoneyGain.Value.ToString("F0"))} honey.";
        if (isJackpot)
            desc += "\nThis flower is a jackpot!";
        return desc;
    }
}
