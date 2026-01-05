namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class AppLogic
{
  public partial record State
  {
    [Meta]
    public partial record MainMenu : State,
    IGet<Input.NewGame>, IGet<Input.LoadGame>, IGet<Input.Settings>
    {
      public MainMenu()
      {
        this.OnEnter(
          () =>
          {
            Get<Data>().ShouldLoadExistingGame = false;

            Get<IAppRepo>().OnMainMenuEntered();

            Output(new Output.ShowMainMenu());
          }
        );
      }

      public Transition On(in Input.NewGame input) => To<LeavingMenu>();

      public Transition On(in Input.Settings input) => To<SettingsMenu>();

      public Transition On(in Input.LoadGame input)
      {
        Get<Data>().ShouldLoadExistingGame = true;

        return To<LeavingMenu>();
      }
    }
  }
}
