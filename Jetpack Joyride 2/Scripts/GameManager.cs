using Godot;
using System;

public partial class GameManager : Node
{

	[Export]private bool lifeCheat = false;
	[Export]public Player player;
	[Export] private bool ShowFPS = false;

	public static GameManager gameManager;

	public enum GameState{ IDLE,PLAYING,DYING, }
	public static GameState currState = GameState.IDLE;

	private static String saveFolderName = "user://";
	private static String saveFileName = "highscore.save";
	private static DirAccess saveFolder;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		saveFolder = DirAccess.Open(saveFolderName);
		gameManager = this;
	}

	public static void SaveHighScore(uint score)
	{
		using var saveFile = FileAccess.Open(saveFolderName+saveFileName, FileAccess.ModeFlags.Write);
		saveFile.Store32(score);
		saveFile.Close();
	}

	public static uint LoadHighScore()
	{
		//CheckSavegame();
		using var saveFile = Godot.FileAccess.Open(saveFolderName+saveFileName, FileAccess.ModeFlags.Read);
		uint o = 0;
		if (saveFile == null)
		{
			GD.PrintErr(FileAccess.GetOpenError());
		}else if (saveFile.IsOpen())
		{
			GD.Print("Open: " + saveFile);
			o = saveFile.Get32();
			saveFile.Close();
		}else{
			o = 0;
			saveFile.Close();
			GD.Print("Exists Not Open: "+saveFile);
		}
		return o;
	}

	public static void CheckSavegame()
	{
		if (!DirAccess.DirExistsAbsolute(saveFolderName))
		{
			GD.PrintErr("user folder doesnt exist");
		}
		if (!FileAccess.FileExists(saveFolderName+saveFileName))
		{
			using var saveFile = FileAccess.Open(saveFolderName+saveFileName, FileAccess.ModeFlags.Write);
			saveFile.Store32(0);
			saveFile.Close();
			if(FileAccess.FileExists(saveFolderName+saveFileName) || saveFile == null){GD.PrintErr("Couldnt create file");}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (lifeCheat) player.health = 3;
		if (Input.IsActionJustPressed("restart")) Reload();
		if (Input.IsActionJustPressed("pause")) GetTree().Paused = !GetTree().Paused;
		if(ShowFPS) GD.Print("FPS: "+Performance.GetMonitor(Performance.Monitor.TimeFps));
	}

	public static void Reload()
	{
		gameManager.GetTree().ReloadCurrentScene();
		GameManager.currState = GameManager.GameState.IDLE;
	}
	
	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			CheckSavegame();
			if(player.currHighScore > LoadHighScore()){SaveHighScore(player.currHighScore);}
			GetTree().Quit(); // default behavior
		}
	}
}
