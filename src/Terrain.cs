using Godot;
using System;

public partial class Terrain : TileMap
{
	[Export]
	public Vector2I Dimensions = new(100, 100);
	[Export]
	int NTypes = 6;

	public Generator Generator = new RandomGenerator();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
