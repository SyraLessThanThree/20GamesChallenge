using Godot;
using System;

public partial class PipesSpawner : Node2D
{

	[Export] public bool isSpawning = false;

	[Export] private Pipe editorOnly;
	[Export] private PackedScene pipeToSpawn;

	[Export] private Player player;
	
	[ExportCategory("Game Variables")]
	[Export] protected float spawnInterval = 1.5f;
	
	[ExportCategory("Pipe Settings")]
	[Export] public float speed = 150;
	//[Export] public float holePos = 648/2f;
	[Export] public float holeSize = 200;
	[Export] public Vector2 holePosVar = new Vector2(50, 648 - 50);


	public Timer spawnTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spawnTimer = new Timer();
		AddChild(spawnTimer);
		spawnTimer.Stop();
		spawnTimer.WaitTime = spawnInterval;
		spawnTimer.Timeout += () => SpawnPipe();
		
		editorOnly.QueueFree();
	}

	public void SpawnPipe()
	{
		Pipe newPipe = pipeToSpawn.Instantiate<Pipe>();
		AddChild(newPipe);
		newPipe.player = player;
		newPipe.speed = speed;
		newPipe.holeSize = holeSize;
		newPipe.holePos = (float)GD.RandRange(holePosVar.X+holeSize/2,holePosVar.Y-holeSize/2);
		newPipe.UpdateStuff();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
