namespace GameDemo;

using Chickensoft.Introspection;

public partial class Player2DLogic
{
  public partial record State
  {
    [Meta, Id("player_2d_)logic_state_alive")]
    public partial record Alive : State,
      IGet<Input.PhysicsTick>
    {
      public virtual Transition On(in Input.PhysicsTick input)
      {
        var delta = input.Delta;
        var player = Get<IPlayer2D>();
        var settings = Get<Settings>();

        var moveDirection = player.GetGlobalInputVector();

        var velocity = moveDirection * settings.MoveSpeed;

        Output(
          new Output.MovementComputed(velocity, moveDirection, delta)
        );
        return ToSelf();
      }
    }
  }
}
