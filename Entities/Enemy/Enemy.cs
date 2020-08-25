using Godot;

/// <summary>
/// Defines the <c>Enemy</c> behavior.
/// </summary>
public class Enemy : KinematicBody2D
{
    private Health _health = null!;

    public override void _Ready()
    {
        _health = GetNode<Health>("Health");
        _health.Connect(nameof(Health.IsZero), this, nameof(Die));
    }

    /// <summary>
    /// <c>Enemy</c> is hit by an object.
    /// </summary>
    public void HandleHit()
    {
        _health.Decrease();
        GD.Print($"Enemy: {_health}");
    }

    /// <summary>
    /// Kill the <c>Enemy</c>.
    /// </summary>
    public void Die()
    {
        QueueFree();
    }
}
