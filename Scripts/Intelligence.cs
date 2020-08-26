using Godot;
using System;

public enum State
{
    UNITIALIZED,
    PATROL,
    ENGAGE
}

public class Intelligence : Node2D
{
    private Area2D _detectionZone = null!;
    private Weapon _weapon = null!;
    private RealisticBody2D _actor = null!;
    private RayCast2D _lineOfSight = null!;
    private Timer _patrolTimer = null!;

    private const int PATROL_RANGE = 100;

    private KinematicBody2D? _target = null;

    private Vector2 _origin = Vector2.Zero;
    private Vector2 _patrolLocation = Vector2.Zero;
    private bool _patrolLocationReached = false;

    private State _currentState = State.UNITIALIZED;
    public State CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState == value)
            {
                return;
            }
            if (value == State.PATROL)
            {
                _origin = GlobalPosition;
                _patrolTimer.Start();
                _patrolLocationReached = true;
            }
            _currentState = value;
            EmitSignal(nameof(StateChanged), _currentState);
        }
    }

    [Signal]
    public delegate void StateChanged(State state);

    public void Initialize(
        RealisticBody2D actor,
        Weapon weapon,
        RayCast2D lineOfSight
    )
    {
        _actor = actor;
        _weapon = weapon;
        _lineOfSight = lineOfSight;
    }

    public override void _Ready()
    {
        _detectionZone = GetNode<Area2D>("DetectionZone");
        _patrolTimer = GetNode<Timer>("PatrolTimer");
        _origin = GlobalPosition;
        CurrentState = State.PATROL;
    }

    public override void _PhysicsProcess(float delta)
    {
        switch (CurrentState)
        {
            case State.PATROL:
                if (!_patrolLocationReached)
                {
                    _actor.MoveToward(_patrolLocation);
                    _actor.RotateToward(_patrolLocation);

                    if (GlobalPosition.DistanceTo(_patrolLocation) < 5)
                    {
                        _patrolLocationReached = true;
                        _patrolTimer.Start();
                    }
                }
                break;
            case State.ENGAGE:
                if (_target != null)
                {
                    _actor.RotateToward(_target.GlobalPosition);
                    if (_lineOfSight.GetCollider() is ITeamed teamedTarget &&
                        _actor is ITeamed teamedActor &&
                        !teamedTarget.TeamName.Equals(teamedActor.TeamName)
                    )
                    {
                        _weapon.Shoot();
                    }
                }
                break;
            case State.UNITIALIZED:
                break;
        }
    }

    public void _on_DetectionZone_body_entered(Node body)
    {
        if (body is ITeamed teamedBody &&
            _actor is ITeamed teamedActor &&
            !teamedBody.TeamName.Equals(teamedActor.TeamName)
        )
        {
            CurrentState = State.ENGAGE;
            _target = (KinematicBody2D)body;
        }
    }

    public void _on_DetectionZone_body_exited(Node body)
    {
        if (_target != null && body == _target)
        {
            CurrentState = State.PATROL;
            _target = null;
        }
    }

    public void _on_PatrolTimer_timeout()
    {
        var randomizer = new Random();
        var randomX = randomizer.Next(-PATROL_RANGE, PATROL_RANGE);
        var randomY = randomizer.Next(-PATROL_RANGE, PATROL_RANGE);

        _patrolLocation = new Vector2(randomX, randomY) + _origin;

        _actor.MoveToward(_patrolLocation);

        _patrolLocationReached = false;
    }
}
