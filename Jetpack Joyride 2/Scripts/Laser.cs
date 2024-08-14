using Godot;
using System;

public partial class Laser : Node2D
{
	[Export] private Line2D line;
	[Export] private CollisionShape2D shape;
	//[Export] private SegmentShape2D segmentColl;

	[ExportCategory("Coordinates")]
	[Export] public Vector2 A {
		get => line.Points[0];
		set => ChangeLineCoords(value,B);
	}
	[Export] public Vector2 B {
		get => line.Points[1];
		set => ChangeLineCoords(A,value);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateLineColl();
	}

	public void ChangeLineCoords(Vector2 A__, Vector2 B__)
	{
		line.SetPointPosition(0,A__);
		line.SetPointPosition(1,B__);
		//line.Points[0] = A__;
		//line.Points[1] = B__;
		
		UpdateLineColl();
	}

	public void UpdateLineColl()
	{
		SegmentShape2D segmentColl = (SegmentShape2D)shape.Shape;
		segmentColl.A = line.Points[0];
		segmentColl.B = line.Points[1];
	}
}
