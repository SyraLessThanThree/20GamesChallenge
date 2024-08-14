using Godot;
using System;

public partial class Coin : Area2D
{
	[Signal]
	public delegate void PlayerCollEventHandler(Player player);

	[Export] public int score = 40;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayerColl += (player => PlayerCollisionFun(player));
	}

	private void PlayerCollisionFun(Player player)
	{
		player.ScorePoints(score);
		QueueFree();
	}
}
