using System.Linq;
using Godot;

[GlobalClass]
public partial class Blackhole : Flower
{
    public override Stat HoneyCost { set; get; } = new(() => GameStore.BlackholeHoneyCost.Value);
    public override Stat HoneyGain { set; get; } = new(() => GameStore.BlackholeHoneyGain.Value);
    public override Stat PollinationTime { set; get; } =
        new(() => GameStore.BlackholePollinationTime.Value);

    public Stat PullRange { set; get; } = new(() => GameStore.BlackholePullRange.Value);

    public override void _Ready()
    {
        base._Ready();
        if (Placed)
            AddToGroup("blackhole");
    }

    protected override string GetTechnicalText()
    {
        string desc = "";
        desc += $"{Style.CK("Pol. Cost")}: {Style.CK(HoneyCost.Value.ToString("F0"))} honey\n";
        desc +=
            $"{Style.CK("Pol. Time")}: {Style.CK(PollinationTime.Value.ToString("F1"))} seconds";
        desc +=
            $"\n{Style.CK("Pull Range")}: {Style.CK(Utils.PixelsToTiles(PullRange.Value).ToString("F0"))} tiles";
        desc +=
            $"\n{Style.CK("Speed Toward")}: {Style.CK(Utils.PixelsToTiles(GameStore.BlackholePositivePullSpeed.Value).ToString("F1"))} tiles per sec";
        desc +=
            $"\n{Style.CK("Speed Away")}: -{Style.CK(Utils.PixelsToTiles(GameStore.BlackholeNegativePullSpeed.Value).ToString("F1"))} tiles per sec";
        if (Placed)
        {
            desc += GetTileStats();
        }
        return desc;
    }

    public static float GetSpeedBonus(Vector2 curPos, Vector2 targetPos)
    {
        Vector2 moveDir = (targetPos - curPos).Normalized();

        float bonus = 0f;
        foreach (
            Blackhole bh in Services
                .Get<Grid>()
                .GetObjectsOfType<Blackhole>()
                .Where(b =>
                    b.CurState == Flower.State.Pollinated
                    && b.GlobalPosition.DistanceTo(curPos) <= b.PullRange.Value
                )
        )
        {
            float alignment = moveDir.Dot((bh.GlobalPosition - curPos).Normalized()); // -1..1
            float pull =
                alignment >= 0
                    ? alignment * GameStore.BlackholePositivePullSpeed.Value
                    : alignment * GameStore.BlackholeNegativePullSpeed.Value;
            bonus += pull;
        }
        return bonus;
    }
}
