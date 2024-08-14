using Godot;
using System;

public partial class GameManager : Node
{

	[Export]private bool lifeCheat = false;
	[Export]public Player player;
	[Export] private bool ShowFPS = false;

	public enum GameState{ IDLE,PLAYING,DYING, }
	public static GameState currState = GameState.IDLE;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (lifeCheat) player.health = 3;
		if (Input.IsActionJustPressed("restart")) GetTree().ReloadCurrentScene();
		if (Input.IsActionJustPressed("pause")) GetTree().Paused = !GetTree().Paused;
		if(ShowFPS) GD.Print("FPS: "+Performance.GetMonitor(Performance.Monitor.TimeFps));
	}
}
