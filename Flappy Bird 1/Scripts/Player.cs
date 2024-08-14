using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Signal] public delegate void DeathEventHandler();
	
	
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public bool dying = false;

	public int score = 0;

	[Export]private PipesSpawner pipes;
	[Export]private Label scoreLabel;
	public AnimatedSprite2D sprite;

	private Vector2 Velocity = new Vector2(0,0);

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = 0f;

	public override void _Ready()
	{
		GD.Randomize();
		sprite = GetNode<AnimatedSprite2D>("Bird");
		sprite.Animation = "default";
		sprite.Play();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		velocity.Y += gravity * (float)delta;

		if (gravity == 0 && Input.IsActionJustPressed("ui_accept"))
		{
			gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
			pipes.isSpawning = true;
			pipes.spawnTimer.Start();
			pipes.SpawnPipe();
			var sprite = GetNode<AnimatedSprite2D>("Bird");
			sprite.Animation = "bird";
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && !dying)
		{
			velocity.Y = JumpVelocity;
			sprite.Animation = "flap";
			GetTree().CreateTimer(.3).Timeout += () => sprite.Animation = "bird";

		}

		Velocity = velocity;
		var coll = MoveAndCollide(Velocity * (float)delta,false,0);
		if (Node.IsInstanceValid(coll)) //we collided
		{
			Die();
		}
		/*if (Node.IsInstanceValid(coll) || TestMove(Transform,new Vector2(1,0))) //we collided or have a pipe next to us
		{
			Die();
		}*/
	}

	public void Die()
	{
		GD.Print("Player ded");
		
		//Window Shake
		var window = GetWindow();
		
		//params
		float shakeMinAng = 20;
		Vector2 shakeIntesity = new Vector2(0,5);

		Vector2I origPos = window.Position;
		
		//tweens
		Tween windowTween = GetTree().CreateTween();
		
		//math
		float toRad = 180 * Mathf.Pi;
		float totalAngle = (float)GD.RandRange(0, 2 * Mathf.Pi);
		float upAngle = (float)GD.RandRange(shakeMinAng * toRad, (180 - shakeMinAng) * toRad);

		for (int i = 0; i < 4; i++)
		{
			windowTween.TweenProperty(window, "position", 
				(Vector2I)(origPos + Vector2.FromAngle(totalAngle+upAngle) * (float)GD.RandRange(shakeIntesity.X,shakeIntesity.Y))
				,.05);

			upAngle = (float)GD.RandRange(shakeMinAng * toRad, (180 - shakeMinAng) * toRad);
			totalAngle += (float)GD.RandRange(2 * Mathf.Pi / 3, 2 * Mathf.Pi / 2);
		}
		
		windowTween.TweenProperty(window, "position", 
			(Vector2I)(origPos)
		,.2);
		
		windowTween.Play();
		
		/*	
		var realAngles = spawnAngles / 360f * Mathf.Pi * 2;
		Velocity = Vector2.FromAngle((float)GD.RandRange(realAngles.X,realAngles.Y)) * speed;
		Velocity = new Vector2(Mathf.Sign(Velocity.X) * speed, Velocity.Y);
		*/
		
		pipes.isSpawning = false;
		dying = true;
		Velocity = new Vector2(0,JumpVelocity/2f);
		gravity /= 4f;
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "rotation", -90f/360f*2*Mathf.Pi, 3f);
		CollisionMask = 0;
		GetTree().CreateTimer(5).Timeout += () => GetTree().ReloadCurrentScene();
		pipes.spawnTimer.Stop();
		pipes.spawnTimer.Paused = true;

		EmitSignal(SignalName.Death);
	}

	public int Score(int add)
	{
		score += add;
		scoreLabel.Text = score.ToString();
		return score;
	}
}
