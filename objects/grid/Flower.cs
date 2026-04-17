using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class Flower : BaseGridObject, IHasHoverTitle, IHasHoverDescription, IHasHoverSubtitle, IHasHoverRefresh
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

    // Pollinate the flower.
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

    public string GetHoverSubtitle()
    {
        if (CurState == State.Pollinated)
            return Style.CK($"{Honey} honey", "subtitle");
        if (CurState == State.Pollinating)
            return Style.CK(
                $"pollinating {Honey}/{HoneyCost.Value} ({Honey / HoneyCost.Value:P0})",
                "subtitle"
            );
        return "";
    }

    public override string GetHoverDescription()
    {
        return "";
    }

    private readonly Dictionary<Action, HoneyChangedEventHandler> _refreshHandlers = new();
    private readonly Dictionary<Action, PollinatedEventHandler> _pollinatedHandlers = new();
    private readonly Dictionary<Action, EmptiedEventHandler> _emptiedHandlers = new();

    public void RegisterRefresh(Action onRefresh)
    {
        HoneyChangedEventHandler honeyHandler = (_) => onRefresh();
        _refreshHandlers[onRefresh] = honeyHandler;
        HoneyChanged += honeyHandler;

        PollinatedEventHandler pollinatedHandler = (_) => onRefresh();
        _pollinatedHandlers[onRefresh] = pollinatedHandler;
        Pollinated += pollinatedHandler;

        EmptiedEventHandler emptiedHandler = () => onRefresh();
        _emptiedHandlers[onRefresh] = emptiedHandler;
        Emptied += emptiedHandler;
    }

    public void UnregisterRefresh(Action onRefresh)
    {
        if (_refreshHandlers.Remove(onRefresh, out var h))
            HoneyChanged -= h;
        if (_pollinatedHandlers.Remove(onRefresh, out var p))
            Pollinated -= p;
        if (_emptiedHandlers.Remove(onRefresh, out var e))
            Emptied -= e;
    }
}
