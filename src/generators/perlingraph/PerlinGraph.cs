

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using Godot.NativeInterop;


public class BiomeNode
{
  public int Biome { get; set; }
  public List<BiomeNode> Adjacencies { get; set; } = new();
  public List<Cell> Cells { get; set; } = new();
  public Vector2I Center 
  {
    get 
    {
      return Cells.Aggregate(Vector2I.Zero, (sum, next) => sum + next.Coordinates) / Cells.Count;
    }
  }

  public int Size { get => Cells.Count; }
} 

public class Cell 
{
  public Vector2I Coordinates;
  public float NoiseVal;
  public int PosterizedVal;
  public int EdgeVal;
  public int BiomeVal;
  public BiomeNode BNode = null;

}

/*
The Basic Idea:
- Generate a field of perlin noise
- posterize the noise to get chunks
- use edge detection to draw the lines between those chunks
*/
public class PerlinGraph : Generator, IGenerator
{
  private FastNoiseLite NoiseGenerator = new FastNoiseLite();
  public Cell[,] Cells;
  private float[,] Noise; 
  private int[,] PosterizedNoise;
  private int[,] Edges; 
  private int[,] Biomes;
  private BiomeNode[,] BiomeNodes;
  public List<BiomeNode> UniqueBNodes = new();
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

    Cells = new Cell[Dimensions.X, Dimensions.Y];
    for (int i = 0; i < Dimensions.X; i++)
    {
      for (int j = 0; j < Dimensions.Y; j++)
      {
        Cells[i, j] = new Cell()
        {
          Coordinates = new Vector2I(i, j)
        };
      }
    }
    // Noise = new float[Dimensions.X, Dimensions.Y];
    // PosterizedNoise = new int[Dimensions.X, Dimensions.Y];
    // Edges = new int[Dimensions.X, Dimensions.Y];
    // Biomes = new int[Dimensions.X, Dimensions.Y];
    // BiomeNodes = new BiomeNode[Dimensions.X, Dimensions.Y];

    CreateNoise();
    CreateEdges();
    CreateBiomes();
  }

  public override int Sample(Vector2I coordinates)
  {
    // return SampleState switch {
    //   "edges" => Edges[coordinates.X, coordinates.Y],
    //   "noise" => Mathf.FloorToInt(Noise[coordinates.X, coordinates.Y] * NTypes),
    //   "posterized" => PosterizedNoise[coordinates.X, coordinates.Y],
    //   "biomes" => Biomes[coordinates.X, coordinates.Y],
    //   _ => Mathf.FloorToInt(Noise[coordinates.X, coordinates.Y] * NTypes),
    // };
    var cell = Cells[coordinates.X, coordinates.Y];
    return SampleState switch {
      "noise" => Mathf.FloorToInt(cell.NoiseVal * NTypes),
      "posterized" => cell.PosterizedVal,
      "edges" => cell.EdgeVal,
      "biomes" => cell.BiomeVal,
      _ => Mathf.FloorToInt(cell.NoiseVal * NTypes)
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
        var cell = Cells[i, j];
        cell.NoiseVal = noise;
        cell.PosterizedVal = posterized;
        // Noise[i, j] = noise;
        // PosterizedNoise[i, j] = posterized;
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
            // var noiseval = PosterizedNoise[noisex, noisey];
            var noiseval = Cells[noisex, noisey].PosterizedVal;
            sum += noiseval * EdgeKernel[kernelx, kernely];
          }
        }
        // Edges[cellx, celly] = sum >= .05 ? 1 : 0;
        // Biomes[cellx, celly] = Edges[cellx, celly];
        var cell = Cells[cellx, celly];
        cell.EdgeVal = sum >= .05 ? 1 : 0;
        cell.BiomeVal = cell.EdgeVal;
      }
    }
  }

  private void FloodFill(int x, int y, int prevVal, int newVal, BiomeNode node)
  {
    var cell = Cells[x, y];
    // if (Biomes[x, y] != prevVal)
    if (cell.BiomeVal != prevVal) 
    { 
      // check for adjacent biomes
      for (int i = Mathf.Max(x-1, 0); i <= Mathf.Min(x+1, Dimensions.X-1); i++)
      {
        for (int j = Mathf.Max(y-1, 0); j <= Mathf.Min(y+1, Dimensions.Y-1); j++)
        {
          if (i < 0 || i >= Dimensions.X || j < 0 || j >= Dimensions.Y) {
            GD.Print($"Cell out of bounds. Dimensions: {Dimensions}, Coords: {i},{j}");
          }
          // var neighbor = BiomeNodes[i, j];
          var neighbor = Cells[i, j].BNode;
          if (neighbor != node && neighbor != null) 
          {
            if (!node.Adjacencies.Contains(neighbor)) { node.Adjacencies.Add(neighbor); }
            if (!neighbor.Adjacencies.Contains(neighbor)) { neighbor.Adjacencies.Add(node); }
          } 
        }
      }
      return; 
    }

    // Biomes[x, y] = node.Biome;
    // BiomeNodes[x, y] = node;
    // node.Cells.Add(new Vector2I(x, y));
    cell.BiomeVal = node.Biome;
    cell.BNode = node;
    node.Cells.Add(Cells[x, y]);

    FloodFill(Mathf.Max(x-1, 0), y, prevVal, newVal, node);
    FloodFill(Mathf.Min(x+1, Dimensions.X-1), y, prevVal, newVal, node);
    FloodFill(x, Mathf.Max(y-1, 0), prevVal, newVal, node);
    FloodFill(x, Mathf.Min(y+1, Dimensions.Y-1), prevVal, newVal, node);
  }

  private void CreateBiomes()
  {
    for (int i = 0; i < Dimensions.X; i++)
    {
      for (int j = 0; j < Dimensions.Y; j++)
      {
        // if (Biomes[i, j] == 0)
        var cell = Cells[i, j];
        if (cell.BiomeVal == 0)
        {
          var biome = RNG.RandiRange(2, NTypes-1);
          var node = new BiomeNode() 
          {
            Biome = biome,
            Cells = new List<Cell> { Cells[i, j] }
          };
          FloodFill(i, j, 0, 1, node);
          UniqueBNodes.Add(node);
        }
      }
    }
  }

}