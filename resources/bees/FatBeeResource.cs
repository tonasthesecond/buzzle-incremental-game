using Godot;

[GlobalClass]
public partial class FatBeeResource : BeeResource
{
    public override void ApplyStats(BeeEntity bee)
    {
        bee.Speed = new Stat(() =>
            (GameStore.BeeSpeed.Value * (1f - GameStore.FatBeeSpeedDebuff.Value))
        );
        bee.HoneyCapacity = new Stat(() =>
            GameStore.BeeCapacityHoney.Value + GameStore.FatBeeCapacityHoneyBonus.Value
        );
        bee.BopAmplitude = 4f;
    }
}
