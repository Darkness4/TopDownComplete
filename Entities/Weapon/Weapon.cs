using Godot;

public class Weapon : Node2D
{
    private Position2D _endOfGun = null!;
    private Position2D _gunDirection = null!;
    private Timer _attackCooldown = null!;
    private AnimationPlayer _animationPlayer = null!;
    private TeamName _teamName = TeamName.UNDEFINED;

    private const int MAX_AMMO_PER_MAG = 15;

    [Export]
    private int _mags = 2;
    private int _ammo = MAX_AMMO_PER_MAG - 1;
    private bool _ammoInChamber = true;

    [Signal]
    public delegate void OutOfAmmo();

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
        if (_attackCooldown.IsStopped() &&
            !_animationPlayer.IsPlaying()
        )
        {
            if (_ammoInChamber)
            {
                // Shoot a bullet
                var target = GetGlobalMousePosition();
                var direction = _endOfGun.GlobalPosition.DirectionTo(_gunDirection.GlobalPosition).Normalized();

                GlobalSignals.Instance.EmitSignal(
                    nameof(GlobalSignals.BulletFired),
                    _endOfGun.GlobalPosition,
                    direction,
                    _teamName
                );

                // Muzzle flash
                _animationPlayer.Play("MuzzleFlash");
                _attackCooldown.Start();

                // Ammo
                if (_ammo > 0)
                {
                    _ammo--;
                }
                else if (_ammoInChamber)
                {
                    _ammoInChamber = false;
                }
            }
            else
            {
                // Click
                _attackCooldown.Start();

                EmitSignal(nameof(OutOfAmmo));
            }
        }
    }

    public void StartReload()
    {
        if (_mags == 0)
        {
            GD.Print("No mags left");
            return;
        }
        if (!_animationPlayer.IsPlaying())
        {
            _animationPlayer.Play("Reload");
            GD.Print("RELOADING !");
            return;
        }
        GD.Print("Cannot reload");
    }

    private void StopReload()
    {
        // Put the mag in the gun
        _mags--;
        _ammo = MAX_AMMO_PER_MAG;

        // Rack the gun
        if (!_ammoInChamber)
        {
            _ammoInChamber = true;
            _ammo--;
        }

        GD.Print($"{_mags} mags left");
    }
}
