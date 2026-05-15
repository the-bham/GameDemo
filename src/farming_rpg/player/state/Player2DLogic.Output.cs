namespace GameDemo;

using Godot;

public partial class Player2DLogic
{
  public static class Output
  {
    public readonly record struct MovementComputed(Vector2 Velocity,
      Vector2 Direction, double Delta);

    public readonly record struct VelocityChanged(Vector2 Velocity);

    public readonly record struct Animate(string AnimationName);
  }
}
