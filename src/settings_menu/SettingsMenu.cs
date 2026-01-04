namespace GameDemo;

using System.Threading.Tasks.Dataflow;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

using Godot;
public interface ISettingsMenu : IControl
{
  event SettingsMenu.ExitSettingsMenuEventHandler ExitSettingsMenu;
  event SettingsMenu.ApplyEventHandler Apply;

  void LoadDefaultSettings();
}

[Meta(typeof(IAutoNode))]
public partial class SettingsMenu : Control, ISettingsMenu
{
  public override void _Notification(int what) => this.Notify(what);

  public bool MetalFXSupported;

  #region Dependencies
  [Dependency]
  public IAppRepo AppRepo => this.DependOn<IAppRepo>();
  #endregion Dependencies

  #region Nodes
  [Node]
  public IButton ApplyButton { get; set; } = default!;
  [Node]
  public IButton ExitButton { get; set; } = default!;
  [Node]
  public IHBoxContainer DisplayMode { get; set; } = default!;

  [Node]
  public IButton Windowed { get; set; } = default!;
  [Node]
  public IButton Fullscreen { get; set; } = default!;
  [Node]
  public IButton ExclusiveFullscreen { get; set; } = default!;
  [Node]
  public IHBoxContainer VSync { get; set; } = default!;

  [Node]
  public IButton VSyncDisabled { get; set; } = default!;

  [Node]
  public IButton VSyncEnabled { get; set; } = default!;

  [Node]
  public IButton VSyncAdaptive { get; set; } = default!;
  [Node]
  public IButton VSyncMailbox { get; set; } = default!;
  [Node]
  public IHBoxContainer MaxFPS { get; set; } = default!;
  [Node]
  public IButton FPS30 { get; set; } = default!;
  [Node]
  public IButton FPS40 { get; set; } = default!;
  [Node]
  public IButton FPS60 { get; set; } = default!;
  [Node]
  public IButton FPS72 { get; set; } = default!;
  [Node]
  public IButton FPS90 { get; set; } = default!;
  [Node]
  public IButton FPS120 { get; set; } = default!;
  [Node]
  public IButton FPS144 { get; set; } = default!;
  [Node]
  public IButton FPSUnlimited { get; set; } = default!;
  [Node]
  public IHBoxContainer ResolutionScale { get; set; } = default!;

  [Node]
  public IButton UltraPerformance { get; set; } = default!;

  [Node]
  public IButton Performance { get; set; } = default!;

  [Node]
  public IButton Balanced { get; set; } = default!;

  [Node]
  public IButton Quality { get; set; } = default!;

  [Node]
  public IButton UltraQuality { get; set; } = default!;

  [Node]
  public IButton Native { get; set; } = default!;

  [Node]
  public IHBoxContainer ResolutionScaleFilter { get; set; } = default!;
  [Node]
  public IButton Bilinear { get; set; } = default!;
  [Node]
  public IButton FSR1 { get; set; } = default!;
  [Node]
  public IButton MetalFXSpatial { get; set; } = default!;
  [Node]
  public IButton FSR2 { get; set; } = default!;
  [Node]
  public IButton MetalFXTemporal { get; set; } = default!;
  [Node]
  public IHBoxContainer TAA { get; set; } = default!;
  [Node]
  public IButton TaaDisabled { get; set; } = default!;
  [Node]
  public IButton TaaEnabled { get; set; } = default!;
  [Node]
  public IHBoxContainer MSAA { get; set; } = default!;
  [Node]
  public IButton MsaaDisabled { get; set; } = default!;
  [Node]
  public IButton Msaa2X { get; set; } = default!;
  [Node]
  public IButton Msaa4X { get; set; } = default!;
  [Node]
  public IButton Msaa8X { get; set; } = default!;
  [Node]
  public IHBoxContainer SSAA { get; set; } = default!;
  [Node]
  public IButton SSAADisabled { get; set; } = default!;
  [Node]
  public IButton FXAA { get; set; } = default!;
  [Node]
  public IButton SMAA { get; set; } = default!;
  [Node]
  public IHBoxContainer ShadowMapping { get; set; } = default!;

  [Node]
  public IButton ShadowsDisabled { get; set; } = default!;
  [Node]
  public IButton ShadowsEnabled { get; set; } = default!;
  [Node]
  public IHBoxContainer GlobalIlluminationType { get; set; } = default!;
  [Node]
  public IButton LightmapGI { get; set; } = default!;
  [Node]
  public IButton VoxelGI { get; set; } = default!;
  [Node]
  public IButton SDFGI { get; set; } = default!;
  [Node]
  public IHBoxContainer GlobalIlluminationQuality { get; set; } = default!;
  [Node]
  public IButton GIQualityDisabled { get; set; } = default!;
  [Node]
  public IButton GIQualityLow { get; set; } = default!;
  [Node]
  public IButton GIQualityHigh { get; set; } = default!;
  [Node]
  public IHBoxContainer SSAO { get; set; } = default!;
  [Node]
  public IButton SSAODisabled { get; set; } = default!;
  [Node]
  public IButton SSAOMedium { get; set; } = default!;
  [Node]
  public IButton SSAOHigh { get; set; } = default!;
  [Node]
  public IHBoxContainer SSIL { get; set; } = default!;
  [Node]
  public IButton SSILDisabled { get; set; } = default!;
  [Node]
  public IButton SSILMedium { get; set; } = default!;
  [Node]
  public IButton SSILHigh { get; set; } = default!;
  [Node]
  public IHBoxContainer Bloom { get; set; } = default!;
  [Node]
  public IButton BloomDisabled { get; set; } = default!;
  [Node]
  public IButton BloomEnabled { get; set; } = default!;
  [Node]
  public IHBoxContainer VolumetricFog { get; set; } = default!;
  [Node]
  public IButton VolumetricFogDisabled { get; set; } = default!;
  [Node]
  public IButton VolumetricFogEnabled { get; set; } = default!;
  #endregion Nodes

  #region Signals
  [Signal]
  public delegate void ExitSettingsMenuEventHandler();
  [Signal]
  public delegate void ApplyEventHandler();
  #endregion Signals

  private DisplaySettings settings = default!;

  public void OnReady()
  {
    ApplyButton.Pressed += OnApplyButtonPressed;
    ExitButton.Pressed += OnExitButtonPressed;

    Windowed.GrabFocus();

    MetalFXSupported = RenderingServer.GetCurrentRenderingDriverName() == "metal";

    if (!MetalFXSupported)
    {
      MetalFXSpatial.Hide();
      MetalFXTemporal.Hide();
    }

    foreach (var menu in new[] { DisplayMode, VSync, MaxFPS, ResolutionScale, ResolutionScaleFilter,
    TAA, MSAA, SSAA, ShadowMapping, GlobalIlluminationType, GlobalIlluminationQuality,
    SSAO, SSIL, Bloom, VolumetricFog })
    {
      MakeButtonGroup(menu);
    }
  }

  public void LoadDefaultSettings()
  {
    Windowed.GrabFocus();
    settings = AppRepo.GetSavedDisplaySettings();

    switch (settings.DisplayMode)
    {
      case Window.ModeEnum.Fullscreen:
        Fullscreen.ButtonPressed = true;
        break;
      case Window.ModeEnum.Windowed:
      case Window.ModeEnum.Maximized:
        Windowed.ButtonPressed = true;
        break;
      case Window.ModeEnum.ExclusiveFullscreen:
        ExclusiveFullscreen.ButtonPressed = true;
        break;
    }

    switch (settings.VSyncMode)
    {
      case DisplayServer.VSyncMode.Disabled:
        VSyncDisabled.ButtonPressed = true;
        break;
      case DisplayServer.VSyncMode.Enabled:
        VSyncEnabled.ButtonPressed = true;
        break;
      case DisplayServer.VSyncMode.Adaptive:
        VSyncAdaptive.ButtonPressed = true;
        break;
      case DisplayServer.VSyncMode.Mailbox:
        VSyncMailbox.ButtonPressed = true;
        break;
    }

    switch (settings.MaxFPS)
    {
      case 30:
        FPS30.ButtonPressed = true;
        break;
      case 40:
        FPS40.ButtonPressed = true;
        break;
      case 60:
        FPS60.ButtonPressed = true;
        break;
      case 72:
        FPS72.ButtonPressed = true;
        break;
      case 90:
        FPS90.ButtonPressed = true;
        break;
      case 120:
        FPS120.ButtonPressed = true;
        break;
      case 144:
        FPS144.ButtonPressed = true;
        break;
      case 0:
        FPSUnlimited.ButtonPressed = true;
        break;
    }

    switch (settings.Scaling3DScale)
    {
      case Scaling3DScale.UltraPerformance:
        UltraPerformance.ButtonPressed = true;
        break;
      case Scaling3DScale.Performance:
        Performance.ButtonPressed = true;
        break;
      case Scaling3DScale.Balanced:
        Balanced.ButtonPressed = true;
        break;
      case Scaling3DScale.Quality:
        Quality.ButtonPressed = true;
        break;
      case Scaling3DScale.UltraQuality:
        UltraQuality.ButtonPressed = true;
        break;
      case Scaling3DScale.Native:
        Native.ButtonPressed = true;
        break;
    }

    switch (settings.Scaling3DMode)
    {
      case Viewport.Scaling3DModeEnum.Bilinear:
        Bilinear.ButtonPressed = true;
        break;
      case Viewport.Scaling3DModeEnum.Fsr:
        FSR1.ButtonPressed = true;
        break;
      // case Viewport.Scaling3DModeEnum.MetalFXSpatial:
      //   MetalFXSpatial.ButtonPressed = true;
      //   break;
      case Viewport.Scaling3DModeEnum.Fsr2:
        FSR2.ButtonPressed = true;
        break;
        // case Viewport.Scaling3DModeEnum.MetalFxTemporal:
        //   MetalFXTemporal.ButtonPressed = true;
        //   break;
    }

    switch (settings.Msaa)
    {
      case Viewport.Msaa.Disabled:
        MsaaDisabled.ButtonPressed = true;
        break;
      case Viewport.Msaa.Msaa2X:
        Msaa2X.ButtonPressed = true;
        break;
      case Viewport.Msaa.Msaa4X:
        Msaa4X.ButtonPressed = true;
        break;
      case Viewport.Msaa.Msaa8X:
        Msaa8X.ButtonPressed = true;
        break;

    }

    switch (settings.Ssaa)
    {
      case Viewport.ScreenSpaceAAEnum.Disabled:
        SSAADisabled.ButtonPressed = true;
        break;
      case Viewport.ScreenSpaceAAEnum.Fxaa:
        FXAA.ButtonPressed = true;
        break;
      case Viewport.ScreenSpaceAAEnum.Smaa:
        SMAA.ButtonPressed = true;
        break;
    }

    switch (settings.Shadows)
    {
      case false:
        ShadowsDisabled.ButtonPressed = true;
        break;
      case true:
        ShadowsEnabled.ButtonPressed = true;
        break;
    }

    switch (settings.GlobalIlluminationType)
    {
      case GIType.LIGHTMAP_GI:
        LightmapGI.ButtonPressed = true;
        break;
      case GIType.VOXEL_GI:
        VoxelGI.ButtonPressed = true;
        break;
      case GIType.SDFGI:
        SDFGI.ButtonPressed = true;
        break;
    }

    switch (settings.GlobalIlluminationQuality)
    {
      case GIQuality.DISABLED:
        GIQualityDisabled.ButtonPressed = true;
        break;
      case GIQuality.LOW:
        GIQualityLow.ButtonPressed = true;
        break;
      case GIQuality.HIGH:
        GIQualityHigh.ButtonPressed = true;
        break;
    }

    switch (settings.ScreenSpaceAOQuality)
    {
      case SSAOQuality.DISABLED:
        SSAODisabled.ButtonPressed = true;
        break;
      case SSAOQuality.MEDIUM:
        SSAOMedium.ButtonPressed = true;
        break;
      case SSAOQuality.HIGH:
        SSAOHigh.ButtonPressed = true;
        break;
    }

    switch (settings.ScreenSpaceILQuality)
    {
      case SSILQuality.DISABLED:
        SSILDisabled.ButtonPressed = true;
        break;
      case SSILQuality.MEDIUM:
        SSILMedium.ButtonPressed = true;
        break;
      case SSILQuality.HIGH:
        SSILHigh.ButtonPressed = true;
        break;
    }

    switch (settings.Taa)
    {
      case false:
        TaaDisabled.ButtonPressed = true;
        break;
      case true:
        TaaEnabled.ButtonPressed = true;
        break;
    }

    switch (settings.Bloom)
    {
      case false:
        BloomDisabled.ButtonPressed = true;
        break;
      case true:
        BloomEnabled.ButtonPressed = true;
        break;
    }

    switch (settings.VolumetricFog)
    {
      case false:
        VolumetricFogDisabled.ButtonPressed = true;
        break;
      case true:
        VolumetricFogEnabled.ButtonPressed = true;
        break;
    }
  }

  public void OnExitTree()
  {
    ApplyButton.Pressed -= OnApplyButtonPressed;
    ExitButton.Pressed -= OnExitButtonPressed;
  }

  public void OnApplyButtonPressed()
  {
    //receive settings from menu
    DisplaySettings settings = new DisplaySettings()
    {
      DisplayMode = Windowed.ButtonPressed ? Window.ModeEnum.Windowed :
                    Fullscreen.ButtonPressed ? Window.ModeEnum.Fullscreen :
                    ExclusiveFullscreen.ButtonPressed ? Window.ModeEnum.ExclusiveFullscreen : Window.ModeEnum.Windowed,
      VSyncMode = VSyncDisabled.ButtonPressed ? DisplayServer.VSyncMode.Disabled :
                    VSyncEnabled.ButtonPressed ? DisplayServer.VSyncMode.Enabled :
                    VSyncAdaptive.ButtonPressed ? DisplayServer.VSyncMode.Adaptive :
                    VSyncMailbox.ButtonPressed ? DisplayServer.VSyncMode.Mailbox : DisplayServer.VSyncMode.Disabled,
      MaxFPS = FPS30.ButtonPressed ? 30 : FPS40.ButtonPressed ? 40 : FPS60.ButtonPressed ? 60 : FPS72.ButtonPressed ? 72 :
              FPS90.ButtonPressed ? 90 : FPS120.ButtonPressed ? 120 : FPS144.ButtonPressed ? 144 : FPSUnlimited.ButtonPressed ? 0 : 60,
      Scaling3DMode = Bilinear.ButtonPressed ? Viewport.Scaling3DModeEnum.Bilinear : FSR1.ButtonPressed ? Viewport.Scaling3DModeEnum.Fsr :
                      // MetalFXSpatial.ButtonPressed ? Viewport.Scaling3DModeEnum.MetalFXSpatial :
                      FSR2.ButtonPressed ? Viewport.Scaling3DModeEnum.Fsr2 :
                      // MetalFXTemporal.ButtonPressed ? Viewport.Scaling3DModeEnum.MetalFXTemporal :
                      Viewport.Scaling3DModeEnum.Bilinear,
      Msaa = MsaaDisabled.ButtonPressed ? Viewport.Msaa.Disabled : Msaa2X.ButtonPressed ? Viewport.Msaa.Msaa2X :
              Msaa4X.ButtonPressed ? Viewport.Msaa.Msaa4X : Msaa8X.ButtonPressed ? Viewport.Msaa.Msaa8X : Viewport.Msaa.Disabled,
      Scaling3DScale = UltraPerformance.ButtonPressed ? Scaling3DScale.UltraPerformance : Performance.ButtonPressed ? Scaling3DScale.Performance :
                      Balanced.ButtonPressed ? Scaling3DScale.Balanced : Quality.ButtonPressed ? Scaling3DScale.Quality :
                       UltraQuality.ButtonPressed ? Scaling3DScale.UltraQuality : Native.ButtonPressed ? Scaling3DScale.Native : Scaling3DScale.Native,
      Taa = TaaDisabled.ButtonPressed ? false : TaaEnabled.ButtonPressed ? true : true,
      Ssaa = SSAADisabled.ButtonPressed ? Viewport.ScreenSpaceAAEnum.Disabled : FXAA.ButtonPressed ? Viewport.ScreenSpaceAAEnum.Fxaa :
              SMAA.ButtonPressed ? Viewport.ScreenSpaceAAEnum.Smaa : Viewport.ScreenSpaceAAEnum.Disabled,
      Shadows = ShadowsDisabled.ButtonPressed ? false : ShadowsEnabled.ButtonPressed ? true : true,
      GlobalIlluminationType = LightmapGI.ButtonPressed ? GIType.LIGHTMAP_GI : VoxelGI.ButtonPressed ? GIType.VOXEL_GI : SDFGI.ButtonPressed ? GIType.SDFGI : GIType.LIGHTMAP_GI,
      GlobalIlluminationQuality = GIQualityDisabled.ButtonPressed ? GIQuality.DISABLED : GIQualityLow.ButtonPressed ? GIQuality.LOW : GIQualityHigh.ButtonPressed ? GIQuality.HIGH : GIQuality.LOW,
      ScreenSpaceAOQuality = SSAODisabled.ButtonPressed ? SSAOQuality.DISABLED : SSAOMedium.ButtonPressed ? SSAOQuality.MEDIUM : SSAOHigh.ButtonPressed ? SSAOQuality.HIGH : SSAOQuality.MEDIUM,
      ScreenSpaceILQuality = SSILDisabled.ButtonPressed ? SSILQuality.DISABLED : SSILMedium.ButtonPressed ? SSILQuality.MEDIUM : SSILHigh.ButtonPressed ? SSILQuality.HIGH : SSILQuality.MEDIUM,
      Bloom = BloomDisabled.ButtonPressed ? false : BloomEnabled.ButtonPressed ? true : true,
      VolumetricFog = VolumetricFogDisabled.ButtonPressed ? false : VolumetricFogEnabled.ButtonPressed ? true : true,
    };

    AppRepo.SaveDisplaySettings(settings);
    AppRepo.ApplyDisplaySettings(settings);
  }

  public void OnExitButtonPressed() => EmitSignal(SignalName.ExitSettingsMenu);

  public void MakeButtonGroup(IHBoxContainer menu)
  {
    var group = new ButtonGroup();

    foreach (var btn in menu.GetChildren())
    {
      if (btn is BaseButton btn2)
      {
        btn2.ButtonGroup = group;
      }
    }

  }
}
