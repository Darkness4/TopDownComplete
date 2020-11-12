using Godot;

/// <summary>
/// Defines the <c>Player</c> behavior.
/// </summary>
public class Player : KinematicBody2D, ITeamed, IHittable, IUnit
{
    private Health _health = null!;
    private Weapon _weapon = null!;
    private RemoteTransform2D _cameraTransform = null!;

    private const int SPEED = 200;
    private const float FRICTION = 0.2f;
    private const float ACCELERATION = 0.1f;

    private Vector2 _velocity = Vector2.Zero;

    [Export]
    private readonly TeamName _teamName = TeamName.PLAYER;

    [Signal]
    public delegate void OnDeath();

    /// <summary>
    /// <c>TeamName</c> of the <c>Player</c>.
    /// </summary>
    public TeamName TeamName
    {
        get => _teamName;
    }

    public override async void _Ready()
    {
        _cameraTransform = GetNode<RemoteTransform2D>("CameraTransform");
        _health = GetNode<Health>("Health")!;
        _health.Connect(nameof(Health.IsZero), this, nameof(Die));

        _weapon = GetNode<Weapon>("Weapon")!;
        _weapon.Initialize(_teamName);

        await ToSignal(GetTree(), "idle_frame");
    }

    public override void _PhysicsProcess(float delta)
    {
        Move();
        LookAt(GetGlobalMousePosition());
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("shoot"))
        {
            _weapon.Shoot();
        }
        else if (@event.IsActionPressed("reload"))
        {
            _weapon.StartReload();
        }
    }

    /// <summary>
    /// <c>Player</c> is hit by an object.
    /// </summary>
    public void HandleHit()
    {
        _health.Decrease();
        GD.Print($"Player: {_health}");
    }

    public void SetCameraTransform(NodePath cameraPath)
    {
        _cameraTransform.RemotePath = cameraPath;
    }

    private void Die()
    {
        EmitSignal(nameof(OnDeath));
        QueueFree();
    }

    private void Move()
    {
        var inputVector = Vector2.Zero;
        if (Input.IsActionPressed("move_up"))
        {
            inputVector.y -= 1;
        }
        if (Input.IsActionPressed("move_down"))
        {
            inputVector.y += 1;
        }
        if (Input.IsActionPressed("move_left"))
        {
            inputVector.x -= 1;
        }
        if (Input.IsActionPressed("move_right"))
        {
            inputVector.x += 1;
        }

        var inputVelocity = inputVector.Normalized() * SPEED;

        if (inputVelocity.Length() > 0)
        {
            _velocity = _velocity.LinearInterpolate(inputVelocity, ACCELERATION);
        }
        else
        {
            _velocity = _velocity.LinearInterpolate(Vector2.Zero, FRICTION);
        }
        _velocity = MoveAndSlide(_velocity);
    }
}
