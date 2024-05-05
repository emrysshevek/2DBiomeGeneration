using Godot;

public abstract class Generator : IGenerator
{
  protected static Generator Instance = null;
  public int NTypes { get; set; }
  public Vector2I Dimensions { get; set; }
  public RandomNumberGenerator RNG = new();

  public virtual void Generate(int ntypes, Vector2I dimensions)
  {
    NTypes = ntypes;
    Dimensions = dimensions;
    RNG.Randomize();
  }

  public abstract int Sample(Vector2I coordinates);

  public virtual void SetSampler(string state)
  {
    ;;
  }

}