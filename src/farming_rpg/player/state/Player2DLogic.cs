namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IPlayer2DLogic : ILogicBlock<Player2DLogic.State>;

[Meta, Id("player_2d_logic")]
[LogicBlock(typeof(State), Diagram = true)]
public partial class Player2DLogic : LogicBlock<Player2DLogic.State>, IPlayer2DLogic
{
  public override Transition GetInitialState() => To<State.Alive>();
}
