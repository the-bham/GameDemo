namespace GameDemo;

public partial class Player2DLogic
{
  public static class Input
  {
    public readonly record struct PhysicsTick(double Delta);
  }
}
