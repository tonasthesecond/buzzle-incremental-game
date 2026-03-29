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
                }

                int amount = Mathf.Min((int)bee.HoneyCapacity.Value, flower.Honey);
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

                bee.carryingHoney = bee.Home.TakePossible((int)bee.HoneyCapacity.Value);
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

public class QueenJob : IBeeJob
{
    private enum State
    {
        Moving,
        Waiting,
    }

    private static readonly ulong MinWaitMs = 1500;
    private static readonly ulong MaxWaitMs = 4000;
    private static readonly ulong MinMoveMs = 1000;
    private static readonly ulong MaxMoveMs = 3000;
    private static readonly float LeashTiles = 2.5f;
    private static readonly float LeashPull = 1.8f; // radians/sec toward hive
    private static readonly float TurnBiasMax = 2.2f; // radians/sec max curve
    private static readonly float TurnBiasShift = 1f; // how much bias shifts on each new bout
    private static readonly float Lookahead = 24f; // pixels ahead to target

    private State state = State.Waiting;
    private ulong stateUntil = 0;
    private float angle;
    private float turnBias = 0f;
    private bool initialized = false;

    public void Tick(Bee bee)
    {
        if (!initialized)
        {
            angle = GD.Randf() * Mathf.Tau;
            turnBias = (float)GD.RandRange(-TurnBiasMax, TurnBiasMax);
            stateUntil = Time.GetTicksMsec() + (ulong)GD.RandRange(MinWaitMs, MaxWaitMs);
            initialized = true;
        }

        switch (state)
        {
            case State.Waiting:
                bee.MoveTo(bee.GlobalPosition); // stay put
                if (Time.GetTicksMsec() < stateUntil)
                    return;
                // new movement bout — shift the bias
                turnBias += (float)GD.RandRange(-TurnBiasShift, TurnBiasShift);
                turnBias = Mathf.Clamp(turnBias, -TurnBiasMax, TurnBiasMax);
                stateUntil = Time.GetTicksMsec() + (ulong)GD.RandRange(MinMoveMs, MaxMoveMs);
                state = State.Moving;
                break;

            case State.Moving:
                bee.FadeTo(1f);

                // continuously rotate angle — this is what makes the curve visible
                angle += turnBias * (float)bee.GetProcessDeltaTime();

                // soft hive leash
                var toHive = bee.Home.GlobalPosition - bee.GlobalPosition;
                if (toHive.Length() > LeashTiles * GameStore.TILE_SIZE)
                    angle = Mathf.LerpAngle(
                        angle,
                        toHive.Angle(),
                        LeashPull * (float)bee.GetProcessDeltaTime()
                    );

                // rolling lookahead — update every frame so movement curves
                bee.MoveTo(
                    bee.GlobalPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Lookahead
                );

                if (Time.GetTicksMsec() >= stateUntil)
                {
                    stateUntil = Time.GetTicksMsec() + (ulong)GD.RandRange(MinWaitMs, MaxWaitMs);
                    state = State.Waiting;
                }
                break;
        }
    }
}
