using Godot;

[GlobalClass]
public partial class Flower : BaseGridObject
{
    [Signal]
    public delegate void PollinatedEventHandler(int honey);

    [Signal]
    public delegate void EmptiedEventHandler();

    [Signal]
    public delegate void HoneyChangedEventHandler(int newHoney);

    public enum State
    {
        Pollinating,
        Pollinated,
    }

    public State CurState = State.Pollinating;

    public virtual Stat HoneyCost { get; set; } = new(1f);
    public virtual Stat HoneyGain { get; set; } = new(2f);
    public virtual Stat PollinationTime { get; set; } = new(3f);

    private int honey = 0; // honey in the flower, tracks pollination progress and output as well
    public int Honey
    {
        get => honey;
        set
        {
            honey = value;
            EmitSignal(SignalName.HoneyChanged, honey);
            if (honey <= 0)
                OnEmptied();
        }
    }

    public override void _Ready()
    {
        base._Ready();
        SetModulate();
    }

    /// Pollinate the flower, adding honey.
    public void AddHoney(int amount)
    {
        Honey += amount;
        CurState = State.Pollinating;
        if (Honey >= HoneyCost.Value)
            CurState = State.Pollinated;
    }

    /// How many honey is required to pollinate this flower.
    public int HoneyRequired()
    {
        if (CurState == State.Pollinated)
            return 0;
        return (int)HoneyCost.Value - Honey;
    }

    public void Pollinate()
    {
        if (CurState != State.Pollinated)
            return;
        OnPollinated();
    }

    /// Called when the flower is pollinated.
    protected virtual void OnPollinated()
    {
        CurState = State.Pollinated;
        SetModulate();
        Honey = (int)HoneyGain.Value;
        EmitSignal(SignalName.Pollinated, Honey);
    }

    protected virtual void OnEmptied()
    {
        CurState = State.Pollinating;
        SetModulate();
        EmitSignal(SignalName.Emptied);
    }

    private void SetModulate()
    {
        if (CurState == State.Pollinated)
            sprite.Modulate = Colors.White;
        else
            sprite.Modulate = Colors.DarkGray;
    }
}
