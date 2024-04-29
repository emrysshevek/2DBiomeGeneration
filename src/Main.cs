using Godot;

public partial class Main : Node2D
{
	[Export]
	public Camera2D Camera;
	[Export]
	public Terrain TerrainMap;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FitToScreen();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void FitToScreen()
	{
		var cellSize = TerrainMap.TileSet.TileSize;
		var size = TerrainMap.Dimensions * cellSize;
		var center = new Vector2(size.X / 2f, size.Y / 2f);

		var yzoom = GetViewportRect().Size.Y / size.Y;
		var xzoom = GetViewportRect().Size.X / size.X;
		var zoom = Mathf.Min(yzoom, xzoom);

		// Zoom camera to fit entire terrain on screen
		Camera.Zoom = new Vector2(zoom, zoom);
		Camera.Position = center;
	}
}
