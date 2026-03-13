using System;
using Godot;

public partial class TestLabel : Label
{
    public override void _Process(double delta)
    {
        Text = GameStore.Honey.ToString();
    }
}
