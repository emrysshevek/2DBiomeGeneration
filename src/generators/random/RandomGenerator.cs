

using Godot;

public class RandomGenerator : Generator, IGenerator
{
    public override void Generate(int ntypes, Vector2I dimensions)
    {
      base.Generate(ntypes, dimensions);
    }

    public override int Sample(Vector2I coordinates)
    {
      return RNG.RandiRange(0, NTypes-1);
    }
}