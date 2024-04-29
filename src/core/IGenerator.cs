using Godot;

public interface IGenerator
{
    void Generate(int ntypes, Vector2I dimensions);
    int Sample(Vector2I coordinates);
}