using Godot;

/// <summary>
/// Defines the <c>Bullet</c> behavior.
/// </summary>
public class Bullet : Area2D
{
    private Timer _killTimer = null!;

    private const int SPEED = 100;
    private Vector2 _direction = Vector2.Zero;

    /// <summary>
    /// Direction of the <c>Bullet</c>.
    /// </summary>
    public Vector2 Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            Rotation += Direction.Angle();
        }
    }
    public TeamName TeamName = TeamName.UNDEFINED;

    public override void _Ready()
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

    /// <summary>
    /// <c>Bullet</c> has timed out.
    /// </summary>
    public void _on_KillTimer_timeout()
    {
        QueueFree();
    }

    /// <summary>
    /// <c>Bullet</c> has hit an object.
    /// </summary>
    public void _on_Bullet_body_entered(Node body)
    {
        QueueFree();
        if (
          body is IHittable hittable &&
          body is ITeamed teamed &&
          TeamName != teamed.TeamName
        )
        {
            hittable.HandleHit();
        }
    }
}
