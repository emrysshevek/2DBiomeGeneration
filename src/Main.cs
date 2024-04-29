using Godot;

public partial class Main : Node2D
{

	[Export]
	public Camera2D Camera;
	[Export]
	public Terrain TerrainMap;

	public float MinZoom = 0.2f;
	public float MaxZoom = 10f;
	public float ZoomFactor = .1f;
	public float ZoomDuration = .2f;
	public float ZoomLevel = 1f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FitToScreen();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);
		if (@event.IsActionPressed("ZoomIn"))
		{
			SetZoomLevel(ZoomLevel * (1 - ZoomFactor));
		}
		else if (@event.IsActionPressed("ZoomOut"))
		{
			SetZoomLevel(ZoomLevel * (1 + ZoomFactor));
		}
	}

	public void SetZoomLevel(float val)
	{
		ZoomLevel = Mathf.Clamp(val, MinZoom, MaxZoom);
		Camera.Zoom = new Vector2(ZoomLevel, ZoomLevel);
	}

	public void FitToScreen()
	{
		var cellSize = TerrainMap.TileSet.TileSize.X;
		var size = TerrainMap.Dimensions * cellSize;
		var center = new Vector2(size.X / 2f, size.Y / 2f);

		var yzoom = GetViewportRect().Size.Y / size.Y;
		var xzoom = GetViewportRect().Size.X / size.X;
		ZoomLevel = Mathf.Min(yzoom, xzoom);

		// Zoom camera to fit entire terrain on screen
		Camera.Zoom = new Vector2(ZoomLevel, ZoomLevel);
		Camera.Position = center;
	}

	public void _OnRegenerateButtonPressed()
	{
		TerrainMap.GenerateMap();
	}
}
