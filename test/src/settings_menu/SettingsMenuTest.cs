namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is Godot object; Godot will dispose"
  )
]
public class SettingsMenuTest : TestClass
{


  private Mock<IAppRepo> _appRepo = default!;
  private SettingsMenu _settingsMenu = default!;

  public SettingsMenuTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _settingsMenu = new SettingsMenu
    {

    };

    _settingsMenu._Notification(-1);
  }
}
