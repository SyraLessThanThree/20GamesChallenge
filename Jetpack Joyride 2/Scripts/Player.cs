using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Signal] public delegate void DeathEventHandler();

	[Export] public Camera2D camera;
	
	public const float JumpAccel = -1000.0f;
	private Vector2 startCnt = new Vector2(0, .5f);
	
	[Export]public float vertSpeed = 100f;
	
	//public Vector2 Velocity = Vector2.Zero;

	public int score = 0;
	public float time = 0f;
	public int health = 3;

	public float currInvincTime = 0f;
	public float invincTime = 1.5f;

	[Export]private Spawner spawner;
	[Export]private Label scoreLabel;
	[ExportCategory("Bullets")]
	[Export]private PackedScene bulletToSpawn;
	[Export]private Node2D bulletParent;
	[Export]public float bulletSpawnRate = .05f;
	[Export] private double bulletSpreadDeg = 10;
	private Node2D bulletSpawnPoint;
	private Timer bulletSpawnTimer;

	[ExportCategory("Scoring")]
	[Export] public float scoreInterval = 10f;
	
	public AnimatedSprite2D jetpackSprite;
	public AnimatedSprite2D guySprite;

	private float X;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready()
	{
		X = Position.X;
		bulletSpawnPoint = GetNode<Node2D>("SpawnBullets");
		bulletSpawnTimer = bulletSpawnPoint.GetNode<Timer>("Timer");
		
		GD.Randomize();
		guySprite = GetNode<AnimatedSprite2D>("Guy");
		guySprite.Animation = "default";
		guySprite.Play();
		
		jetpackSprite = GetNode<AnimatedSprite2D>("Jetpack");
		jetpackSprite.Animation = "default";
		jetpackSprite.Play();

		bulletSpawnTimer.WaitTime = bulletSpawnRate;
		bulletSpawnTimer.Timeout += () =>
		{
			var bullet = bulletToSpawn.Instantiate<Bullet>();
			bulletParent.AddChild(bullet);
			bullet.GlobalPosition = bulletSpawnPoint.GlobalPosition;
			float ang = (float)GD.RandRange(-bulletSpreadDeg, bulletSpreadDeg);
			bullet.Rotate(ang/180*Mathf.Pi);
			bullet.ApplyImpulse(Vector2.Down.Rotated(ang*Mathf.Pi/180f)*600f);
			bullet.ApplyImpulse(Velocity);
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		time += (float)delta;
	}

	public override void _Process(double delta)
	{
		
		if (GameManager.currState == GameManager.GameState.IDLE && Input.IsActionJustPressed("ui_accept")){
			GameManager.currState = GameManager.GameState.PLAYING;
			spawner.StartSpawning();
			guySprite.Animation = "idle";
		}
		if (GameManager.currState == GameManager.GameState.PLAYING)
		{
			UpdateScore();
			
			if (currInvincTime > 0)
			{
				currInvincTime = Mathf.Max(0, currInvincTime - (float)delta);
				//SetCollisionMaskValue(2,false);
			}
			else
			{
				SetAnimationKeepState(guySprite, "idle");
				//SetCollisionMaskValue(2,true);
			}
			
			if (Input.IsActionJustPressed("ui_accept")){
				startCnt.X = startCnt.Y;
				SetAnimationKeepState(jetpackSprite, health + "fly");
				bulletSpawnTimer.Start();
			}else if (Input.IsActionJustReleased("ui_accept"))
			{
				SetAnimationKeepState(jetpackSprite, health + "idle");
				bulletSpawnTimer.Stop();
			}
			
			if(Input.IsActionPressed("ui_accept"))
				startCnt.X = Mathf.Max(0,startCnt.X-(float)delta);

			Velocity += (
				(Input.IsActionPressed("ui_accept")) ? JumpAccel + startCnt.X * JumpAccel : gravity
			) * (float)delta * new Vector2(1,1);

		}
		if (GameManager.currState == GameManager.GameState.DYING)
		{
			Velocity += gravity * (float)delta * new Vector2(1,1);
		}

		// Handle Jump.
		/*if ( && currState != State.DYING)
		{
			var sprite = GetNode<Sprite2D>("Sprite");
			var idleTex = sprite.Texture;
			sprite.Texture = flapTex;
			GetTree().CreateTimer(.3).Timeout += () => sprite.Texture = idleTex;

		}*/
		float dir = 0f;
		if (Input.IsActionPressed("ui_left")) dir += -1;
		if (Input.IsActionPressed("ui_right")) dir += 1;
			
		Velocity = Velocity * new Vector2(0, 1) + new Vector2(dir * vertSpeed,0);

		Velocity *= new Vector2(1,1);
		var coll = MoveAndCollide(Velocity * (float)delta,false,0);
		if (Node.IsInstanceValid(coll) && GameManager.currState == GameManager.GameState.PLAYING)
		{
			CollideWith(coll);
		}
		Position = new Vector2(X,Position.Y);
	}
	
	
	private void OnPickupArea(Area2D collider)
	{
		GD.Print("PickedUp "+collider.Name);
		if ((bool)collider.GetMeta("player_coll_function", false))
		{
			collider.EmitSignal(Coin.SignalName.PlayerColl,this);
		}
	}
	
	public void CollideWith(KinematicCollision2D coll)
	{
		CollisionObject2D collider = (CollisionObject2D)coll.GetCollider();
		if ((bool)collider.GetMeta("player_coll_function", false))
		{
			collider.EmitSignal(Coin.SignalName.PlayerColl,this);
		}
		if ((bool)collider.GetMeta("push_player",false))
		{
			Velocity += new Vector2(0,(coll.GetNormal()*20).Y);
			if ((int)collider.GetMeta("hurt_amount", 0) < 0 && currInvincTime > 0)
			{
				MoveAndSlide();
			}
		}
		if ((int)collider.GetMeta("hurt_amount",0) != 0)
		{
			//MoveAndSlide();
			int hurt = (int)collider.GetMeta("hurt_amount");
			HurtHeal(hurt);
		}
	}

	public int HurtHeal(int amount)
	{
		if ((health-amount)<=0 && currInvincTime <= 0){
			SetAnimationKeepState(jetpackSprite, 0 + (Input.IsActionPressed("ui_accept")?"fly":"idle"));
			bulletSpawnTimer.Stop();
			Die();
			return health;
		}

		if (amount < 0 || (amount > 0 && currInvincTime <= 0)){
			health -= amount;
			health = Math.Clamp(health, 0, 3);
			SetAnimationKeepState(jetpackSprite, health + (Input.IsActionPressed("ui_accept")?"fly":"idle"));
		}
			
		if(currInvincTime <= 0 && amount > 0){
			SetAnimationKeepState(guySprite, "hurt");
			currInvincTime = invincTime;
			ShakeWindow();
		}
		return health;
	}

	public void SetAnimationKeepState(AnimatedSprite2D sprite, string animationName)
	{
		var t = new Vector2(sprite.FrameProgress,sprite.Frame);
		sprite.Animation = animationName;
		sprite.SetFrameAndProgress((int)t.Y,t.X);
	}

	public void ShakeWindow(Nullable<Vector2> shakeRange = null)
	{
		//Window Shake
		var window = GetWindow();

		bool useCamera = window.Mode != Window.ModeEnum.Windowed;
		
		//params
		float shakeMinAng = 20;
		Vector2 shakeIntesity = shakeRange.HasValue ? shakeRange.Value : new Vector2(0,5);
		
		GodotObject positioner = useCamera ? camera : window;
		Vector2 origPos = (Vector2)positioner.Get("position");
		
		//tweens
		Tween windowTween = GetTree().CreateTween();
		
		//math
		float toRad = 180 * Mathf.Pi;
		float totalAngle = (float)GD.RandRange(0, 2 * Mathf.Pi);
		float upAngle = (float)GD.RandRange(shakeMinAng * toRad, (180 - shakeMinAng) * toRad);

		//if (useCamera) shakeIntesity = new Vector2(0,0);

		for (int i = 0; i < 4; i++)
		{
			if (useCamera) {
				windowTween.TweenProperty(positioner, "position", 
					(origPos + Vector2.FromAngle(totalAngle+upAngle) * (float)GD.RandRange(shakeIntesity.X,shakeIntesity.Y))
					,.05);
			} else{
				windowTween.TweenProperty(positioner, "position", 
					(Vector2I)(origPos + Vector2.FromAngle(totalAngle+upAngle) * (float)GD.RandRange(shakeIntesity.X,shakeIntesity.Y))
					,.05);
			}

			upAngle = (float)GD.RandRange(shakeMinAng * toRad, (180 - shakeMinAng) * toRad);
			totalAngle += (float)GD.RandRange(2 * Mathf.Pi / 3, 2 * Mathf.Pi / 2);
		}
		
		if (useCamera) {
			windowTween.TweenProperty(positioner, "position", 
				(origPos)
				,.2);
		} else{
			windowTween.TweenProperty(positioner, "position", 
				(Vector2I)(origPos)
				,.2);
		}
		
		windowTween.Play();
	}

	public void Die()
	{
		
		ShakeWindow(new Vector2(5,10));
		
		/*	
		var realAngles = spawnAngles / 360f * Mathf.Pi * 2;
		Velocity = Vector2.FromAngle((float)GD.RandRange(realAngles.X,realAngles.Y)) * speed;
		Velocity = new Vector2(Mathf.Sign(Velocity.X) * speed, Velocity.Y);
		*/
		
		spawner.isSpawning = false;
		GameManager.currState = GameManager.GameState.DYING;
		Velocity = new Vector2(0,-200f);
		gravity /= 4f;
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "rotation", -90f/360f*2*Mathf.Pi, 3f);
		CollisionMask = 0;
		GetTree().CreateTimer(5).Timeout += () => GetTree().ReloadCurrentScene();
		spawner.spawnTimer.Stop();
		spawner.spawnTimer.Paused = true;

		EmitSignal(SignalName.Death);
	}

	public void UpdateScore()
	{
		score += Mathf.FloorToInt(time*scoreInterval);
		time = time % (1/scoreInterval);
		UpdateScoreLabel();
	}

	public int ScorePoints(int add)
	{
		score += add;
		UpdateScoreLabel();
		return score;
	}

	public void UpdateScoreLabel()
	{
		scoreLabel.Text = score.ToString();
	}
}
