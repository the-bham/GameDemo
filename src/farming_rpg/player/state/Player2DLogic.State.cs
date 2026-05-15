namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class Player2DLogic
{
  [Meta]
  public abstract partial record State : StateLogic<State>;
}
