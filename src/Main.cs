using Godot;

public partial class Main : Node2D
{

	[Export]
	public TerrainCamera Camera;
	[Export]
	public Terrain TerrainMap;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Camera.FitToScreen();
	}

	

	public void _OnRegenerateButtonPressed()
	{
		TerrainMap.GenerateMap();
	}
}
