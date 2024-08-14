using Godot;
using System;

public partial class Player : Node2D
{
	private Paddle paddle;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		paddle = GetNode<Paddle>("Paddle");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		paddle.wantedY = GetGlobalMousePosition().Y;
	}
}
