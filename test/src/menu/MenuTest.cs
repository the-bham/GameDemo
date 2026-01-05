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
public class MenuTest : TestClass
{
  private Mock<IButton> _newGameButton = default!;
  private Mock<IButton> _loadGameButton = default!;
  private Mock<IButton> _settingsButton = default!;
  private Menu _menu = default!;

  public MenuTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _newGameButton = new Mock<IButton>();
    _loadGameButton = new Mock<IButton>();
    _settingsButton = new Mock<IButton>();

    _menu = new Menu
    {
      NewGameButton = _newGameButton.Object,
      LoadGameButton = _loadGameButton.Object,
      SettingsButton = _settingsButton.Object
    };

    _menu._Notification(-1);
  }

  [Test]
  public void Subscribes()
  {
    _menu.OnReady();
    _newGameButton.VerifyAdd(menu => menu.Pressed += _menu.OnNewGamePressed);
    _loadGameButton.VerifyAdd(menu => menu.Pressed += _menu.OnLoadGamePressed);
    _settingsButton.VerifyAdd(menu => menu.Pressed += _menu.OnSettingsPressed);

    _menu.OnExitTree();
    _newGameButton.VerifyRemove(menu => menu.Pressed -= _menu.OnNewGamePressed);
    _loadGameButton.VerifyRemove(menu => menu.Pressed -= _menu.OnLoadGamePressed);
    _settingsButton.VerifyRemove(menu => menu.Pressed -= _menu.OnSettingsPressed);
  }

  [Test]
  public async Task SignalsNewGameButtonPressed()
  {
    var signal = _menu.ToSignal(_menu, Menu.SignalName.NewGame);

    _menu.OnNewGamePressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Test]
  public async Task SignalLoadGameButtonPressed()
  {
    var signal = _menu.ToSignal(_menu, Menu.SignalName.LoadGame);

    _menu.OnLoadGamePressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }

  [Test]
  public async Task SignalSettingsButtonPressed()
  {
    var signal = _menu.ToSignal(_menu, Menu.SignalName.Settings);

    _menu.OnSettingsPressed();

    await signal;

    signal.IsCompleted.ShouldBeTrue();
  }
}
