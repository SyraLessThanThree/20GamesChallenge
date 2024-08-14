using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Spawner : Node2D
{
	[ExportCategory("Runtime Settings")]
	[Export] private bool Disable = false;
	[Export] public bool isSpawning = false;
	[ExportCategory("Prefabs")]
	[Export] private PackedScene spawnTemplates;
	[Export] private PackedScene coin;
	[Export] private PackedScene pipeToSpawn;
	[Export] private PackedScene sawsToSpawn;

	[ExportCategory("Objects")]
	[Export] private Node editorOnly;
	[Export] private Player player;
	
	[ExportCategory("Game Variables")]
	[Export] protected float spawnInterval = 1.5f;
	[Export] public Rect2 screenBounds;
	
	[ExportCategory("Move Object Settings")]
	[Export] public float speed = 150;
	[Export] public float xDespawn = -30f;
	[ExportCategory("Spawned Object Settings")]
	[Export]public static float yMiddleOffset = 324f;
	//[Export] public float holePos = 648/2f;
	[Export] public float holeSize = 200;
	[Export] public Vector2 holePosVar = new Vector2(50, 648 - 50);


	public Timer spawnTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		screenBounds = GetViewportRect();
		
		if(editorOnly != null) editorOnly.QueueFree();
		
		spawnTimer = new Timer();
		AddChild(spawnTimer);
		spawnTimer.Stop();
		spawnTimer.WaitTime = spawnInterval;
		spawnTimer.Timeout += () => SpawnOnce();

		SetupSpawn();
	}

	private List<Node2D> currMovingNodes = new List<Node2D>();


	enum Spawn
	{
		Pipe,
		SawCol,
		Saw,
		Laser,
	}
	Dictionary<Spawn,int> spawnList = new Dictionary<Spawn,int>()
	{
		{Spawn.Pipe,1},
		{Spawn.SawCol,1},
		{Spawn.Saw,4},
		{Spawn.Laser,0},
	};

	int maxChance = 0;
	private void SetupSpawn()
	{
		spawnList.Values.ToList().ForEach(i => maxChance += i);
		
		SetupCoins();
		SetupSaws();
	}

	enum MNmetaNames{ Speed, }
	float speedVariation = 0.15f;
	float tempSpawnMult = 1f;
	
	//DEBUG
	private float MNexpirationTime = 20f;
	public void SpawnOnce()
	{
		tempSpawnMult = 1f;
		Node2D movingNode = new Node2D();
		currMovingNodes.Add(movingNode);
		AddChild(movingNode);
		movingNode.GlobalPosition = GlobalPosition;
		movingNode.SetMeta(MNmetaNames.Speed.ToString(),speed* (float)GD.RandRange(1-speedVariation,1+speedVariation));
		Timer expTimer = new Timer();
		expTimer.WaitTime = MNexpirationTime;
		expTimer.Timeout += () =>
		{
			currMovingNodes.Remove(movingNode);
			movingNode.QueueFree();
		};
		movingNode.AddChild(expTimer);
		expTimer.Start();

		int roll = GD.RandRange(0, maxChance);
		Spawn spawnedObjectType = spawnList.First((pair =>
		{
			if (pair.Value == 0) return false;
			roll -= pair.Value;
			return roll <= 0;
		})).Key;
		GD.Print("Spawned: "+spawnedObjectType.ToString());
		Node2D spawnedObjs;
		//TODO: create moving obj
		switch (spawnedObjectType)
		{
			case Spawn.Pipe:
				float pipeHolePosY = (float)GD.RandRange(holePosVar.X + holeSize / 2, holePosVar.Y - holeSize / 2);
				spawnedObjs = Pipe.SpawnPipe(movingNode,holeSize,pipeHolePosY);
				movingNode.Position += new Vector2(0, +yMiddleOffset);
				SpawnCoinPile(spawnedObjs, new Vector2I(GD.RandRange(0,4), GD.RandRange(1,3)), new Vector2(0, pipeHolePosY-yMiddleOffset),new int[2]);
				break;
			case Spawn.Saw:
				float posY = (float)GD.RandRange(holePosVar.X, holePosVar.Y);
				spawnedObjs = spawnSaw.Invoke(movingNode, new Vector2(0, posY));
				tempSpawnMult = .2f;
				break;
			case Spawn.SawCol:
				float sawColHolePosY = (float)GD.RandRange(holePosVar.X + holeSize / 2, holePosVar.Y - holeSize / 2);
				spawnedObjs = SpawnSawCol(movingNode, holeSize, sawColHolePosY);
				break;
			default:
				spawnedObjs = null;
				break;
		}

		spawnTimer.Start(spawnInterval*tempSpawnMult);
	}

	private Node2D SpawnSawCol(Node2D parent, float holeSize_, float holeY)
	{
		float currY = holeY - holeSize_ / 2f;
		while (currY >= screenBounds.Position.Y)//upper "pipe"
		{
			spawnSaw.Invoke(parent, new Vector2(0, currY));
			currY -= sawSize.Y / 2;
		}
		spawnSaw.Invoke(parent, new Vector2(0, currY));
		
		currY = holeY + holeSize_ / 2f;
		while (currY <= (screenBounds.Position.Y + screenBounds.Size.Y))//lower "pipe"
		{
			spawnSaw.Invoke(parent, new Vector2(0, currY));
			currY += sawSize.Y / 2;
		}
		spawnSaw.Invoke(parent, new Vector2(0, currY));
		
		return parent;
	}

	public void StartSpawning()
	{
		isSpawning = true;
		spawnTimer.Start();
		SpawnOnce();
	}

	Vector2 coinSize = new Vector2(-1,-1);
	private Func<Node2D, Vector2, Coin> spawnCoin;
	public void SetupCoins()
	{
		if (spawnCoin == null)
			spawnCoin = (movingNode, pos_) =>
			{
				Coin o = coin.Instantiate<Coin>();
				movingNode.AddChild(o);
				o.Position = pos_+new Vector2(0,coinSize.Y/2f);
				return o;
			};
		if (coinSize.X == -1 || coinSize.Y == -1)
		{
			var _ = spawnCoin.Invoke(this, Vector2.Zero);
			coinSize = _.GetMeta("size", Vector2.Zero).AsVector2();
			_.QueueFree();
		}
	}
	Vector2 sawSize = new Vector2(-1,-1);
	private Func<Node2D, Vector2, Node2D> spawnSaw;
	public void SetupSaws()
	{
		if (spawnSaw == null)
			spawnSaw = (movingNode, pos_) =>
			{
				Node2D o = sawsToSpawn.Instantiate<Node2D>();
				movingNode.AddChild(o);
				double randX = .2f;
				randX = GD.RandRange(-sawSize.X * randX/2, sawSize.X * randX/2);
				o.Position = pos_+new Vector2((float)randX,sawSize.Y*3/4f);
				return o;
			};
		if (sawSize.X == -1 || sawSize.Y == -1)
		{
			var _ = spawnSaw.Invoke(this, Vector2.Zero);
			sawSize = _.GetMeta("size", Vector2.Zero).AsVector2();
			_.QueueFree();
		}
	}
	
	
	public void SpawnCoinPile(Node2D movingNode, Vector2I amount, Vector2 pos,int[] yOffset)
	{
		if (amount.X <= 0 || amount.Y <= 0) return;

		Vector2 coinSize_ = coinSize;
		coinSize_.Y *= 3/4f;

		for(int x = 0; x < amount.X; x++) for(int y = 0; y < amount.Y; y++)
		{
			//bool isFirst = x == 0;
			//bool isLast = (x == (amount.X - 1)) && !isFirst;
			
			Vector2 localPos = pos;
			localPos.Y += coinSize_.Y * y - amount.Y * coinSize_.Y/2;
			localPos.X += coinSize_.X * x;
			
			if((yOffset.Length-1)>=x) localPos.Y += coinSize_.Y/2 * yOffset[x];
			
			spawnCoin.Invoke(movingNode, localPos);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		List<Node2D> toRemove = new List<Node2D>();
		foreach (var movingNode in currMovingNodes)
		{
			movingNode.Translate((float)delta * movingNode.GetMeta(MNmetaNames.Speed.ToString()).AsSingle() * new Vector2(-1,0));
			if (movingNode.GlobalPosition.X <= xDespawn)
			{
				movingNode.QueueFree();
				toRemove.Add(movingNode);
			}
		}
		
		toRemove.ForEach(((a) => currMovingNodes.Remove(a)));
	}
}
