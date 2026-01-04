namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.UMLGenerator;
using Godot;

public interface IApp : ICanvasLayer, IProvide<IAppRepo>;

[Meta(typeof(IAutoNode))]
[ClassDiagram(UseVSCodePaths = true)]
public partial class App : CanvasLayer, IApp
{
  public override void _Notification(int what) => this.Notify(what);

  #region Constants

  public const string GAME_SCENE_PATH = "res://src/game/Game.tscn";

  #endregion Constants

  #region External

  public IGame Game { get; set; } = default!;
  public IInstantiator Instantiator { get; set; } = default!;

  #endregion External

  #region Provisions

  IAppRepo IProvide<IAppRepo>.Value() => AppRepo;

  #endregion Provisions

  #region State

  public IAppRepo AppRepo { get; set; } = default!;
  public IAppLogic AppLogic { get; set; } = default!;

  public AppLogic.IBinding AppBinding { get; set; } = default!;

  #endregion State

  #region Nodes

  [Node] public IMenu Menu { get; set; } = default!;
  [Node] public ISettingsMenu SettingsMenu { get; set; } = default!;
  [Node] public ISubViewport GamePreview { get; set; } = default!;
  [Node] public IColorRect BlankScreen { get; set; } = default!;
  [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;
  [Node] public ISplash Splash { get; set; } = default!;

  #endregion Nodes

  public void Initialize()
  {
    Instantiator = new Instantiator(GetTree());
    AppRepo = new AppRepo();
    AppLogic = new AppLogic();
    AppLogic.Set(AppRepo);
    AppLogic.Set(new AppLogic.Data());

    // Listen for various menu signals. Each of these just translate to an input
    // for the overall app's state machine.
    Menu.NewGame += OnNewGame;
    Menu.LoadGame += OnLoadGame;

    AnimationPlayer.AnimationFinished += OnAnimationFinished;

    Menu.Settings += OnSettings;
    SettingsMenu.ExitSettingsMenu += OnMainMenu;
    AppRepo.AppliedDisplaySettings += OnAppliedDisplaySettings;

    this.Provide();
  }

  public void OnReady()
  {
    AppBinding = AppLogic.Bind();

    AppBinding
      .Handle((in AppLogic.Output.ShowSplashScreen _) =>
      {
        HideMenus();
        BlankScreen.Hide();
        Splash.Show();
      })
      .Handle((in AppLogic.Output.HideSplashScreen _) =>
      {
        BlankScreen.Show();
        FadeToBlack();
      })
      .Handle((in AppLogic.Output.RemoveExistingGame _) =>
      {
        GamePreview.RemoveChildEx(Game);
        Game.QueueFree();
        Game = default!;
      })
      .Handle((in AppLogic.Output.SetupGameScene _) =>
      {
        if (Game == null)
        {
          Game = Instantiator.LoadAndInstantiate<Game>(GAME_SCENE_PATH);
          GamePreview.AddChildEx(Game);
        }

        Instantiator.SceneTree.Paused = false;
      })
      .Handle((in AppLogic.Output.ShowMainMenu _) =>
      {
        // Load everything while we're showing a black screen, then fade in.
        HideMenus();
        Menu.NewGameButton.GrabFocus();
        Menu.Show();
        Game.Show();

        FadeInFromBlack();
      })
      .Handle((in AppLogic.Output.FadeToBlack _) => FadeToBlack())
      .Handle((in AppLogic.Output.ShowGame _) =>
      {
        HideMenus();
        FadeInFromBlack();
      })
      .Handle((in AppLogic.Output.HideGame _) => FadeToBlack())
      .Handle(
        (in AppLogic.Output.StartLoadingSaveFile _) =>
        {
          Game.SaveFileLoaded += OnSaveFileLoaded;
          Game.LoadExistingGame();
        }
      )
      .Handle((in AppLogic.Output.ShowSettingsMenu _) =>
      {
        SettingsMenu.LoadDefaultSettings();
        SettingsMenu.Show();
      })
      .Handle((in AppLogic.Output.HideSettingsMenu _) => SettingsMenu.Hide())
      .Handle((in AppLogic.Output.HideMainMenu _) => HideMenus())
      .Handle((in AppLogic.Output.LoadDisplaySettings _) =>
      {
        AppRepo.ApplyDisplaySettings(
          AppRepo.GetSavedDisplaySettings()
        );
      });

    // Enter the first state to kick off the binding side effects.
    AppLogic.Start();
  }

  public void OnNewGame() => AppLogic.Input(new AppLogic.Input.NewGame());

  public void OnLoadGame() => AppLogic.Input(new AppLogic.Input.LoadGame());

  public void OnSettings() => AppLogic.Input(new AppLogic.Input.Settings());

  public void OnMainMenu() => AppLogic.Input(new AppLogic.Input.MainMenu());

  public void OnAnimationFinished(StringName animation)
  {
    // There's only two animations :)
    // We don't care what state we're in — we just tell the current state what's
    // happened and it will do the right thing.

    if (animation == "fade_in")
    {
      AppLogic.Input(new AppLogic.Input.FadeInFinished());
      BlankScreen.Hide();
      return;
    }

    AppLogic.Input(new AppLogic.Input.FadeOutFinished());
  }

  public void FadeInFromBlack()
  {
    BlankScreen.Show();
    AnimationPlayer.Play("fade_in");
  }

  public void FadeToBlack()
  {
    BlankScreen.Show();
    AnimationPlayer.Play("fade_out");
  }

  public void HideMenus()
  {
    Splash.Hide();
    Menu.Hide();
  }

  public void OnExitTree()
  {
    // Cleanup things we own.
    AppLogic.Stop();
    AppBinding.Dispose();
    AppRepo.Dispose();

    Menu.NewGame -= OnNewGame;
    Menu.LoadGame -= OnLoadGame;
    Menu.Settings -= OnSettings;
    AppRepo.AppliedDisplaySettings -= OnAppliedDisplaySettings;
    AnimationPlayer.AnimationFinished -= OnAnimationFinished;
  }

  public void OnSaveFileLoaded()
  {
    Game.SaveFileLoaded -= OnSaveFileLoaded;
    AppLogic.Input(new AppLogic.Input.SaveFileLoaded());
  }

  public void OnAppliedDisplaySettings(DisplaySettings settings)
  {
    GetTree().Root.Mode = settings.DisplayMode;
    DisplayServer.WindowSetVsyncMode(settings.VSyncMode);
    Engine.MaxFps = settings.MaxFPS;

    switch (settings.Scaling3DScale)
    {
      case Scaling3DScale.UltraPerformance:
        GetTree().Root.Scaling3DScale = 1.0f / 3.0f;
        break;
      case Scaling3DScale.Performance:
        GetTree().Root.Scaling3DScale = 1.0f / 2.0f;
        break;
      case Scaling3DScale.Balanced:
        GetTree().Root.Scaling3DScale = 1.0f / 1.7f;
        break;
      case Scaling3DScale.Quality:
        GetTree().Root.Scaling3DScale = 1.0f / 1.5f;
        break;
      case Scaling3DScale.UltraQuality:
        GetTree().Root.Scaling3DScale = 1.0f / 1.3f;
        break;
      case Scaling3DScale.Native:
        GetTree().Root.Scaling3DScale = 1.0f;
        break;
    }

    GetTree().Root.UseTaa = settings.Taa;
    GetTree().Root.Scaling3DMode = settings.Scaling3DMode;

    GetTree().Root.Msaa3D = settings.Msaa;
    GetTree().Root.ScreenSpaceAA = settings.Ssaa;
    GetTree().Root.PropagateCall("set", ["shadow_enabled", settings.Shadows]);

    switch (settings.ScreenSpaceAOQuality)
    {
      case SSAOQuality.MEDIUM:
        RenderingServer.EnvironmentSetSsaoQuality(RenderingServer.EnvironmentSsaoQuality.Medium, false, 0.5f, 2, 50, 300);
        break;
      case SSAOQuality.HIGH:
        RenderingServer.EnvironmentSetSsaoQuality(RenderingServer.EnvironmentSsaoQuality.High, false, 0.5f, 2, 50, 300);
        break;
    }

    switch (settings.ScreenSpaceILQuality)
    {
      case SSILQuality.MEDIUM:
        RenderingServer.EnvironmentSetSsilQuality(RenderingServer.EnvironmentSsilQuality.Medium, false, 0.5f, 2, 50, 300);
        break;
      case SSILQuality.HIGH:
        RenderingServer.EnvironmentSetSsilQuality(RenderingServer.EnvironmentSsilQuality.High, false, 0.5f, 2, 50, 300);
        break;
    }


  }

}
