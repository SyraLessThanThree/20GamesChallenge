using Godot;
using System;

public partial class HurtComponent : Node
{
	[Export]private int amount = 1;
	private Player player;
	public ComponentManager componentManager;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		componentManager = GetNode<ComponentManager>("/root/World/ComponentManager");
		player = GetNode<GameManager>("/root/World/GameManager").player;
		Node2D parent = GetParent<Node2D>();
		parent.SetMeta("hurt_amount",amount);
		if (parent as Area2D != null)
		{
			Area2D area = parent as Area2D;

			componentManager.ConnectSignal(area,Area2D.SignalName.BodyEntered, (GodotObject body) =>
			{
				if(!(body is Player)) return;
				if(GameManager.currState != GameManager.GameState.PLAYING) return;
				GD.Print("SignalTriggered");
				player.HurtHeal(amount);
			});
		}

		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
