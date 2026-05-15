namespace GameDemo;

using Chickensoft.Introspection;
using Godot;

public partial class Player2DLogic
{
  public partial record State
  {
    [Meta, Id("player_2d_)logic_state_alive")]
    public partial record Alive : State,
      IGet<Input.PhysicsTick>,
      IGet<Input.Animate>
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

      public virtual Transition On(in Input.Animate input)
      {
        var player = Get<IPlayer2D>();

        var state = "idle";

        if (player.Velocity.Length() > 0)
        {
          state = "walk";
        }

        var moveDirection = input.Direction;

        var direction = "down";

        if (Mathf.Abs(moveDirection.X) > Mathf.Abs(moveDirection.Y))
        {
          if (moveDirection.X > 0)
          {
            direction = "right";
          }
          else
          {
            direction = "left";
          }
        }
        else
        {

          if (moveDirection.Y > 0)
          {
            direction = "down";
          }
          else
          {
            direction = "up";
          }
        }

        Output(new Output.Animate(state + "_" + direction));

        return ToSelf();
      }
    }
  }
}
