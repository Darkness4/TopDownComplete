using Godot;

public partial class RealisticBody2D : CharacterBody2D
{
    private const int SPEED = 200;
    private const float FRICTION = 0.2f;
    private const float ACCELERATION = 0.1f;

    private const float ROTATION_WEIGHT = 0.1f;

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
            Velocity = Velocity.Lerp(directionVelocity, ACCELERATION);
        }
        else
        {
            Velocity = Velocity.Lerp(Vector2.Zero, FRICTION);
        }
        MoveAndSlide();
    }
}
