using Godot;
using System;

public partial class Pipe : AnimatableBody2D
{
	private CollisionShape2D[] Collisions = new CollisionShape2D[2];
	private Line2D[] LineUp = new Line2D[2];
	private Line2D[] LineDown = new Line2D[2];

	public bool scored = false;

	[Export] public float despawnX = -70;

	[Export] public float holePos = 648/2f;
	[Export] public float holeSize = 200;

	private Vector2 MinMaxY = Vector2.Zero;
	private float AccentHeight = 15;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Collisions[0] = GetNode<CollisionShape2D>("CollUp");
		Collisions[1] = GetNode<CollisionShape2D>("CollDown");

		LineUp[0] = GetNode<Line2D>("Visual/LineUp");
		LineUp[1] = GetNode<Line2D>("Visual/PipeAccentUp");

		LineDown[0] = GetNode<Line2D>("Visual/LineDown");
		LineDown[1] = GetNode<Line2D>("Visual/PipeAccentDown");

		MinMaxY = new Vector2(LineUp[0].Points[0].Y, LineDown[0].Points[1].Y);

		UpdateStuff();
	}

	public void UpdateStuff()
	{
		UpdateLines();
		AlignCollider();
	}

	public void UpdateLines()
	{
		LineUp[0].SetPointPosition(1,new Vector2(0,holePos-holeSize/2f));
		LineDown[0].SetPointPosition(0,new Vector2(0,holePos+holeSize/2f));
		
		LineUp[1].SetPointPosition(0,new Vector2(0,holePos-holeSize/2f));
		LineDown[1].SetPointPosition(0,new Vector2(0,holePos+holeSize/2f));
		
		LineUp[1].SetPointPosition(1,new Vector2(0,LineUp[1].Points[0].Y-AccentHeight));
		LineDown[1].SetPointPosition(1,new Vector2(0,LineDown[1].Points[0].Y+AccentHeight));
	}

	public void AlignCollider()
	{
		Vector2 offset = new Vector2(0, -648 / 2f);
		float collWidth = Collisions[0].Shape.GetRect().Size.X;
		float upHeight = LineUp[0].Points[1].Y;
		float downHeight = LineDown[0].Points[1].Y-LineDown[0].Points[0].Y;
		
		Collisions[0].Shape.Set("size", new Vector2(collWidth, upHeight));
		Collisions[1].Shape.Set("size", new Vector2(collWidth, downHeight));

		Collisions[0].Position = new Vector2(Collisions[0].Position.X,upHeight/2f)+offset;
		Collisions[1].Position = new Vector2(Collisions[1].Position.X,MinMaxY.Y-downHeight/2f)+offset;
	}

	public override void _PhysicsProcess(double delta)
	{
		/*if (!scored && GlobalPosition.X <= player.GlobalPosition.X)
		{
			scored = true;
			//player.Score(1);
		}*/
	}

	public static Pipe SpawnPipe(Node2D movingNode, float holeSize_, float holePos_, Vector2? offset_ = null)
	{
		
		Pipe newPipe = ResourceLoader.Load<PackedScene>("res://Prefabs/Spawns/Pipe.tscn").Instantiate<Pipe>();
		movingNode.AddChild(newPipe);
		//newPipe.Position += offset_.GetValueOrDefault(new Vector2(0,-Spawner.yMiddleOffset));
		newPipe.Position += offset_.GetValueOrDefault(new Vector2(0,0));
		newPipe.holeSize = holeSize_;
		newPipe.holePos = holePos_;
		newPipe.UpdateStuff();
		return newPipe;
	}
}
