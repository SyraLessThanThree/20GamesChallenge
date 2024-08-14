using Godot;
using System;

public partial class Bullet : RigidBody2D
{
	private AnimatedSprite2D sprite;

	[Export] public float bulletBounceDeg = 30;
	[Export] public float rotRange = 10;

	private ulong msecTimeSinceCreated;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		msecTimeSinceCreated = Time.GetTicksMsec();
		sprite = GetNode<AnimatedSprite2D>("Sprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		HandleVisual();

		var coll = MoveAndCollide(Vector2.Zero, true, 0);
		if (Node2D.IsInstanceValid(coll) && ( (coll.GetCollider() as Player is null && coll.GetCollider() as Bullet is null) || (Time.GetTicksMsec()-msecTimeSinceCreated) > 1500) )
		{
			LinearVelocity *= new Vector2(0, -.25f);
			LinearVelocity += coll.GetNormal() * LinearVelocity.Length() * ( (((Node2D)coll.GetCollider()).GetParent().Name.Equals("Walls"))?2:4 );
			LinearVelocity = LinearVelocity.Rotated((float)GD.RandRange(-bulletBounceDeg,bulletBounceDeg)*Mathf.Pi/180f);
			LinearVelocity *= .75f;
			ApplyTorqueImpulse((float)GD.RandRange(-rotRange,rotRange));
			CollisionMask = 0;
			if(GD.RandRange(1,3)==3) CollisionLayer = 0;
			var col = Colors.LightGray;
			col.A = .75f;
			sprite.Modulate = col;
			GetTree().CreateTimer(2).Timeout += () => QueueFree();
		}
	}

	public void HandleVisual()
	{
		sprite.GlobalRotation = 0;

		int origFrame = Mathf.RoundToInt(RotationDegrees / 90f * 4f - .5f) % (8 * 2 + 1);

		int calcedFrame = origFrame % 9;

		if (( (Math.Abs(origFrame) > 8) ^ (origFrame < 0) ) && (Math.Abs(origFrame) > 8)) calcedFrame = -calcedFrame + 7;
		
		
		int usedFrame = (Math.Abs(calcedFrame) % 5);
		
		if ((Math.Abs(calcedFrame) > 4)) usedFrame = Math.Abs(-usedFrame + 3);
		sprite.FlipV = (Math.Abs(calcedFrame) > 4) ^ origFrame < -8;
		sprite.FlipH = ( (calcedFrame < 0) ^ (Math.Abs(origFrame) > 8) );
		sprite.Frame = usedFrame;
		
		//GD.Print("Orig: "+origFrame+" Calced: "+calcedFrame+" Used: "+usedFrame);
	}
}
