using Godot;
using System;

public partial class Paddle : Node2D
{
	[Export]
	public Vector2 minMax = new Vector2(100, 548);

	[Export] public float speed = 5f;

	public float wantedY;
	public Vector2 velocity = Vector2.Zero;
	

	private Node2D character;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		wantedY = Position.Y;
		character = GetParent<Node2D>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		velocity = GlobalPosition;
		//wantedY = GetGlobalMousePosition().Y;
		var calcY = 0f;
		var dir = (wantedY > GlobalPosition.Y) ? 1f : -1f;

		var speed_ = (speed > Mathf.Abs(wantedY-GlobalPosition.Y)) ? Mathf.Abs(wantedY-GlobalPosition.Y) : speed;
		
		calcY = (float)delta * 100f * dir * speed_
				+ GlobalPosition.Y;
		calcY = Mathf.Clamp(calcY, minMax.X, minMax.Y);
		
		character.GlobalPosition = new Vector2(character.GlobalPosition.X,calcY);
		velocity = (velocity - GlobalPosition).Abs();
	}
}
