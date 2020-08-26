using Godot;
using System;
using System.Collections.Generic;

public enum State
{
    UNITIALIZED,
    PATROL,
    ENGAGE,
    GO_FOR_CAPTURE
}

public class Intelligence : Node
{
    private Area2D _detectionZone = null!;
    private Weapon _weapon = null!;
    private Actor _actor = null!;
    private RayCast2D _lineOfSight = null!;
    private Timer _patrolTimer = null!;
    private Timer _checkForClosestTimer = null!;

    private const int PATROL_RANGE = 100;
    private const int ACCEPTABLE_CAPTURE_RANGE = 10;

    private LinkedList<KinematicBody2D> _targets = new LinkedList<KinematicBody2D>();

    // Patrol-related
    private Vector2 _origin = Vector2.Zero;
    private Vector2 _patrolLocation = Vector2.Zero;
    private bool _patrolLocationReached = false;

    // GoForCapture-related
    private CapturableBase? _closestBase = null;

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
                _origin = _actor.GlobalPosition;
                _patrolTimer.Start();
                _patrolLocation = GeneratePatrolLocation();
                _patrolLocationReached = false;
            }
            _currentState = value;
            GD.Print($"Set State {CurrentState}");
            EmitSignal(nameof(StateChanged), _currentState);
        }
    }

    [Signal]
    public delegate void StateChanged(State state);

    /// <summary>
    /// <c>Intelligence</c> constructor.
    /// </summary>
    public void Initialize(
        Actor actor,
        Weapon weapon,
        RayCast2D lineOfSight
    )
    {
        _actor = actor;
        _weapon = weapon;
        _lineOfSight = lineOfSight;

        _weapon.Connect(nameof(Weapon.OutOfAmmo), this, nameof(HandleReload));
        _origin = _actor.GlobalPosition;
        CurrentState = State.GO_FOR_CAPTURE;
    }

    public override void _Ready()
    {
        _detectionZone = GetNode<Area2D>("DetectionZone")!;
        _patrolTimer = GetNode<Timer>("PatrolTimer")!;
        _checkForClosestTimer = GetNode<Timer>("CheckForClosestTimer");
    }

    public override void _PhysicsProcess(float delta)
    {
        switch (CurrentState)
        {
            case State.PATROL:
                OnPatrolState();
                break;
            case State.ENGAGE:
                OnEngageState();
                break;
            case State.GO_FOR_CAPTURE:
                OnGoForCaptureState();
                break;
            case State.UNITIALIZED:
                break;
        }
    }

    /// <summary>
    /// Reload on signal.
    /// </summary>
    public void HandleReload()
    {
        _weapon.StartReload();
    }

    public void HandleBaseCapture(TeamName _)
    {
        CurrentState = State.GO_FOR_CAPTURE;
    }

    public void _on_DetectionZone_body_entered(Node body)
    {
        if (body is ITeamed teamedBody &&
            _actor is ITeamed teamedActor &&
            teamedBody.TeamName != teamedActor.TeamName
        )
        {
            CurrentState = State.ENGAGE;
            _targets.AddLast((KinematicBody2D)body);
        }
    }

    public void _on_DetectionZone_body_exited(Node body)
    {
        _targets.Remove((KinematicBody2D)body);

        if (_targets.Count == 0 && CurrentState != State.UNITIALIZED)
        {
            CurrentState = State.GO_FOR_CAPTURE;
        }
    }

    public void _on_PatrolTimer_timeout()
    {
        _patrolLocation = GeneratePatrolLocation();

        _actor.MoveToward(_patrolLocation);

        _patrolLocationReached = false;
    }

    private Vector2 GeneratePatrolLocation()
    {
        var randomizer = new Random();
        var randomX = randomizer.Next(-PATROL_RANGE, PATROL_RANGE);
        var randomY = randomizer.Next(-PATROL_RANGE, PATROL_RANGE);

        return new Vector2(randomX, randomY) + _origin;
    }

    private void OnPatrolState()
    {
        if (!_patrolLocationReached)
        {
            _actor.MoveToward(_patrolLocation);
            _actor.RotateToward(_patrolLocation);

            if (_actor.GlobalPosition.DistanceTo(_patrolLocation) < 5)
            {
                _patrolLocationReached = true;
                _patrolTimer.Start();
            }
        }
    }

    private void OnEngageState()
    {
        if (_targets.Count != 0)
        {
            var target = _targets.First.Value;

            _actor.RotateToward(target.GlobalPosition);
            if (_lineOfSight.GetCollider() is ITeamed teamedTarget &&
                _actor is ITeamed teamedActor &&
                teamedTarget.TeamName != teamedActor.TeamName
            )
            {
                _weapon.Shoot();
            }
        }
    }

    private void OnGoForCaptureState()
    {
        if (_checkForClosestTimer.IsStopped())
        {
            _checkForClosestTimer.Start();
            _closestBase = _actor.FindClosestCapturableBase();
        }

        if (_closestBase != null && _actor.GlobalPosition.DistanceTo(_closestBase.GlobalPosition) > ACCEPTABLE_CAPTURE_RANGE)
        {
            _actor.MoveToward(_closestBase.GlobalPosition);
            _actor.RotateToward(_closestBase.GlobalPosition);
        }
        else
        {
            CurrentState = State.PATROL;
        }
    }
}
