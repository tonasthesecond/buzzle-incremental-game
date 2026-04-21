using System.Collections.Generic;
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
    protected Flower? flower;

    /// Filter for eligible flowers.
    protected abstract bool IsEligible(Flower f);

    /// Called once on arrival at flower. Return false to abort and re-seek.
    protected abstract bool OnArrived(Bee bee);

    /// Called after animation finishes (only used if WaitForAnimation = true).
    protected virtual void OnAnimationFinished(Bee bee) { }

    /// Called once per tick while in PreMove - move to flower when ready, return true to proceed.
    protected virtual bool PreMoveTick(Bee bee)
    {
        bee.MoveTo(flower!.GlobalPosition);
        return true;
    }

    /// Override to block seeking until conditions are met.
    protected virtual bool CanSeek(Bee bee) => true;

    /// Called when the bee is at the flower, return true to proceed.
    protected virtual void ProcessAtFlower(Bee bee, out bool isDone) => isDone = true;

    /// Called after deposit / job end.
    protected virtual void OnHome(Bee bee)
    {
        bee.Home.Deposit(bee.carryingHoney);
        Services.Get<HoneyTracker>().Record(bee.HoneySource, bee.carryingHoney);
        Services.Get<HoneyTracker>().Record(bee.BeeTypeName, bee.carryingHoney);
        bee.carryingHoney = 0;
        bee.FadeTo(0f);
        bee.SetJob(new IdleJob());
    }

    /// Override to rank/sort candidates before random selection.
    protected virtual IEnumerable<Flower> GetCandidates(Flower[] eligible) => eligible;

    public void Tick(Bee bee)
    {
        Grid grid = Services.Get<Grid>()!;
        BeeSystem beeSystem = Services.Get<BeeSystem>()!;
        switch (state)
        {
            case State.SeekingFlower:
                if (!CanSeek(bee))
                    return;
                var eligible = grid.GetObjectsOfType<Flower>()
                    .Where(f => IsEligible(f) && !beeSystem.IsClaimed(f))
                    .ToArray();
                flower = GetCandidates(eligible).OrderBy(_ => GD.Randf()).FirstOrDefault();
                if (flower == null)
                {
                    bee.SetJob(new IdleJob());
                    return;
                }
                beeSystem.ClaimObject(flower, bee);
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
                if (!OnArrived(bee))
                {
                    bee.SetJob(new IdleJob());
                    return;
                }
                state = State.AtFlower;
                break;
            case State.AtFlower:
                ProcessAtFlower(bee, out bool isDone);
                if (!isDone)
                    return;
                beeSystem.ReleaseObject(flower!);
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
    protected override bool IsEligible(Flower f) => f.CurState == Flower.State.Pollinated;

    protected override bool OnArrived(Bee bee)
    {
        if (flower == null || flower!.Honey <= 0)
            return false;

        int amount = Mathf.Min((int)bee.HoneyCapacity.Value, flower.Honey);
        bee.carryingHoney += amount;
        flower.Honey -= amount;
        return true;
    }
}

public class PollinatorJob : FlowerJob
{
    protected override bool IsEligible(Flower f) => f.CurState == Flower.State.Pollinating;

    protected override bool CanSeek(Bee bee) => GameStore.Honey > 0;

    protected override bool PreMoveTick(Bee bee)
    {
        int needed = Mathf.Min(flower!.HoneyRequired(), (int)bee.HoneyCapacity.Value);
        bee.carryingHoney = bee.Home.TakePossible(needed);
        bee.HoneySource = flower.ObjectName;
        if (bee.carryingHoney == 0)
        {
            state = State.SeekingFlower;
            return false;
        }
        bee.MoveTo(flower!.GlobalPosition);
        return true;
    }

    protected override bool OnArrived(Bee bee)
    {
        flower!.AddHoney(bee.carryingHoney);
        if (flower!.HoneyRequired() == 0)
            bee.StartPollinatingAnim(flower!.GlobalPosition, flower!.PollinationTime.Value);
        return true;
    }

    protected override void ProcessAtFlower(Bee bee, out bool isDone)
    {
        if (bee.IsAnimating)
        {
            isDone = false;
            return;
        }
        bee.carryingHoney = 0;
        if (flower.CurState == Flower.State.Pollinated)
            flower.Pollinate();
        isDone = true;
    }

    // pollinator doesn't deposit on home — honey was already spent
    protected override void OnHome(Bee bee) => bee.SetJob(new IdleJob());
}

/// Shared rocket charge behavior
public abstract class RocketFlowerJob : FlowerJob
{
    private ulong chargeStartMs;

    protected virtual bool PreCharge(Bee bee) => true;

    protected override bool PreMoveTick(Bee bee)
    {
        if (chargeStartMs == 0)
        {
            if (!PreCharge(bee)) // abort back to seeking if prep fails
            {
                state = State.SeekingFlower;
                return false;
            }

            Services.Get<BeeSystem>().ClaimObject(flower!, bee);
            chargeStartMs = Time.GetTicksMsec();
            bee.Speed.AddPercent("rocket_charge", -GameStore.RocketBeeChargeSpeedDebuff.Value);
            var pullDir = (bee.GlobalPosition - flower!.GlobalPosition).Normalized();
            bee.MoveTo(bee.GlobalPosition + pullDir * GameStore.RocketBeeChargeDistance.Value);
            bee.Sprite.FlipH = flower!.GlobalPosition.X < bee.GlobalPosition.X;
        }
        bee.FlipOverride = flower!.GlobalPosition.X < bee.GlobalPosition.X;

        if (Time.GetTicksMsec() - chargeStartMs < (ulong)GameStore.RocketBeeChargeTime.Value)
            return false;

        bee.FlipOverride = null;
        bee.Speed.Remove("rocket_charge");
        bee.Speed.AddPercent("rocket_boost", GameStore.RocketBeeSpeedBuff.Value);
        bee.MoveTo(flower!.GlobalPosition);
        return true;
    }

    // intercept arrival to strip speed buff, then delegate
    protected sealed override bool OnArrived(Bee bee)
    {
        bee.Speed.Remove("rocket_boost");
        return OnRocketArrived(bee);
    }

    protected abstract bool OnRocketArrived(Bee bee);
}

public class RocketHarvesterJob : RocketFlowerJob
{
    protected override bool IsEligible(Flower f) => f.CurState == Flower.State.Pollinated;

    protected override bool OnRocketArrived(Bee bee)
    {
        if (flower!.Honey <= 0)
            return false;

        int amount = Mathf.Min((int)bee.HoneyCapacity.Value, flower.Honey);
        bee.carryingHoney += amount;
        flower.Honey -= amount;
        return true;
    }
}

public class RocketIsolatedHarvesterJob : RocketHarvesterJob
{
    protected override IEnumerable<Flower> GetCandidates(Flower[] eligible)
    {
        if (eligible.Length == 0)
            return eligible;

        var grid = Services.Get<Grid>();
        var hives = grid.GetObjectsOfType<Hive>();

        var mostIsolated = eligible
            .Select(f => new
            {
                Flower = f,
                ClosestHiveDist = hives.Min(h =>
                    h.GlobalPosition.DistanceSquaredTo(f.GlobalPosition)
                ),
            })
            .OrderByDescending(x => x.ClosestHiveDist)
            .First()
            .Flower;

        return new[] { mostIsolated };
    }
}

public class RocketPollinatorJob : RocketFlowerJob
{
    protected override bool IsEligible(Flower f) => f.CurState == Flower.State.Pollinating;

    protected override bool CanSeek(Bee bee) => GameStore.Honey > 0;

    protected override bool PreCharge(Bee bee)
    {
        int needed = Mathf.Min(flower!.HoneyRequired(), (int)bee.HoneyCapacity.Value);
        bee.carryingHoney = bee.Home.TakePossible(needed);
        return bee.carryingHoney > 0;
    }

    protected override bool OnRocketArrived(Bee bee)
    {
        flower!.AddHoney(bee.carryingHoney);
        if (flower!.CurState == Flower.State.Pollinated)
            bee.StartPollinatingAnim(flower!.GlobalPosition, flower!.PollinationTime.Value);
        return true;
    }

    protected override void ProcessAtFlower(Bee bee, out bool isDone)
    {
        if (bee.IsAnimating)
        {
            isDone = false;
            return;
        }
        bee.carryingHoney = 0;
        if (flower.CurState == Flower.State.Pollinated)
            flower.Pollinate();
        isDone = true;
    }

    protected override void OnHome(Bee bee) => bee.SetJob(new IdleJob());
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
    private static readonly float RoseLeashPull = 2.2f; // radians/sec toward closest rose
    private static readonly float FlockPull = 0.8f; // radians/sec toward hive-mates
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
                // rose leash (if enabled and rose exists)
                if (GameStore.QueenBeeLeashRose)
                {
                    Rose? rose = Services
                        .Get<Grid>()
                        .GetClosestObjectOfType<Rose>(bee.Home.GlobalPosition);
                    if (rose != null)
                    {
                        Vector2 toRose = rose.GlobalPosition - bee.GlobalPosition;
                        angle = Mathf.LerpAngle(angle, toRose.Angle(), RoseLeashPull * dt);
                    }
                }
                // bias toward visible hive-mates
                Bee[] hivemates = Services
                    .Get<BeeSystem>()
                    .GetBees()
                    .Where(b => b != bee && b.Home == bee.Home && b.Visible)
                    .ToArray();
                if (hivemates.Length > 0)
                {
                    Vector2 center = Vector2.Zero;
                    foreach (Bee b in hivemates)
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
