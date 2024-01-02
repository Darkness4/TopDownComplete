using Godot;

/// <summary>
/// Defines the <c>Player</c> behavior.
/// </summary>
public partial class Player : CharacterBody2D, ITeamed, IHittable, IUnit
{
    private Health _health = null!;
    private Weapon _weapon = null!;
    private RemoteTransform2D _cameraTransform = null!;

    private const int SPEED = 200;
    private const float FRICTION = 0.2f;
    private const float ACCELERATION = 0.1f;

    [Export]
    private TeamName _teamName = TeamName.PLAYER;

    [Signal]
    public delegate void OnDeathEventHandler();

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
        _health.Connect(Counter.SignalName.IsZero, new Callable(this, MethodName.Die));

        _weapon = GetNode<Weapon>("Weapon")!;
        _weapon.Initialize(_teamName);

        await ToSignal(GetTree(), "process_frame");
    }

    public override void _PhysicsProcess(double delta)
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
        EmitSignal(SignalName.OnDeath);
        QueueFree();
    }

    private void Move()
    {
        var inputVector = Vector2.Zero;
        if (Input.IsActionPressed("move_up"))
        {
            inputVector.Y -= 1;
        }
        if (Input.IsActionPressed("move_down"))
        {
            inputVector.Y += 1;
        }
        if (Input.IsActionPressed("move_left"))
        {
            inputVector.X -= 1;
        }
        if (Input.IsActionPressed("move_right"))
        {
            inputVector.X += 1;
        }

        var inputVelocity = inputVector.Normalized() * SPEED;

        if (inputVelocity.Length() > 0)
        {
            Velocity = Velocity.Lerp(inputVelocity, ACCELERATION);
        }
        else
        {
            Velocity = Velocity.Lerp(Vector2.Zero, FRICTION);
        }
        MoveAndSlide();
    }
}
