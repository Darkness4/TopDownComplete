using Godot;

public class Weapon : Node2D
{
    private Position2D _endOfGun = null!;
    private Position2D _gunDirection = null!;
    private Timer _attackCooldown = null!;
    private AnimationPlayer _animationPlayer = null!;
    private TeamName _teamName = TeamName.UNDEFINED;

    public override void _Ready()
    {
        _endOfGun = GetNode<Position2D>("EndOfGunPosition2D");
        _gunDirection = GetNode<Position2D>("GunDirectionPosition2D");
        _attackCooldown = GetNode<Timer>("AttackCooldown");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void Initialize(TeamName teamName)
    {
        _teamName = teamName;
    }

    public void Shoot()
    {
        if (_attackCooldown.IsStopped())
        {
            // Shoot a bullet
            var target = GetGlobalMousePosition();
            var direction = _endOfGun.GlobalPosition.DirectionTo(_gunDirection.GlobalPosition).Normalized();

            GlobalSignals.Instance.EmitSignal(nameof(GlobalSignals.BulletFired), _endOfGun.GlobalPosition, direction, _teamName);

            // Muzzle flash
            _animationPlayer.Play("MuzzleFlash");
            _attackCooldown.Start();
        }
    }
}
