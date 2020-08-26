using Godot;

/// <summary>
/// Defines the <c>Actor</c> behavior.
/// </summary>
public class Actor : RealisticBody2D, ITeamed, IHittable
{
    private Health _health = null!;
    private Weapon _weapon = null!;
    private RayCast2D _lineOfSight = null!;
    private Intelligence _intelligence = null!;

    private const int SPEED = 200;
    private const float FRICTION = 0.2f;
    private const float ACCELERATION = 0.1f;

    private Vector2 _velocity = Vector2.Zero;

    [Export]
    private TeamName _teamName = TeamName.ENEMY;
    public TeamName TeamName
    {
        get => _teamName;
    }

    public override void _Ready()
    {
        _health = GetNode<Health>("Health");
        _health.Connect(nameof(Health.IsZero), this, nameof(Die));

        _weapon = GetNode<Weapon>("Weapon");
        _weapon.Initialize(_teamName);

        _lineOfSight = GetNode<RayCast2D>("LineOfSight");
        _intelligence = GetNode<Intelligence>("Intelligence");

        _intelligence.Initialize(this, _weapon, _lineOfSight);
    }

    /// <summary>
    /// <c>Actor</c> is hit by an object.
    /// </summary>
    public void HandleHit()
    {
        _health.Decrease();
        GD.Print($"Actor: {_health}");
    }

    /// <summary>
    /// Kill the <c>Actor</c>.
    /// </summary>
    public void Die()
    {
        QueueFree();
    }
}
