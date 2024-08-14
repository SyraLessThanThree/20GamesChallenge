using Godot;
using System;

public partial class Ball : CharacterBody2D
{
	[Export] private Scores scores;
	
	[ExportGroup("Out Of Bounds X")] [Export]
	public Vector2 OOBX = new Vector2(0, 1150);

	[Export]public Vector2 spawnAngles = new Vector2(-45f,45f);

	[Export] public float speed = 400f;

	[Export]public float respawnTimeSec = 1.5f;
	private Vector2 spawnPoint;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spawnPoint = GlobalPosition;
		GetTree().CreateTimer(respawnTimeSec).Timeout += StartMoving;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if(Velocity == Vector2.Zero) return;
		
		var collData = MoveAndCollide(Velocity * (float)delta);
		if (Node.IsInstanceValid(collData))
		{
			var collParent = ((Node)collData.GetCollider()).GetParent();
			bool isPaddle = (collParent.Name.Equals("PaddleLine"));

			Paddle paddle = isPaddle ? collParent.GetParent<Paddle>() : null;
			Node character = isPaddle ? collParent.GetParent().GetParent() : null;
			
			GD.Print("Collided with "+ ( isPaddle ? character.Name : collParent.Name) );
			Velocity = Velocity.Bounce(collData.GetNormal());
			if (isPaddle)
			{
				Velocity = (Velocity + paddle.velocity * -15f).Normalized() * speed;
				Velocity = new Vector2(Mathf.Sign(Velocity.X) * speed, Velocity.Y);
			}
		}

		if (OOBX.X > GlobalPosition.X || GlobalPosition.X > OOBX.Y)
		{
			if (GlobalPosition.X > OOBX.Y)
			{
				scores.UpdateScorePlayer(1);
			}
			else
			{
				scores.UpdateScoreNPC(1);
			}
			Velocity = Vector2.Zero;
			GlobalPosition = spawnPoint;
			GetTree().CreateTimer(respawnTimeSec).Timeout += StartMoving;
		}
	}

	private void StartMoving()
	{
		var realAngles = spawnAngles / 360f * Mathf.Pi * 2;
		Velocity = Vector2.FromAngle((float)GD.RandRange(realAngles.X,realAngles.Y)) * speed;
		Velocity = new Vector2(Mathf.Sign(Velocity.X) * speed, Velocity.Y);
	}
	
	
}
