namespace GameDemo;

using System;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Chickensoft.Serialization.Godot;
using Chickensoft.UMLGenerator;
using Godot;

/// <summary>
///   Pure application game logic repository shared between view-specific logic
///   blocks.
/// </summary>
public interface IAppRepo : IDisposable
{
  /// <summary>
  ///   Event invoked when the game is about to start.
  /// </summary>
  event Action? GameEntered;

  /// <summary>
  ///   Event invoked when the game is about to end.
  /// </summary>
  event Action<PostGameAction>? GameExited;

  /// <summary>Event invoked when the splash screen is skipped.</summary>
  event Action? SplashScreenSkipped;

  /// <summary>Event invoked when the main menu is entered.</summary>
  event Action? MainMenuEntered;

  /// <summary>
  ///   Event invoked when the settings button is pressed
  /// </summary>
  event Action? SettingsMenuEntered;

  /// <summary>Event invoked when the display settings are applied.</summary>
  event Action<DisplaySettings>? AppliedDisplaySettings;

  /// <summary>Inform the app that the game should be shown.</summary>
  void OnEnterGame();

  /// <summary>Inform the app that the game should be exited.</summary>
  /// <param name="action">Action to take following the end of the game.</param>
  void OnExitGame(PostGameAction action);

  /// <summary>Tells the app that the main menu was entered.</summary>
  void OnMainMenuEntered();

  /// <summary>Tells the app that the settings was entered.</summary>
  void OnSettingsMenuEntered();

  /// <summary> Tells the app to apply settings to view ports and environments.</summary>
  void ApplyDisplaySettings(DisplaySettings settings);

  /// <summary> Retrieves display settings from settings file.</summary>
  DisplaySettings GetSavedDisplaySettings();

  /// <summary> Persists passed settings to settings file.</summary>
  void SaveDisplaySettings(DisplaySettings settings);

  /// <summary>Skips the splash screen.</summary>
  void SkipSplashScreen();
}

/// <summary>
///   Pure application game logic repository — shared between view-specific logic
///   blocks.
/// </summary>
public class AppRepo : IAppRepo
{

  #region Settings
  public JsonSerializerOptions JsonOptions { get; set; } = default!;
  public const string SETTINGS_FILE_NAME = "settings.json";

  public IFileSystem FileSystem { get; set; } = default!;
  public string SettingsFilePath { get; set; } = default!;

  #endregion Settings
  public event Action? SplashScreenSkipped;
  public event Action? MainMenuEntered;
  public event Action? GameEntered;
  public event Action<PostGameAction>? GameExited;

  public event Action? SettingsMenuEntered;

  public event Action<DisplaySettings>? AppliedDisplaySettings;

  private bool _disposedValue;

  public void SkipSplashScreen() => SplashScreenSkipped?.Invoke();

  public void OnMainMenuEntered() => MainMenuEntered?.Invoke();

  public void OnEnterGame() => GameEntered?.Invoke();
  public void OnExitGame(PostGameAction action) => GameExited?.Invoke(action);
  public void OnSettingsMenuEntered() => SettingsMenuEntered?.Invoke();

  public DisplaySettings GetSavedDisplaySettings()
  {
    //retrieve from file
    FileSystem = new FileSystem();

    SettingsFilePath = FileSystem.Path.Join(OS.GetUserDataDir(), SETTINGS_FILE_NAME);

    if (!FileSystem.File.Exists(SettingsFilePath))
    {
      GD.Print("No settings file to load :'(");
      return new DisplaySettings()
      {
        DisplayMode = Window.ModeEnum.Windowed,
        VSyncMode = DisplayServer.VSyncMode.Disabled,
        MaxFPS = 60,
        Scaling3DMode = Viewport.Scaling3DModeEnum.Bilinear,
        Msaa = Viewport.Msaa.Msaa2X,
        Scaling3DScale = Scaling3DScale.Balanced,
        Taa = true,
        Ssaa = Viewport.ScreenSpaceAAEnum.Fxaa,
        Shadows = true,
        GlobalIlluminationType = GIType.LIGHTMAP_GI,
        GlobalIlluminationQuality = GIQuality.LOW,
        ScreenSpaceAOQuality = SSAOQuality.MEDIUM,
        ScreenSpaceILQuality = SSILQuality.MEDIUM,
        Bloom = true,
        VolumetricFog = true,
      };
    }

    var upgradeDependencies = new Blackboard();
    var resolver = JsonTypeInfoResolver.Combine(
      DisplaySettingsEnumContext.Default,

      new SerializableTypeResolver()
    );
    JsonOptions = new JsonSerializerOptions
    {
      Converters = {
        new SerializableTypeConverter(upgradeDependencies),
         new JsonStringEnumConverter<Window.ModeEnum>(),
         // Chickensoft type converter
        new SerializableTypeConverter()
      },
      TypeInfoResolver = resolver,
      WriteIndented = true
    };
    var json = FileSystem.File.ReadAllText(SettingsFilePath);
    DisplaySettings? settings = JsonSerializer.Deserialize<DisplaySettings>(json, JsonOptions);

    if (settings == null)
    {
      settings = new DisplaySettings()
      {
        DisplayMode = Window.ModeEnum.Windowed,
        VSyncMode = DisplayServer.VSyncMode.Disabled,
        MaxFPS = 60,
        Scaling3DMode = Viewport.Scaling3DModeEnum.Bilinear,
        Msaa = Viewport.Msaa.Msaa2X,
        Scaling3DScale = Scaling3DScale.Balanced,
        Taa = true,
        Ssaa = Viewport.ScreenSpaceAAEnum.Fxaa,
        Shadows = true,
        GlobalIlluminationType = GIType.LIGHTMAP_GI,
        GlobalIlluminationQuality = GIQuality.LOW,
        ScreenSpaceAOQuality = SSAOQuality.MEDIUM,
        ScreenSpaceILQuality = SSILQuality.MEDIUM,
        Bloom = true,
        VolumetricFog = true,
      };
    }

    return settings;
  }

  public void SaveDisplaySettings(DisplaySettings settings)
  {
    //save settings to file
    //retrieve from file
    FileSystem = new FileSystem();

    SettingsFilePath = FileSystem.Path.Join(OS.GetUserDataDir(), SETTINGS_FILE_NAME);

    var resolver = JsonTypeInfoResolver.Combine(
      DisplaySettingsEnumContext.Default,

      new SerializableTypeResolver()
    );

    // Tell our type type resolver about the Godot-specific converters.
    GodotSerialization.Setup();

    var upgradeDependencies = new Blackboard();
    JsonOptions = new JsonSerializerOptions
    {
      Converters = {
        new SerializableTypeConverter(upgradeDependencies),
         new JsonStringEnumConverter<Window.ModeEnum>(),
         // Chickensoft type converter
        new SerializableTypeConverter()
      },
      TypeInfoResolver = resolver,
      WriteIndented = true
    };

    var json = JsonSerializer.Serialize(settings, JsonOptions);
    FileSystem.File.WriteAllTextAsync(SettingsFilePath, json);
  }

  public void ApplyDisplaySettings(DisplaySettings settings)
  {
    AppliedDisplaySettings?.Invoke(settings);
  }


  #region Internals

  protected void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
      {
        // Dispose managed objects.
        SplashScreenSkipped = null;
        MainMenuEntered = null;
        GameEntered = null;
        GameExited = null;
      }

      _disposedValue = true;
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  #endregion Internals
}
