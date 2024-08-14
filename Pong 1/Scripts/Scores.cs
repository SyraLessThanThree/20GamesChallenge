using Godot;
using System;
using Microsoft.VisualBasic;

public partial class Scores : Node2D
{
	[ExportCategory("Scores")]
	[Export] private Label Player;
	[Export] private Label NPC;

	private int playerScore = 0;
	private int npcScore = 0;
	// Called when the node enters the scene tree for the first time.
	public int UpdateScorePlayer(int add)
	{
		playerScore += add;
		UpdateGraphics();
		return playerScore;
	}
	public int UpdateScoreNPC(int add)
	{
		npcScore += add;
		UpdateGraphics();
		return npcScore;
	}

	public void UpdateGraphics()
	{
		Player.Text = playerScore.ToString();
		NPC.Text = npcScore.ToString();
	}
}
