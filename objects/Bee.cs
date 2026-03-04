using Godot;
using System;

public partial class Bee : Node2D
{

	bool isMoving = false;
	Vector2 target;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Hello");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!isMoving)
		{
			target = GD.Randf() * GetViewportRect().Size;
			isMoving = true;
		}
		else
		{
			Position = Position.Lerp(target, (float)delta * 1f);
			if (Position.DistanceTo(target) < 10f)
			{
				isMoving = false;
			}
		}
	}
}
