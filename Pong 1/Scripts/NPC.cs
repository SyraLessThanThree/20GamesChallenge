using Godot;
using System;

public partial class NPC : Node2D
{
	private Paddle paddle;

	[Export] private Ball ball;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		paddle = GetNode<Paddle>("Paddle");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		paddle.wantedY = ball.GlobalPosition.Y;
	}
}
