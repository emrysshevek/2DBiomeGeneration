using Godot;
using System;

public partial class Terrain : TileMap
{
	[Export]
	public Vector2I Dimensions = new(100, 100);
	[Export]
	int NTypes = 6;
	[Export]
	public TerrainCamera Camera;
	private Vector2I _dimensions;

	public Generator Generator = new PerlinGraph();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_dimensions = Dimensions;
		GenerateMap();
	}

	public void SetDimensions()
	{
		if (_dimensions.X < 1)
		{
			Dimensions.X = (int)(GetViewportRect().Size.X / Camera.Zoom.X) / TileSet.TileSize.X + 1;
		}
		else { Dimensions.X = _dimensions.X; }
		if (_dimensions.Y < 1) 
		{
			Dimensions.Y = (int)(GetViewportRect().Size.Y / Camera.Zoom.Y) / TileSet.TileSize.Y + 1;
		}
		else { Dimensions.Y = _dimensions.Y; }
	}

	public void GenerateMap()
	{
		SetDimensions();
		Generator.Generate(NTypes, Dimensions);
		for (int i = 0; i < Dimensions.X; i++)
		{
			for (int j = 0; j < Dimensions.Y; j++)
			{
				var coordinates = new Vector2I(i, j);
				int type = Generator.Sample(coordinates);
				SetCell
				(
					layer: 0, 
					coords: coordinates,
					sourceId: 0,
					atlasCoords: new Vector2I(type, 0)
				);
			}
		}
		Camera.FitToScreen();
	}
}
