using Godot;
using System;

public partial class Wall : Line2D
{
	private CollisionShape2D coll;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		coll = GetNode<CollisionShape2D>("Body/Coll");
		SegmentShape2D collLine = (SegmentShape2D)coll.Shape;
		collLine.A = this.Points[0];
		collLine.B = this.Points[1];
		//coll.Shape = collLine;
		GD.Print(String.Format("Set Wall Coll of {0} with Parent {1}",Name,GetParent().Name));
		GD.Print(String.Format("A: {0} B: {1}",Points[0],Points[1]));
		GD.Print(String.Format("Found A: {0} Found B: {1}\n",((SegmentShape2D)coll.Shape).A,((SegmentShape2D)coll.Shape).B ));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
