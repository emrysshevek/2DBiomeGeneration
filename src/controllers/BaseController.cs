using Godot;
using System;

public partial class BaseController : Control, IGenerator
{
	[Signal]
	public delegate void ResampleEventHandler();
	[Export]
	public TileMap Map;
	protected Generator generator;

  public virtual void Generate(int ntypes, Vector2I dimensions)
	{
		generator.Generate(ntypes, dimensions);
	}

	public virtual int Sample(Vector2I coordinates)
	{
		return generator.Sample(coordinates);
	}

	public virtual void SetSampler(string state)
	{
		generator.SetSampler(state);
	}

}
