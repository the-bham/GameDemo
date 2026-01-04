namespace GameDemo;

using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chickensoft.Introspection;
using Chickensoft.Serialization;
using Chickensoft.Serialization.Godot;
using Godot;

public enum Scaling3DScale
{
  UltraPerformance,
  Performance,
  Balanced,
  Quality,
  UltraQuality,
  Native,

}

public enum GIType
{
  LIGHTMAP_GI,
  VOXEL_GI,
  SDFGI,

}

public enum GIQuality
{
  DISABLED,
  LOW,
  HIGH
}

public enum SSAOQuality
{
  DISABLED,
  MEDIUM,
  HIGH
}

public enum SSILQuality
{
  DISABLED,
  MEDIUM,
  HIGH
}

// Register the enum on a System.Text.Json context so it will get metadata
// generated for it.
[JsonSerializable(typeof(Window.ModeEnum))]
[JsonSerializable(typeof(DisplayServer.VSyncMode))]
[JsonSerializable(typeof(Scaling3DScale))]
[JsonSerializable(typeof(Viewport.Scaling3DModeEnum))]
[JsonSerializable(typeof(Viewport.Msaa))]
[JsonSerializable(typeof(Viewport.ScreenSpaceAAEnum))]
[JsonSerializable(typeof(GIType))]
[JsonSerializable(typeof(GIQuality))]
[JsonSerializable(typeof(SSAOQuality))]
[JsonSerializable(typeof(SSILQuality))]
public partial class DisplaySettingsEnumContext : JsonSerializerContext;

[Meta, Id("settings")]
public partial record DisplaySettings
{
  [Save("display_mode")]
  public required Window.ModeEnum DisplayMode { get; init; }

  [Save("vsync_mode")]
  public required DisplayServer.VSyncMode VSyncMode { get; init; }

  [Save("max_fps")]
  public required int MaxFPS { get; init; }

  [Save("scaling_3d_scale")]
  public required Scaling3DScale Scaling3DScale { get; init; }

  [Save("scaling_3d_mode")]
  public required Viewport.Scaling3DModeEnum Scaling3DMode { get; init; }

  [Save("taa")]
  public required bool Taa { get; init; }


  [Save("msaa")]
  public required Viewport.Msaa Msaa { get; init; }

  [Save("ssaa")]
  public required Viewport.ScreenSpaceAAEnum Ssaa { get; init; }

  [Save("shadow_mapping")]
  public required bool Shadows { get; init; }

  [Save("global_illumination_type")]
  public required GIType GlobalIlluminationType { get; init; }

  [Save("global_illumination_quality")]
  public required GIQuality GlobalIlluminationQuality { get; init; }

  [Save("screen_space_ambient_occlusion")]
  public required SSAOQuality ScreenSpaceAOQuality { get; init; }

  [Save("screen_space_indirect_lighting")]
  public required SSILQuality ScreenSpaceILQuality { get; init; }

  [Save("bloom")]
  public required bool Bloom { get; init; }

  [Save("volumetric_fog")]
  public required bool VolumetricFog { get; init; }
}
