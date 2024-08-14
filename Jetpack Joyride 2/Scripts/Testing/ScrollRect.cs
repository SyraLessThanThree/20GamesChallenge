using Godot;
using System;

public partial class ScrollRect : ColorRect
{
	private ShaderMaterial myShader;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		myShader = (Material as ShaderMaterial);
		RectChanged();
	}
	
	public void RectChanged(){
		myShader.SetShaderParameter("node_pos", Position);
		myShader.SetShaderParameter("rect_size", Size);
	}
}
