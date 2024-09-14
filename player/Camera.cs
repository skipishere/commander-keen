using Godot;
using System;
using System.Diagnostics;

public partial class Camera : Camera2D
{
    public static Rect2 CameraRect { get; private set; }

	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var layers = GetTree().GetNodesInGroup("LevelLayers");
        if (layers == null)
        {
            Debug.Print("Failed to set camera limits, no layers found.");
        }
        else
        {
            (Vector2I start, Vector2I end) = (new Vector2I(), new Vector2I());
            foreach (var layer in layers)
            {
                if (layer is TileMapLayer tileMapLayer)
                {
                    var tileMapRange = tileMapLayer.GetUsedRect();
                    var tileMapCellSize = tileMapLayer.TileSet.TileSize;
                    var correction = new Vector2I(0, 1); // We need to show the top of the top tiles as well.

                    // Convert the tilemap range to global coordinates so the camera will be correctly set based on the tilemap.
                    var startPosition = (Vector2I)tileMapLayer.ToGlobal((tileMapRange.Position + correction) * tileMapCellSize);
                    var endPosition = (Vector2I)tileMapLayer.ToGlobal((tileMapRange.End + correction) * tileMapCellSize);

                    if (start == Vector2I.Zero && end == Vector2I.Zero)
                    {
                        start = startPosition;
                        end = endPosition;
                    }
                    else
                    {
                        start.X = Math.Min(start.X, startPosition.X);
                        start.Y = Math.Min(start.Y, startPosition.Y);
                        end.X = Math.Max(end.X, endPosition.X);
                        end.Y = Math.Max(end.Y, endPosition.Y);
                    }
                }
            }

            this.LimitLeft = start.X;
            this.LimitRight = end.X;
            this.LimitTop = start.Y;
            this.LimitBottom = end.Y;
            Debug.Print("Camera ready - Limits: " + this.LimitLeft + ", " + this.LimitRight + ", " + this.LimitTop + ", " + this.LimitBottom);
            CameraRect = new Rect2(start, end - start);
        }
    }
}
