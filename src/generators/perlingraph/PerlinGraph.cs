

using System;
using System.Runtime.CompilerServices;
using Godot;
using Godot.NativeInterop;

/*
The Basic Idea:
- Generate a field of perlin noise
- posterize the noise to get chunks
- use edge detection to draw the lines between those chunks
*/
public class PerlinGraph : Generator, IGenerator
{
  private FastNoiseLite NoiseGenerator = new FastNoiseLite();
  private float[,] Noise; 
  private int[,] PosterizedNoise;
  private int[,] Edges; 
  private int[,] Biomes;
  private string SampleState = "noise";

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

    Noise = new float[Dimensions.X, Dimensions.Y];
    PosterizedNoise = new int[Dimensions.X, Dimensions.Y];
    Edges = new int[Dimensions.X, Dimensions.Y];
    Biomes = new int[Dimensions.X, Dimensions.Y];

    CreateNoise();
    CreateEdges();
    CreateBiomes();
  }

  public override int Sample(Vector2I coordinates)
  {
    return SampleState switch {
      "edges" => Edges[coordinates.X, coordinates.Y],
      "noise" => Mathf.FloorToInt(Noise[coordinates.X, coordinates.Y] * NTypes),
      "posterized" => PosterizedNoise[coordinates.X, coordinates.Y],
      "biomes" => Biomes[coordinates.X, coordinates.Y],
      _ => Mathf.FloorToInt(Noise[coordinates.X, coordinates.Y] * NTypes),
    };
  }

  public override void SetSampler(string state)
  {
    SampleState = state;
  }

  public static float Sigmoid(double value)
  {
    return 1.0f / (1.0f + (float) Math.Exp(-6 * (value - 0.5)));
  }

  public void CreateNoise()
  {
    
    for (int i = 0; i < Dimensions.X; i++)
    {
      for (int j = 0; j < Dimensions.Y; j++)
      {
        // var noise = (NoiseGenerator.GetNoise2D(i * Step, j * Step) + 1 ) / 2.0f;
        // var posterized = noise > .5 ? 1 : 0;
        var noise = Math.Abs(NoiseGenerator.GetNoise2D(i * Step, j * Step));
        var posterized = noise > .1 ? 0 : 1;
        Noise[i, j] = noise;
        PosterizedNoise[i, j] = posterized;
        // Biomes[i, j] = posterized;
      }
    }
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
            var noisex = (int)Mathf.Clamp(cellx + (kernelx - 1), 0, Dimensions.X-1);
            var noisey = (int)Mathf.Clamp(celly + (kernely - 1), 0, Dimensions.Y-1);
            // var yoffset = kernely - 1;
            var noiseval = PosterizedNoise[noisex, noisey];
            sum += noiseval * EdgeKernel[kernelx, kernely];
          }
        }
        Edges[cellx, celly] = sum >= .05 ? 1 : 0;
        Biomes[cellx, celly] = Edges[cellx, celly];
      }
    }
  }

  private void FloodFill(int x, int y, int prevVal, int newVal)
  {
    if (Biomes[x, y] != prevVal) { return; }
    Biomes[x, y] = newVal;
    FloodFill(Mathf.Max(x-1, 0), y, prevVal, newVal);
    FloodFill(Mathf.Min(x+1, Dimensions.X-1), y, prevVal, newVal);
    FloodFill(x, Mathf.Max(y-1, 0), prevVal, newVal);
    FloodFill(x, Mathf.Min(y+1, Dimensions.Y-1), prevVal, newVal);
  }

  private void CreateBiomes()
  {
    for (int i = 0; i < Dimensions.X; i++)
    {
      for (int j = 0; j < Dimensions.Y; j++)
      {
        if (Biomes[i, j] == 0)
        {
          var biome = RNG.RandiRange(2, NTypes-1);
          FloodFill(i, j, 0, biome);
        }
      }
    }
  }
}