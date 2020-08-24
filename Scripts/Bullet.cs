using Godot;

/// <summary>
/// Defines the <c>Bullet</c> behavior.
/// </summary>
public class Bullet : Area2D
{
  private Timer _killTimer;

  private const int SPEED = 300;
  private Vector2 _direction = Vector2.Zero;

  /// <summary>
  /// Direction of the <c>Bullet</c>.
  /// </summary>
  public Vector2 Direction
  {
    get
    {
      return _direction;
    }

    set
    {
      _direction = value;
      Rotation += Direction.Angle();
    }
  }

  public override void _Process(float delta)
  {
    _killTimer = GetNode<Timer>("KillTimer");
    _killTimer.Start();
  }

  public override void _PhysicsProcess(float delta)
  {
    if (_direction != Vector2.Zero)
    {
      var velocity = _direction * SPEED;

      GlobalPosition += velocity;
    }
  }

  public void _on_KillTimer_timeout()
  {
    QueueFree();
  }
}
