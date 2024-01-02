using Godot;

/// <summary>
/// Defines the <c>Actor</c> behavior.
/// </summary>
public partial class Actor : RealisticBody2D, ITeamed, IHittable, IUnit
{
    private Health _health = null!;
    private Weapon _weapon = null!;
    private RayCast2D _lineOfSight = null!;
    private Intelligence _intelligence = null!;

    [Export]
    private TeamName _teamName = TeamName.UNDEFINED;

    [Signal]
    public delegate void OnDeathEventHandler();

    /// <summary>
    /// <c>TeamName</c> of the <c>Actor</c>.
    /// </summary>
    public TeamName TeamName
    {
        get => _teamName;
    }

    public override void _Ready()
    {
        _health = GetNode<Health>("Health")!;
        _health.Connect(Counter.SignalName.IsZero, new Callable(this, MethodName.Die));

        _weapon = GetNode<Weapon>("Weapon")!;
        _weapon.Initialize(_teamName);

        _lineOfSight = GetNode<RayCast2D>("LineOfSight")!;
        _intelligence = GetNode<Intelligence>("Intelligence")!;

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

    private void Die()
    {
        EmitSignal(SignalName.OnDeath);
        _intelligence.Uninitialize();
        QueueFree();
    }
}
