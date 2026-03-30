using System.Linq;
using Godot;

public interface IBeeJob
{
    void Tick(BeeEntity bee);
}

public class IdleJob : IBeeJob
{
    public void Tick(BeeEntity bee) => bee.FadeTo(0f);
}

/// Shared skeleton: seek flower -> [pre-move hook] -> travel -> arrive -> [animation] -> home.
public abstract class FlowerJob : IBeeJob
{
    protected enum State
    {
        SeekingFlower,
        PreMove,
        TravelingToFlower,
        AtFlower,
        TravelingHome,
    }

    protected State state = State.SeekingFlower;
    protected BaseFlower? flower;

    /// Filter for eligible flowers.
    protected abstract bool IsEligible(BaseFlower f);

    /// Called once on arrival at flower. Return false to abort and re-seek.
    protected abstract bool OnArrived(BeeEntity bee);

    /// Whether to wait for IsAnimating before going home.
    protected virtual bool WaitForAnimation => false;

    /// Called after animation finishes (only used if WaitForAnimation = true).
    protected virtual void OnAnimationFinished(BeeEntity bee) { }

    /// Called once per tick while in PreMove - move to flower when ready, return true to proceed.
    protected virtual bool PreMoveTick(BeeEntity bee)
    {
        bee.MoveTo(flower!.GlobalPosition);
        return true;
    }

    /// Override to block seeking until conditions are met.
    protected virtual bool CanSeek(BeeEntity bee) => true;

    /// Called after deposit / job end.
    protected virtual void OnHome(BeeEntity bee)
    {
        bee.Home.Deposit(bee.carryingHoney);
        bee.carryingHoney = 0;
        bee.FadeTo(0f);
        bee.SetJob(new IdleJob());
    }

    public void Tick(BeeEntity bee)
    {
        Grid grid = Services.Get<Grid>()!;
        BeeSystem beeSystem = Services.Get<BeeSystem>()!;

        switch (state)
        {
            case State.SeekingFlower:
                if (!CanSeek(bee))
                    return;

                flower = grid.GetObjectsOfType<BaseFlower>()
                    .Where(f => IsEligible(f) && !beeSystem.IsClaimed(f))
                    .OrderBy(_ => GD.Randf())
                    .FirstOrDefault();

                if (flower == null)
                {
                    if (!grid.GetObjectsOfType<BaseFlower>().Any(IsEligible))
                        bee.SetJob(new IdleJob());
                    return;
                }

                beeSystem.ClaimObject(flower);
                bee.FadeTo(1f);
                state = State.PreMove;
                break;

            case State.PreMove:
                if (PreMoveTick(bee))
                    state = State.TravelingToFlower;
                break;

            case State.TravelingToFlower:
                if (bee.IsMoving)
                    return;

                beeSystem.ReleaseObject(flower!);
                if (!OnArrived(bee))
                {
                    state = State.SeekingFlower;
                    return;
                }
                if (WaitForAnimation)
                {
                    state = State.AtFlower;
                }
                else
                {
                    bee.MoveTo(bee.Home.GlobalPosition);
                    state = State.TravelingHome;
                }
                break;

            case State.AtFlower:
                if (bee.IsAnimating)
                    return;

                OnAnimationFinished(bee);
                bee.MoveTo(bee.Home.GlobalPosition);
                state = State.TravelingHome;
                break;

            case State.TravelingHome:
                if (bee.IsMoving)
                    return;

                OnHome(bee);
                break;
        }
    }
}

public class HarvesterJob : FlowerJob
{
    protected override bool IsEligible(BaseFlower f) => f.Honey > 0;

    protected override bool OnArrived(BeeEntity bee)
    {
        if (flower!.Honey <= 0)
            return false; // picked clean mid-flight

        int amount = Mathf.Min((int)bee.HoneyCapacity.Value, flower.Honey);
        bee.carryingHoney += amount;
        flower.Honey -= amount;
        return true;
    }
}

public class PollinatorJob : FlowerJob
{
    protected override bool IsEligible(BaseFlower f) => f.Honey <= 0;

    protected override bool WaitForAnimation => true;

    protected override bool CanSeek(BeeEntity bee) => GameStore.Honey > 0;

    protected override bool PreMoveTick(BeeEntity bee)
    {
        int needed = Mathf.Min(flower!.HoneyRequired(), (int)bee.HoneyCapacity.Value);
        bee.carryingHoney = bee.Home.TakePossible(needed);
        if (bee.carryingHoney == 0)
        {
            state = State.SeekingFlower;
            return false;
        }
        bee.MoveTo(flower!.GlobalPosition);
        return true;
    }

    protected override bool OnArrived(BeeEntity bee)
    {
        bee.StartPollinatingAnim(flower!.GlobalPosition);
        return true;
    }

    protected override void OnAnimationFinished(BeeEntity bee)
    {
        flower!.Pollinate(bee.carryingHoney);
        Services.Get<BeeSystem>().ReleaseObject(flower!);
        bee.carryingHoney = 0;
    }

    // pollinator doesn't deposit on home — honey was already spent
    protected override void OnHome(BeeEntity bee) => bee.SetJob(new IdleJob());
}

/// Shared rocket charge behavior
public abstract class RocketFlowerJob : FlowerJob
{
    private ulong chargeStartMs;
    private const float PullbackDistance = 20f;
    private bool chargeDebuffActive;
    private bool speedBuffActive;

    protected virtual bool PreCharge(BeeEntity bee) => true;

    protected override bool PreMoveTick(BeeEntity bee)
    {
        if (chargeStartMs == 0)
        {
            if (!PreCharge(bee)) // abort back to seeking if prep fails
            {
                state = State.SeekingFlower;
                return false;
            }

            chargeStartMs = Time.GetTicksMsec();
            bee.Speed.AddPercent("rocket_charge", -GameStore.RocketBeeChargeSpeedDebuff.Value);
            chargeDebuffActive = true;
            var pullDir = (bee.GlobalPosition - flower!.GlobalPosition).Normalized();
            bee.MoveTo(bee.GlobalPosition + pullDir * PullbackDistance);
            bee.Sprite.FlipH = flower!.GlobalPosition.X < bee.GlobalPosition.X;
        }
        bee.FlipOverride = flower!.GlobalPosition.X < bee.GlobalPosition.X;

        if (Time.GetTicksMsec() - chargeStartMs < (ulong)GameStore.RocketBeeChargeTime.Value)
            return false;

        if (chargeDebuffActive)
        {
            bee.Speed.Remove("rocket_charge");
            chargeDebuffActive = false;
        }

        bee.FlipOverride = null;
        bee.Speed.AddPercent("rocket_boost", GameStore.RocketBeeSpeedBuff.Value);
        speedBuffActive = true;
        bee.MoveTo(flower!.GlobalPosition);
        return true;
    }

    // intercept arrival to strip speed buff, then delegate
    protected sealed override bool OnArrived(BeeEntity bee)
    {
        if (speedBuffActive)
        {
            bee.Speed.Remove("rocket_boost");
            speedBuffActive = false;
        }
        return OnRocketArrived(bee);
    }

    protected abstract bool OnRocketArrived(BeeEntity bee);
}

public class RocketHarvesterJob : RocketFlowerJob
{
    protected override bool IsEligible(BaseFlower f) => f.Honey > 0;

    protected override bool OnRocketArrived(BeeEntity bee)
    {
        if (flower!.Honey <= 0)
            return false;

        int amount = Mathf.Min((int)bee.HoneyCapacity.Value, flower.Honey);
        bee.carryingHoney += amount;
        flower.Honey -= amount;
        return true;
    }
}

public class RocketPollinatorJob : RocketFlowerJob
{
    protected override bool IsEligible(BaseFlower f) => f.Honey <= 0;

    protected override bool WaitForAnimation => true;

    protected override bool CanSeek(BeeEntity bee) => GameStore.Honey > 0;

    // in RocketPollinatorJob
    protected override bool PreCharge(BeeEntity bee)
    {
        int needed = Mathf.Min(flower!.HoneyRequired(), (int)bee.HoneyCapacity.Value);
        bee.carryingHoney = bee.Home.TakePossible(needed);
        return bee.carryingHoney > 0;
    }

    protected override bool OnRocketArrived(BeeEntity bee)
    {
        bee.StartPollinatingAnim(flower!.GlobalPosition);
        return true;
    }

    protected override void OnAnimationFinished(BeeEntity bee)
    {
        flower!.Pollinate(bee.carryingHoney);
        Services.Get<BeeSystem>().ReleaseObject(flower!);
        bee.carryingHoney = 0;
    }

    protected override void OnHome(BeeEntity bee) => bee.SetJob(new IdleJob());
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
    private static readonly float FlockPull = 0.8f; // radians/sec toward hive-mates
    private static readonly float TurnBiasMax = 2.2f; // radians/sec max curve
    private static readonly float TurnBiasShift = 1f; // how much bias shifts on each new bout
    private static readonly float Lookahead = 24f; // pixels ahead to target

    private State state = State.Waiting;
    private ulong stateUntil = 0;
    private float angle;
    private float turnBias = 0f;
    private bool initialized = false;

    public void Tick(BeeEntity bee)
    {
        if (!initialized)
        {
            angle = GD.Randf() * Mathf.Tau;
            turnBias = (float)GD.RandRange(-TurnBiasMax, TurnBiasMax);
            stateUntil = Time.GetTicksMsec() + (ulong)GD.RandRange(MinWaitMs, MaxWaitMs);
            initialized = true;
        }

        float dt = (float)bee.GetProcessDeltaTime();

        switch (state)
        {
            case State.Waiting:
                bee.MoveTo(bee.GlobalPosition); // stay put
                if (Time.GetTicksMsec() < stateUntil)
                    return;

                turnBias += (float)GD.RandRange(-TurnBiasShift, TurnBiasShift);
                turnBias = Mathf.Clamp(turnBias, -TurnBiasMax, TurnBiasMax);
                stateUntil = Time.GetTicksMsec() + (ulong)GD.RandRange(MinMoveMs, MaxMoveMs);
                state = State.Moving;
                break;

            case State.Moving:
                bee.FadeTo(1f);

                // continuously rotate angle
                angle += turnBias * dt;

                // soft hive leash
                Vector2 toHive = bee.Home.GlobalPosition - bee.GlobalPosition;
                if (toHive.Length() > LeashTiles * GameStore.TILE_SIZE)
                    angle = Mathf.LerpAngle(angle, toHive.Angle(), LeashPull * dt);

                // bias toward visible hive-mates
                BeeEntity[] hivemates = Services
                    .Get<BeeSystem>()
                    .GetBees()
                    .Where(b => b != bee && b.Home == bee.Home && b.Visible)
                    .ToArray();
                if (hivemates.Length > 0)
                {
                    Vector2 center = Vector2.Zero;
                    foreach (BeeEntity b in hivemates)
                        center += b.GlobalPosition;
                    center /= hivemates.Length;
                    Vector2 toFlock = center - bee.GlobalPosition;
                    if (toFlock.Length() > GameStore.TILE_SIZE)
                        angle = Mathf.LerpAngle(angle, toFlock.Angle(), FlockPull * dt);
                }

                // rolling lookahead — updates every frame so movement curves
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
