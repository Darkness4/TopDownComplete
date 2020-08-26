using Godot;

public class RealisticBody2D : KinematicBody2D
{
    private const int SPEED = 200;
    private const float FRICTION = 0.2f;
    private const float ACCELERATION = 0.1f;

    private const float ROTATION_WEIGHT = 0.1f;

    private Vector2 _velocity = Vector2.Zero;

    /// <summary>
    /// Rotate the body toward a <c>location</c>.
    /// </summary>
    public void RotateToward(Vector2 location)
    {
        var angleToLocation = GlobalPosition.DirectionTo(location).Angle();
        Rotation = Mathf.LerpAngle(Rotation, angleToLocation, ROTATION_WEIGHT);
    }

    /// <summary>
    /// Move the body toward a <c>location</c>.
    /// </summary>
    public void MoveToward(Vector2 location)
    {
        var directionVelocity = GlobalPosition.DirectionTo(location) * SPEED;

        if (directionVelocity.Length() > 0)
        {
            _velocity = _velocity.LinearInterpolate(directionVelocity, ACCELERATION);
        }
        else
        {
            _velocity = _velocity.LinearInterpolate(Vector2.Zero, FRICTION);
        }
        _velocity = MoveAndSlide(_velocity);
    }
}
