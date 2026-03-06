using System.Threading.Tasks;
using Godot;

public partial class Bee : Node2D
{
    [Signal]
    public delegate void ArrivedEventHandler(Bee bee);

    public IBeeJob job = new IdleJob();
    public HiveTile Home;
    public int carryingHoney = 0;
    public Vector2 targetPosition;

    private Sprite2D sprite;
    private float phase;
    private bool wasMoving = false;

    public bool IsMoving =>
        Position.DistanceSquaredTo(targetPosition) >= Mathf.Pow(GameStore.TILE_SIZE / 10f, 2);

    public void MoveTo(Vector2 position) => targetPosition = position;

    public async Task TravelTo(Vector2 position)
    {
        MoveTo(position);
        if (IsMoving)
            await ToSignal(this, SignalName.Arrived);
    }

    public void SetJob(IBeeJob newJob)
    {
        job = newJob;
        job.Start(this);
    }

    public override void _Ready()
    {
        phase = GD.Randf() * Mathf.Tau;
        sprite = GetNode<Sprite2D>("Sprite2D");
        Callable.From(() => targetPosition = Position).CallDeferred();
    }

    public override void _Process(double delta)
    {
        bool moving = IsMoving;
        if (moving)
            Move(delta);
        else if (wasMoving)
            EmitSignal(SignalName.Arrived, this);
        wasMoving = moving;
    }

    void Move(double delta)
    {
        Position = Position.MoveToward(targetPosition, GameStore.BeeSpeed * (float)delta);
        float t = Time.GetTicksMsec() / 1000f;
        sprite.Position = new Vector2(0, 2f * Mathf.Sin(t * Mathf.Tau + phase));
        sprite.FlipH = targetPosition.X < Position.X;
    }
}
