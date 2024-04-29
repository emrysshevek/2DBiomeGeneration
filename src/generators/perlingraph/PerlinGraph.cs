

using Godot;

/*
The Basic Idea:
- Generate a field of perlin noise
- posterize the noise to get chunks
- use edge detection to draw the lines between those chunks
*/
public class PerlinGraph : Generator, IGenerator
{
  private FastNoiseLite NoiseGenerator = new FastNoiseLite();
  private int[,] Cells; 
  private float[,] EdgeKernel = new float[3,3]
  {
    { -1, -1, -1 },
    { -1, 8, -1 },
    { -1, -1, -1}
  };
  private float Step = 3f;
  public override void Generate(int ntypes, Vector2I dimensions)
  {
    base.Generate(ntypes, dimensions);
    NoiseGenerator.Offset = new Vector3(RNG.RandfRange(-1000f, 1000f), RNG.RandfRange(-1000f, 1000f), RNG.RandfRange(-1000f, 1000f));
    Cells = new int[Dimensions.X, Dimensions.Y];

    CreateEdges();
  }

  public override int Sample(Vector2I coordinates)
  {
    return Cells[coordinates.X, coordinates.Y];
  }

  private int GetNoiseValue(int x, int y)
  {
    return NoiseGenerator.GetNoise2D(x * Step, y * Step) > 0 ? 0 : 1;
  }

  private void CreateEdges()
  {
    for (int cellx = 0; cellx < Dimensions.X; cellx++)
    {
      for (int celly = 0; celly < Dimensions.Y; celly++)
      {
        var sum = 0f;
        for (int kernelx = 0; kernelx < 3; kernelx++)
        {
          for (int kernely = 0; kernely < 3; kernely++)
          {
            var xoffset = kernelx - 1;
            var yoffset = kernely - 1;
            var cellval = GetNoiseValue(cellx + xoffset, celly + yoffset);
            sum += cellval * EdgeKernel[kernelx, kernely];
          }
        }
        Cells[cellx, celly] = sum >= 0 ? 0 : 1;
      }
    }
  }
}