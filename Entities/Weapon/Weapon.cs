using Godot;

public partial class Weapon : Node2D
{
    private Marker2D _endOfGun = null!;
    private Marker2D _gunDirection = null!;
    private Timer _attackCooldown = null!;
    private AnimationPlayer _animationPlayer = null!;
    private TeamName _teamName = TeamName.UNDEFINED;

    private const int MAX_AMMO_PER_MAG = 15;

    [Export]
    private int _mags = 2;
    private int _ammo = MAX_AMMO_PER_MAG - 1;
    private bool _ammoInChamber = true;

    [Signal]
    public delegate void OutOfAmmoEventHandler();

    public override void _Ready()
    {
        _endOfGun = GetNode<Marker2D>("EndOfGunPosition2D")!;
        _gunDirection = GetNode<Marker2D>("GunDirectionPosition2D")!;
        _attackCooldown = GetNode<Timer>("AttackCooldown")!;
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer")!;
    }

    /// <summary>
    /// <c>Weapon</c> constructor
    /// </summary>
    public void Initialize(TeamName teamName)
    {
        _teamName = teamName;
    }

    /// <summary>
    /// <c>Shoot</c> command.
    /// </summary>
    public void Shoot()
    {
        if (_attackCooldown.IsStopped() &&
            !_animationPlayer.IsPlaying()
        )
        {
            if (_ammoInChamber)
            {
                // Shoot a bullet
                var direction = _endOfGun.GlobalPosition.DirectionTo(_gunDirection.GlobalPosition).Normalized();

                GlobalSignals.Instance.EmitSignal(
                    GlobalSignals.SignalName.BulletFired,
                    _endOfGun.GlobalPosition,
                    direction,
                    (int)_teamName
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
                _attackCooldown.Start();

                EmitSignal(SignalName.OutOfAmmo);
            }
        }
    }

    /// <summary>
    /// Start reload animation.
    /// </summary>
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
