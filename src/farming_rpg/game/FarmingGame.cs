namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.SaveFileBuilder;
using Godot;


public interface IFarmingGame : INode2D,
IProvide<IGameRepo>, IProvide<ISaveChunk<GameData>>, IProvide<EntityTable>
{
  void LoadExistingGame();

  event FarmingGame.SaveFileLoadedEventHandler? SaveFileLoaded;
}

[Meta(typeof(IAutoNode))]
public partial class FarmingGame : Node2D, IFarmingGame
{

  public override void _Notification(int what) => this.Notify(what);

  #region Save

  [Signal]
  public delegate void SaveFileLoadedEventHandler();

  public EntityTable EntityTable { get; set; } = new();
  EntityTable IProvide<EntityTable>.Value() => EntityTable;
  public ISaveFile<GameData> SaveFile { get; set; } = default!;
  public ISaveChunk<GameData> GameChunk { get; set; } = default!;
  ISaveChunk<GameData> IProvide<ISaveChunk<GameData>>.Value() => GameChunk;

  #endregion Save

  #region State

  public IGameRepo GameRepo { get; set; } = default!;

  #endregion State

  #region Nodes

  [Node] public IPlayer2D Player { get; set; } = default!;

  #endregion Nodes

  #region Provisions

  IGameRepo IProvide<IGameRepo>.Value() => GameRepo;
  public IGameLogic GameLogic { get; set; } = default!;

  public GameLogic.IBinding GameBinding { get; set; } = default!;

  #endregion Provisions

  #region Dependencies

  [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();

  #endregion Dependencies

  public void Setup()
  {
    GameRepo = new GameRepo();
    GameLogic = new GameLogic();
    GameLogic.Set(GameRepo);
    GameLogic.Set(AppRepo);
  }

  public void OnResolved()
  {

    GameBinding = GameLogic.Bind();

    GameLogic.Start();

    this.Provide();
  }


  public void LoadExistingGame()
  {
    SaveFile.Load()
      .ContinueWith((_) => CallDeferred(nameof(FinishedLoadingSaveFile)));
  }

  private void FinishedLoadingSaveFile()
   => EmitSignal(SignalName.SaveFileLoaded);

}
