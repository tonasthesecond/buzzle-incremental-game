using Godot;

[GlobalClass]
public partial class FatBee : Bee
{
    public override void ApplyStats(Bee bee)
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
