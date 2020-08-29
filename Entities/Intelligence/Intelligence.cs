using Godot;
using System;
using System.Collections.Generic;

public enum IntelligenceState
{
    UNINITIALIZED,
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
    private CapturableBaseManager _capturableBaseManager = null!;

    private const int PATROL_RANGE = 100;
    private const int ACCEPTABLE_CAPTURE_RANGE = 10;

    private LinkedList<KinematicBody2D> _targets = new LinkedList<KinematicBody2D>();

    // Patrol-related
    private Vector2 _origin = Vector2.Zero;
    private Vector2 _patrolLocation = Vector2.Zero;
    private bool _patrolLocationReached = false;

    // GoForCapture-related
    private CapturableBase? _closestBase = null;

    private IntelligenceState _currentState = IntelligenceState.UNINITIALIZED;
    public IntelligenceState CurrentState
    {
        get => _currentState;
        private set
        {
            if (_currentState == value)
            {
                return;
            }
            if (value == IntelligenceState.PATROL)
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

    public void Uninitialize()
    {
        CurrentState = IntelligenceState.UNINITIALIZED;
    }

    [Signal]
    public delegate void StateChanged(IntelligenceState state);

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
        CurrentState = IntelligenceState.GO_FOR_CAPTURE;
    }

    public override void _Ready()
    {
        _detectionZone = GetNode<Area2D>("DetectionZone")!;
        _patrolTimer = GetNode<Timer>("PatrolTimer")!;
        _checkForClosestTimer = GetNode<Timer>("CheckForClosestTimer");
        _capturableBaseManager = GetTree().CurrentScene.GetNode<CapturableBaseManager>("CapturableBaseManager")!;
        _capturableBaseManager.Connect(nameof(CapturableBase.OnBaseCaptured), this, nameof(HandleBaseCaptured));
    }

    public override void _PhysicsProcess(float delta)
    {
        switch (CurrentState)
        {
            case IntelligenceState.PATROL:
                OnPatrolState();
                break;
            case IntelligenceState.ENGAGE:
                OnEngageState();
                break;
            case IntelligenceState.GO_FOR_CAPTURE:
                OnGoForCaptureState();
                break;
            case IntelligenceState.UNINITIALIZED:
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

    public void HandleBaseCaptured()
    {
        if (CurrentState != IntelligenceState.ENGAGE)
        {
            CurrentState = IntelligenceState.GO_FOR_CAPTURE;
        }
    }

    private void _on_DetectionZone_body_entered(Node body)
    {
        if (body is ITeamed teamedBody && IsEnemyWith(teamedBody))
        {
            CurrentState = IntelligenceState.ENGAGE;
            _targets.AddLast((KinematicBody2D)body);
        }
    }

    private void _on_DetectionZone_body_exited(Node body)
    {
        if (body is ITeamed teamedBody && IsEnemyWith(teamedBody))
        {
            _targets.Remove((KinematicBody2D)body);

            if (_targets.Count == 0 && CurrentState != IntelligenceState.UNINITIALIZED)
            {
                CurrentState = IntelligenceState.GO_FOR_CAPTURE;
            }
        }
    }

    private void _on_PatrolTimer_timeout()
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
            if (_lineOfSight.GetCollider() is ITeamed teamedTarget && IsEnemyWith(teamedTarget)
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
            _closestBase = FindClosestCapturableBase();
        }

        if (_closestBase != null && _actor.GlobalPosition.DistanceTo(_closestBase.GlobalPosition) > ACCEPTABLE_CAPTURE_RANGE)
        {
            _actor.MoveToward(_closestBase.GlobalPosition);
            _actor.RotateToward(_closestBase.GlobalPosition);
        }
        else
        {
            CurrentState = IntelligenceState.PATROL;
        }
    }

    private CapturableBase? FindClosestCapturableBase()
    {
        CapturableBase? closestBase = null;
        float minDistance = -1f;

        foreach (var node in _capturableBaseManager.GetChildren())
        {
            if (node is CapturableBase currentBase && currentBase.TeamName != _actor.TeamName)
            {
                var distance = currentBase.GlobalPosition.DistanceTo(_actor.GlobalPosition);
                if (closestBase == null || distance < minDistance)
                {
                    minDistance = distance;
                    closestBase = currentBase;
                }
            }
        }

        return closestBase;
    }

    private bool IsEnemyWith(ITeamed teamedBody)
    {
        return teamedBody.TeamName != _actor.TeamName;
    }
}
