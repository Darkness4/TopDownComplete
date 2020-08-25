using Godot;

public class Weapon : Node2D
{
    private Position2D _endOfGun = null!;
    private Position2D _gunDirection = null!;
    private Timer _attackCooldown = null!;
    private AnimationPlayer _animationPlayer = null!;

    /// <summary>
    /// Fired a bullet with the following parameters.
    /// </summary>
    [Signal]
    public delegate void FiredBullet(
      Vector2 position,
      Vector2 direction
    );

    public override void _Ready()
    {
        _endOfGun = GetNode<Position2D>("EndOfGunPosition2D");
        _gunDirection = GetNode<Position2D>("GunDirectionPosition2D");
        _attackCooldown = GetNode<Timer>("AttackCooldown");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void Shoot()
    {
        if (_attackCooldown.IsStopped())
        {
            // Shoot a bullet
            var target = GetGlobalMousePosition();
            var direction = _endOfGun.GlobalPosition.DirectionTo(_gunDirection.GlobalPosition).Normalized();

            EmitSignal(nameof(FiredBullet), _endOfGun.GlobalPosition, direction);

            // Muzzle flash
            _animationPlayer.Play("MuzzleFlash");
            _attackCooldown.Start();
        }
    }
}
