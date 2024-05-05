using Godot;
using System;

public partial class PerlinGraphController : BaseController
{
	private PerlinGraph PG = new PerlinGraph();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		generator = new PerlinGraph();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
}
