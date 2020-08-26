using Godot;

/// <summary>
/// Defines the <c>Actor</c> behavior.
/// </summary>
public class Actor : RealisticBody2D, ITeamed, IHittable, IUnit
{
    private Health _health = null!;
    private Weapon _weapon = null!;
    private RayCast2D _lineOfSight = null!;
    private Intelligence _intelligence = null!;
    private CapturableBaseManager _capturableBaseManager = null!;

    [Export]
    private readonly TeamName _teamName = TeamName.ENEMY;

    /// <summary>
    /// <c>TeamName</c> of the <c>Actor</c>.
    /// </summary>
    public TeamName TeamName
    {
        get => _teamName;
    }

    public override void _Ready()
    {
        _capturableBaseManager = GetTree().CurrentScene.GetNode<CapturableBaseManager>("CapturableBaseManager")!;

        _health = GetNode<Health>("Health")!;
        _health.Connect(nameof(Health.IsZero), this, nameof(Die));

        _weapon = GetNode<Weapon>("Weapon")!;
        _weapon.Initialize(_teamName);

        _lineOfSight = GetNode<RayCast2D>("LineOfSight")!;
        _intelligence = GetNode<Intelligence>("Intelligence")!;

        _intelligence.Initialize(this, _weapon, _lineOfSight);

        _capturableBaseManager.Connect(nameof(CapturableBase.BaseCaptured), _intelligence, nameof(_intelligence.HandleBaseCapture));
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
        _intelligence.CurrentState = State.UNITIALIZED;
        QueueFree();
    }

    /// <summary>
    /// Find the closest capturable base.
    /// </summary>
    public CapturableBase? FindClosestCapturableBase()
    {
        var currentBase = _capturableBaseManager.GetChildren().GetEnumerator();

        var closestBase = currentBase.Current as CapturableBase;
        float? minDistance = closestBase?.GlobalPosition.DistanceTo(GlobalPosition);

        while (currentBase.MoveNext())
        {
            var node = currentBase.Current as CapturableBase;

            if (node != null && node.TeamName != TeamName)
            {
                var distance = node.GlobalPosition.DistanceTo(GlobalPosition);
                if (minDistance == null || distance < minDistance)
                {
                    minDistance = distance;
                    closestBase = node;
                }
            }
        }
        return closestBase;
    }
}
