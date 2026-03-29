using System.Linq;
using Godot;

public interface IBeeJob
{
    void Tick(Bee bee);
}

public class IdleJob : IBeeJob
{
    public void Tick(Bee bee) => bee.FadeTo(0f);
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
        switch (state)
        {
            // find an unclaimed flower with honey
            case State.SeekingFlower:
                var eligible = bee
                    .Grid.GetObjectsOfType<BaseFlower>()
                    .Where(f => f.Honey > 0)
                    .ToArray();

                if (eligible.Length == 0)
                {
                    bee.SetJob(new IdleJob());
                    return;
                }

                flower = eligible
                    .Where(f => !bee.BeeSystem.IsClaimed(f))
                    .OrderBy(_ => GD.Randf())
                    .FirstOrDefault();

                if (flower == null)
                    return; // all claimed, retry next frame

                bee.BeeSystem.ClaimObject(flower);
                bee.FadeTo(1f);
                bee.MoveTo(flower.GlobalPosition);
                state = State.TravelingToFlower;
                break;

            // arrived at flower, take honey
            case State.TravelingToFlower:
                if (bee.IsMoving)
                    return;

                bee.BeeSystem.ReleaseObject(flower!);
                if (flower!.Honey <= 0)
                {
                    state = State.SeekingFlower;
                    return;
                } // picked clean mid-flight

                int amount = Mathf.Min((int)GameStore.BeeCapacityHoney.Value, flower.Honey);
                bee.carryingHoney += amount;
                flower.Honey -= amount;
                bee.MoveTo(bee.Home.GlobalPosition);
                state = State.TravelingHome;
                break;

            // arrived home, deposit
            case State.TravelingHome:
                if (bee.IsMoving)
                    return;

                bee.Home.Deposit(bee.carryingHoney);
                bee.carryingHoney = 0;
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
        Pollinating,
        TravelingHome,
    }

    private State state = State.SeekingFlower;
    private BaseFlower? flower;

    public void Tick(Bee bee)
    {
        switch (state)
        {
            // find an unclaimed empty flower
            case State.SeekingFlower:
                if (GameStore.Honey <= 0)
                    return;

                var eligible = bee
                    .Grid.GetObjectsOfType<BaseFlower>()
                    .Where(f => f.Honey <= 0)
                    .ToArray();

                if (eligible.Length == 0)
                {
                    bee.SetJob(new IdleJob());
                    return;
                }

                flower = eligible
                    .Where(f => !bee.BeeSystem.IsClaimed(f))
                    .OrderBy(_ => GD.Randf())
                    .FirstOrDefault();

                if (flower == null)
                    return;

                bee.carryingHoney = bee.Home.TakePossible((int)GameStore.BeeCapacityHoney.Value);
                if (bee.carryingHoney == 0)
                    return;

                bee.BeeSystem.ClaimObject(flower);
                bee.FadeTo(1f);
                bee.MoveTo(flower.GlobalPosition);
                state = State.TravelingToFlower;
                break;

            // arrived at flower, start pollination animation
            case State.TravelingToFlower:
                if (bee.IsMoving)
                    return;

                bee.StartPollinatingAnim(flower!.GlobalPosition);
                state = State.Pollinating;
                break;

            // animation done, deposit honey into flower
            case State.Pollinating:
                if (bee.IsAnimating)
                    return;

                flower!.Pollinate(bee.carryingHoney);
                bee.BeeSystem.ReleaseObject(flower!);
                bee.carryingHoney = 0;
                bee.MoveTo(bee.Home.GlobalPosition);
                state = State.TravelingHome;
                break;

            // arrived home
            case State.TravelingHome:
                if (bee.IsMoving)
                    return;

                bee.SetJob(new IdleJob());
                break;
        }
    }
}
