namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;

using Godot;


public interface IPlayer2D :
  ICharacterBody2D
{
  IPlayer2DLogic Player2DLogic { get; }

  Vector2 GetGlobalInputVector();
}

[Meta(typeof(IAutoNode))]
public partial class Player2D : CharacterBody2D,
IPlayer2D,
IProvide<IPlayer2DLogic>,
IProvide<Player2DLogic.Settings>
{
  public override void _Notification(int what) => this.Notify(what);

  #region Exports
  /// <summary>Player speed</summary>
  [Export(PropertyHint.Range, "0, 100, 0.1")]
  public float MoveSpeed { get; set; } = 30f;

  public Vector2 FacingDirection = Vector2.Down;

  #endregion Exports

  #region Nodes

  [Node("%AnimatedSprite")] public IAnimatedSprite2D AnimatedSprite { get; set; } = default!;

  #endregion Nodes


  #region Dependencies

  [Dependency]
  public IGameRepo GameRepo => this.DependOn<IGameRepo>();

  [Dependency]
  public IAppRepo AppRepo => this.DependOn<IAppRepo>();

  #endregion Dependencies


  #region Provisions

  IPlayer2DLogic IProvide<IPlayer2DLogic>.Value() => Player2DLogic;
  Player2DLogic.Settings IProvide<Player2DLogic.Settings>.Value() => Settings;


  #endregion Provisions

  #region State

  public IPlayer2DLogic Player2DLogic { get; set; } = default!;

  public Player2DLogic.Settings Settings { get; set; } = default!;

  public Player2DLogic.IBinding PlayerBinding { get; set; } = default!;

  #endregion State

  public void Setup()
  {
    Settings = new Player2DLogic.Settings(
      MoveSpeed
    );

    Player2DLogic = new Player2DLogic();

    Player2DLogic.Set(this as IPlayer2D);
    Player2DLogic.Set(Settings);
    Player2DLogic.Set(AppRepo);
    Player2DLogic.Set(GameRepo);
  }

  public void OnReady() => SetPhysicsProcess(true);

  public void OnExitTree()
  {
    Player2DLogic.Stop();
    PlayerBinding.Dispose();
  }

  public void OnResolved()
  {


    PlayerBinding = Player2DLogic.Bind();

    PlayerBinding
      .Handle((in Player2DLogic.Output.MovementComputed output) =>
      {
        if (output.Direction.Length() > 0)
        {
          FacingDirection = output.Direction;
        }

        Velocity = output.Velocity;
      })
      .Handle((in Player2DLogic.Output.VelocityChanged output) =>
      Velocity = output.Velocity
      ).Handle((in Player2DLogic.Output.Animate output) =>
        AnimatedSprite.Play(output.AnimationName)
      );

    // Allow the player model to lookup our state machine and bind to it.
    this.Provide();

    // Start the player state machine last.
    Player2DLogic.Start();
  }

  public void OnPhysicsProcess(double delta)
  {
    Player2DLogic.Input(new Player2DLogic.Input.PhysicsTick(delta));

    MoveAndSlide();

    Player2DLogic.Input(new Player2DLogic.Input.Animate(FacingDirection));
  }
  public Vector2 GetGlobalInputVector()
  {
    var moveInput = Input.GetVector("move_left", "move_right", "move_up", "move_down");

    return moveInput;
  }
}
