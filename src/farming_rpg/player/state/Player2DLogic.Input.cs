namespace GameDemo;

using Godot;

public partial class Player2DLogic
{
  public static class Input
  {
    public readonly record struct Moved(Vector2 GlobalPosition);

    public readonly record struct PhysicsTick(double Delta);

    public readonly record struct Animate(Vector2 Direction);
  }
}
