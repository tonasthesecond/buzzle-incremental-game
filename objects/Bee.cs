using Godot;

public partial class Bee : Node2D
{
    public enum State
    {
        Idling,
        Moving,
        Pollinating,
    }

    public State state = State.Idling;

    public int carryingHoney = 0;
    public Vector2 targetPosition;

    public override void _Ready() { }

    public override void _Process(double delta)
    {
        switch (state)
        {
            case State.Idling:
                break;
            case State.Moving:
                Move(delta);
                if ((Position - targetPosition).IsZeroApprox())
                {
                    // if (GridSystem.isFlowerAt(targetPosition)) state = State.Pollinating;
                    state = State.Idling;
                }
                break;
            case State.Pollinating:
                break;
        }
    }

    void Move(double delta)
    {
        Position = Position.MoveToward(targetPosition, GameStore.BeeSpeed * (float)delta);
    }
}
