using Godot;
using System;

public partial class ActivateArea : Area2D
{
	[Export] private Node node;
	private void _on_body_entered(Node2D body)
	{
		Laser laser = (Laser)node;
		GD.Print("coll");
		laser.B += new Vector2(0, -50);
		//laser.ChangeLineCoords(laser.A,laser.B + new Vector2(0,-50));
	}
}
