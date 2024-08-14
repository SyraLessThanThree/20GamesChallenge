using Godot;
using System;

public partial class GameManager : Node
{

	[Export]private bool lifeCheat = false;
	[Export]public Player player;
	[Export] private bool ShowFPS = false;

	public static GameManager gameManager;

	public enum GameState{ IDLE,PLAYING,DYING, }
	public static GameState currState = GameState.IDLE;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameManager = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (lifeCheat) player.health = 3;
		if (Input.IsActionJustPressed("restart")) Reload();
		if (Input.IsActionJustPressed("pause")) GetTree().Paused = !GetTree().Paused;
		if(ShowFPS) GD.Print("FPS: "+Performance.GetMonitor(Performance.Monitor.TimeFps));
	}

	public static void Reload()
	{
		gameManager.GetTree().ReloadCurrentScene();
		GameManager.currState = GameManager.GameState.IDLE;
	}
}
