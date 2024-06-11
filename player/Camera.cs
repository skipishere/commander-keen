using Godot;
using System;
using System.Diagnostics;

public partial class Camera : Camera2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug.Print("Camera ready");
		
		var tileMap = Owner.GetParent().GetNode<TileMap>("TileMap");
		var tileMapRange = tileMap.GetUsedRect();
		var tileMapCellSize = tileMap.TileSet.TileSize;

		this.LimitLeft = tileMapRange.Position.X * tileMapCellSize.X;
		this.LimitRight = tileMapRange.End.X * tileMapCellSize.X;
		this.LimitTop = (tileMapRange.Position.Y + 1) * tileMapCellSize.Y;
		this.LimitBottom = (tileMapRange.End.Y + 1) * tileMapCellSize.Y;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
