using System.Linq;
using System.Threading.Tasks;
using Godot;

public interface IBeeJob
{
    void Start(Bee bee);
}

public abstract class BeeJob : IBeeJob
{
    public async void Start(Bee bee)
    {
        await bee.ToSignal(bee.GetTree(), SceneTree.SignalName.ProcessFrame);
        await Run(bee);
    }

    protected abstract Task Run(Bee bee);
}

public class IdleJob : IBeeJob
{
    public void Start(Bee bee) { }
}

public class HarvesterJob : BeeJob
{
    protected override async Task Run(Bee bee)
    {
        var grid = Services.Get<Grid>();
        var beeSystem = Services.Get<BeeSystem>();

        bee.Home.AddBee(bee);

        while (true)
        {
            // Wait for an unclaimed flower that actually has honey
            FlowerTile flower = null;
            while (flower == null)
            {
                flower = grid.GetTilesOfType<FlowerTile>()
                    .Where(f => !beeSystem.IsClaimed(f) && f.Honey > 0)
                    .OrderBy(_ => GD.Randf())
                    .FirstOrDefault();
                if (flower == null)
                    await bee.ToSignal(bee.GetTree(), SceneTree.SignalName.ProcessFrame);
            }

            // Travel to flower and harvest
            beeSystem.ClaimTile(flower);
            bee.Show();
            await bee.TravelTo(flower.GlobalPosition);

            beeSystem.ReleaseTile(flower);
            int honeyHarvestAmount = Mathf.Min(GameStore.BeeCapacity, flower.Honey);
            bee.carryingHoney += honeyHarvestAmount;
            flower.Honey -= honeyHarvestAmount;

            // Return to home hive and deposit
            await bee.TravelTo(bee.Home.GlobalPosition);

            bee.Home.DepositMax(bee.carryingHoney); // TODO: process leftover
            bee.carryingHoney = 0;
            bee.Hide();
        }
    }
}

public class PollinatorJob : BeeJob
{
    protected override async Task Run(Bee bee)
    {
        var grid = Services.Get<Grid>();
        var beeSystem = Services.Get<BeeSystem>();

        bee.Home.AddBee(bee);

        while (true)
        {
            // Wait until hive has honey to spend
            while (bee.Home.Honey <= 0)
                await bee.ToSignal(bee.GetTree(), SceneTree.SignalName.ProcessFrame);

            // Wait for an unclaimed unpollinated flower
            FlowerTile flower = null;
            while (flower == null)
            {
                flower = grid.GetTilesOfType<FlowerTile>()
                    .Where(f => !beeSystem.IsClaimed(f) && f.Honey <= 0)
                    .OrderBy(_ => GD.Randf())
                    .FirstOrDefault();
                if (flower == null)
                    await bee.ToSignal(bee.GetTree(), SceneTree.SignalName.ProcessFrame);
            }

            // Load up from hive
            beeSystem.ClaimTile(flower);
            bee.carryingHoney = bee.Home.TakePossible(GameStore.BeeCapacity);

            // Travel and dump honey into flower
            bee.Show();
            await bee.TravelTo(flower.GlobalPosition);

            beeSystem.ReleaseTile(flower);
            flower.Pollinate(bee.carryingHoney);
            bee.carryingHoney = 0;

            // Return home
            await bee.TravelTo(bee.Home.GlobalPosition);
            bee.Hide();
        }
    }
}
