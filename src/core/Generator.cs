using Godot;

public abstract class Generator : IGenerator
{
  public int NTypes { get; set; }
  public Vector2I Dimensions { get; set; }

  public virtual void Generate(int ntypes, Vector2I dimensions)
  {
    NTypes = ntypes;
    Dimensions = dimensions;
  }

  public abstract int Sample(Vector2I coordinates);
}