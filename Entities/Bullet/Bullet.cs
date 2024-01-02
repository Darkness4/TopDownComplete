using Godot;

/// <summary>
/// Defines the <c>Bullet</c> behavior.
/// </summary>
public partial class Bullet : Area2D
{
    private Timer _killTimer = null!;

    private const int SPEED = 100;

    private Vector2 _direction = Vector2.Zero;
    private TeamName _teamName = TeamName.UNDEFINED;

    private Vector2 Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            Rotation += Direction.Angle();
        }
    }

    /// <summary>
    /// <c>Bullet</c> constructor.
    /// </summary>
    public void Initialize(
        Vector2 position,
        Vector2 direction,
        TeamName teamName
    )
    {
        GlobalPosition = position;
        Direction = direction;
        _teamName = teamName;
    }

    public override void _Ready()
    {
        _killTimer = GetNode<Timer>("KillTimer")!;
        _killTimer.Start();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_direction != Vector2.Zero)
        {
            var velocity = _direction * SPEED;

            GlobalPosition += velocity;
        }
    }

    /// <summary>
    /// <c>Bullet</c> has timed out.
    /// </summary>
    private void OnKillTimerTimeout()
    {
        QueueFree();
    }

    /// <summary>
    /// <c>Bullet</c> has hit an object.
    /// </summary>
    private void OnBulletBodyEntered(Node2D body)
    {
        if (
          body is IHittable hittable &&
          body is ITeamed teamed &&
          _teamName != teamed.TeamName
        )
        {
            hittable.HandleHit();
        }
        QueueFree();
    }
}
