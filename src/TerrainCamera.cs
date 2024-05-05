using Godot;
using System;

public partial class TerrainCamera : Camera2D
{
	[Export]
	public Terrain TerrainMap;

	public float MinZoom = 0.2f;
	public float MaxZoom = 10f;
	public float ZoomFactor = .1f;
	public float ZoomDuration = .2f;
	public float ZoomLevel = 1f;
	private Vector2 MousePosition = Vector2.Inf;
	private bool MouseIsPressed = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
      base._Input(@event);
			if (@event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.IsPressed()) 
			{ 
				MouseIsPressed = true;
				MousePosition = eventMouseButton.Position; 
			}
			else { MouseIsPressed = false; }
		} 
    else if (@event is InputEventMouseMotion eventMouseMotion)
		{
			if (MouseIsPressed)
			{
				var newPos = eventMouseMotion.Position;
				Position += (MousePosition - newPos) / Zoom;
				MousePosition = newPos;
			}
		}
				
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
		Zoom = new Vector2(ZoomLevel, ZoomLevel);
	}

	public void FitToScreen()
	{
		var cellSize = TerrainMap.TileSet.TileSize.X;
		var size = TerrainMap.Dimensions * cellSize;
		var center = new Vector2(size.X / 2f, size.Y / 2f);

		// var yzoom = GetViewportRect().Size.Y / size.Y;
		// var xzoom = GetViewportRect().Size.X / size.X;
		// ZoomLevel = Mathf.Min(yzoom, xzoom);

		// // Zoom camera to fit entire terrain on screen
		// Camera.Zoom = new Vector2(ZoomLevel, ZoomLevel);
		Position = center;
	}
}
