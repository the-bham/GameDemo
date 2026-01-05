namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Text.Json;
using Chickensoft.Collections;
using Chickensoft.GoDotTest;
using Chickensoft.Serialization;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is disposed in last test"
  )
]
public class AppRepoTest : TestClass
{
  private AppRepo _repo = default!;
  private Mock<IFileSystem> _fileSystem = default!;
  private JsonSerializerOptions _jsonOptions = default!;

  public string SETTINGS_FILE_PATH = "./ settings.json";

  public AppRepoTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _fileSystem = new();
    _jsonOptions = new()
    {
      WriteIndented = true,
      TypeInfoResolver = new SerializableTypeResolver(),
      Converters = {
        new SerializableTypeConverter(new Blackboard())
      },
    };

    _repo = new()
    {
      FileSystem = _fileSystem.Object,
      JsonOptions = _jsonOptions,
      SettingsFilePath = SETTINGS_FILE_PATH
    };
  }

  [Cleanup]
  public void Cleanup() => _repo.Dispose();

  [Test]
  public void Initializes()
  {
    var repo = new AppRepo();
    repo.ShouldBeAssignableTo<IAppRepo>();
  }

  [Test]
  public void SkipSplashScreen()
  {
    var called = false;

    void splashScreenSkipped() => called = true;

    // invoke event without handlers to cover null check
    _repo.SkipSplashScreen();

    _repo.SplashScreenSkipped += splashScreenSkipped;

    _repo.SkipSplashScreen();

    called.ShouldBe(true);
  }

  [Test]
  public void OnMainMenuEnteredInvokesEvent()
  {
    var called = 0;

    void onMainMenuEntered() => called++;

    _repo.OnMainMenuEntered();
    _repo.MainMenuEntered += onMainMenuEntered;
    _repo.OnMainMenuEntered();

    called.ShouldBe(1);
  }

  [Test]
  public void OnEnterGameInvokesEvent()
  {
    var called = 0;

    void onEnterGame() => called++;

    _repo.OnEnterGame();
    _repo.GameEntered += onEnterGame;
    _repo.OnEnterGame();

    called.ShouldBe(1);
  }

  [Test]
  public void OnSettingsMenuEnteredInvokesEvent()
  {
    var called = 0;

    void onSettingsMenuEntered() => called++;

    _repo.OnSettingsMenuEntered();
    _repo.SettingsMenuEntered += onSettingsMenuEntered;
    _repo.OnSettingsMenuEntered();

    called.ShouldBe(1);
  }

  [Test]
  public void OnExitGameInvokesEventWithPostGameAction()
  {
    var called = 0;
    var action = PostGameAction.GoToMainMenu;

    void onExitGame(PostGameAction a)
    {
      called++;
      a.ShouldBe(action);
    }

    _repo.OnExitGame(action);
    _repo.GameExited += onExitGame;
    _repo.OnExitGame(action);

    called.ShouldBe(1);
  }

  [Test]
  public void ApplyDisplaySettingsWithSettings()
  {
    var called = 0;
    DisplaySettings settings = default!;

    void applyDisplaySettings(DisplaySettings a)
    {
      called++;
      a.ShouldBe(settings);
    }

    _repo.ApplyDisplaySettings(settings);
    _repo.AppliedDisplaySettings += applyDisplaySettings;
    _repo.ApplyDisplaySettings(settings);

    called.ShouldBe(1);
  }

  [Test]
  public void GetSavedDisplaySettings()
  {
    var settings = _repo.GetSavedDisplaySettings();
  }

  [Test]
  public void Disposes()
  {
    Should.NotThrow(_repo.Dispose);
    // Redundant dispose shouldn't do anything.
    Should.NotThrow(_repo.Dispose);
  }
}
