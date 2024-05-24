using Godot;
using System;

public partial class PerlinGraphController : BaseController
{
	private PerlinGraph PG = new PerlinGraph();
	private bool ShowGraph = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		generator = new PerlinGraph();
	}

	public override void _Draw()
	{
		base._Draw();
		if (ShowGraph) 
		{
			foreach(var bnode in ((PerlinGraph)generator).UniqueBNodes)
			{
				var pos = Map.MapToLocal(bnode.Center);
				GD.Print($"Node position: {pos}");
				DrawCircle(bnode.Center, 10, Color.Color8(0,0,0));
			}
		}
	}

	public void _OnNoiseButtonPressed()
	{
		generator.SetSampler("noise");
		EmitSignal(SignalName.Resample);
	}

	public void _OnEdgesButtonPressed()
	{
		generator.SetSampler("edges");
		EmitSignal(SignalName.Resample);
	}

	public void _OnPosterizedPressed()
	{
		generator.SetSampler("posterized");
		EmitSignal(SignalName.Resample);
	}

	public void _OnBiomesButtonPressed()
	{
		generator.SetSampler("biomes");
		EmitSignal(SignalName.Resample);
	}

	public void _OnGraphButtonPressed()
	{
		ShowGraph = !ShowGraph;
		QueueRedraw();
	}
}
