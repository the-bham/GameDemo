namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class SettingsMenuStateTest : TestClass
{
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.SettingsMenu _state = default!;
  private AppLogic.Data _data = default!;


  public SettingsMenuStateTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _appRepo = new();
    _data = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
    _context.Set(_data);
  }

  [Test]

  public void OnEnter()
  {
    _state.Enter();
    _context.Outputs.ShouldBe([
      new AppLogic.Output.HideMainMenu(),
      new AppLogic.Output.ShowSettingsMenu()
    ]);
  }

  [Test]

  public void OnExit()
  {
    _state.Exit();
    _context.Outputs.ShouldBe([
      new AppLogic.Output.HideSettingsMenu()
    ]);
  }

  [Test]
  public void RespondsToMainMenu()
  {
    var next = _state.On(new AppLogic.Input.MainMenu());

    next.State.ShouldBeOfType<AppLogic.State.MainMenu>();
  }
}
