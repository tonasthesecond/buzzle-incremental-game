using System.Linq;
using Godot;

public interface IBeeJob
{
    void Tick(Bee bee);
}

public class IdleJob : IBeeJob
{
    public void Tick(Bee bee) { }
}

public class HarvesterJob : IBeeJob
{
    private enum State
    {
        SeekingFlower,
        TravelingToFlower,
        TravelingHome,
    }

    private State state = State.SeekingFlower;
    private BaseFlower? flower;

    public void Tick(Bee bee)
    {
        var grid = Services.Get<Grid>();
        var beeSystem = Services.Get<BeeSystem>();

        switch (state)
        {
            case State.SeekingFlower:
                var eligible = grid.GetObjectsOfType<BaseFlower>()
                    .Where(f => f.Honey > 0)
                    .ToArray();

                if (eligible.Length == 0)
                {
                    bee.SetJob(new IdleJob());
                    return;
                }

                flower = eligible
                    .Where(f => !beeSystem.IsClaimed(f))
                    .OrderBy(_ => GD.Randf())
                    .FirstOrDefault();

                if (flower == null)
                    return; // all claimed, retry next frame

                beeSystem.ClaimObject(flower);
                bee.Show();
                bee.MoveTo(flower.GlobalPosition);
                state = State.TravelingToFlower;
                break;

            case State.TravelingToFlower:
                if (bee.IsMoving)
                    return;

                beeSystem.ReleaseObject(flower!);
                if (flower!.Honey <= 0)
                {
                    state = State.SeekingFlower;
                    return;
                } // picked clean mid-flight

                int amount = Mathf.Min(GameStore.BeeCapacityHoney, flower.Honey);
                bee.carryingHoney += amount;
                flower.Honey -= amount;
                bee.MoveTo(bee.Home.GlobalPosition);
                state = State.TravelingHome;
                break;

            case State.TravelingHome:
                if (bee.IsMoving)
                    return;

                bee.Home.Deposit(bee.carryingHoney);
                bee.carryingHoney = 0;
                bee.Hide();
                bee.SetJob(new IdleJob());
                break;
        }
    }
}

public class PollinatorJob : IBeeJob
{
    private enum State
    {
        SeekingFlower,
        TravelingToFlower,
        TravelingHome,
    }

    private State state = State.SeekingFlower;
    private BaseFlower? flower;

    public void Tick(Bee bee)
    {
        var grid = Services.Get<Grid>();
        var beeSystem = Services.Get<BeeSystem>();

        switch (state)
        {
            case State.SeekingFlower:
                if (GameStore.Honey <= 0)
                    return;
                var eligible = grid.GetObjectsOfType<BaseFlower>()
                    .Where(f => f.Honey <= 0)
                    .ToArray();

                if (eligible.Length == 0)
                {
                    bee.SetJob(new IdleJob());
                    return;
                }

                flower = eligible
                    .Where(f => !beeSystem.IsClaimed(f))
                    .OrderBy(_ => GD.Randf())
                    .FirstOrDefault();

                if (flower == null)
                    return;

                bee.carryingHoney = bee.Home.TakePossible(GameStore.BeeCapacityHoney);
                if (bee.carryingHoney == 0)
                    return;

                beeSystem.ClaimObject(flower);
                bee.Show();
                bee.MoveTo(flower.GlobalPosition);
                state = State.TravelingToFlower;
                break;

            case State.TravelingToFlower:
                if (bee.IsMoving)
                    return;

                beeSystem.ReleaseObject(flower!);
                flower!.Pollinate(bee.carryingHoney);
                bee.carryingHoney = 0;
                bee.MoveTo(bee.Home.GlobalPosition);
                state = State.TravelingHome;
                break;

            case State.TravelingHome:
                if (bee.IsMoving)
                    return;

                bee.Hide();
                bee.SetJob(new IdleJob());
                break;
        }
    }
}
