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
        {
            GD.Print($"Jackpot!");
        }
    }
}
