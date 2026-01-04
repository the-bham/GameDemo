namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public partial record State
  {
    [Meta]
    public partial record SettingsMenu : State,
    IGet<Input.MainMenu>
    {
      public SettingsMenu()
      {
        this.OnEnter(
          () =>
          {
            Output(new Output.HideMainMenu());
            Output(new Output.ShowSettingsMenu());
          }
        );

        this.OnExit(() => Output(new Output.HideSettingsMenu()));
      }
      public Transition On(in Input.MainMenu input) => To<MainMenu>();
    }
  }
}
