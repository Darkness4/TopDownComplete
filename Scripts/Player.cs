using Godot;

/// <summary>
/// Defines the <c>Player</c> behavior.
/// </summary>
public class Player : KinematicBody2D
{
  private Position2D _endOfGun;
  private Position2D _gunDirection;

  private const int SPEED = 200;
  private const float FRICTION = 0.2f;
  private const float ACCELERATION = 0.1f;

  private Vector2 _velocity = Vector2.Zero;

  /// <summary>
  /// <c>Player</c> has fired a bullet with the following parameters.
  /// </summary>
  [Signal]
  public delegate void PlayerFiredBullet(
    Vector2 position,
    Vector2 direction
  );


  public override async void _Ready()
  {
    _endOfGun = GetNode<Position2D>("EndOfGunPosition2D");
    _gunDirection = GetNode<Position2D>("GunDirectionPosition2D");

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
      Shoot();
    }
  }

  /// <summary>
  /// Kill the <c>Player</c>.
  /// </summary>
  public void Die()
  {
    GetTree().ReloadCurrentScene();
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

  private void Shoot()
  {
    var target = GetGlobalMousePosition();
    var direction = _endOfGun.GlobalPosition.DirectionTo(_gunDirection.GlobalPosition).Normalized();

    EmitSignal(nameof(PlayerFiredBullet), _endOfGun.GlobalPosition, direction);
  }
}
