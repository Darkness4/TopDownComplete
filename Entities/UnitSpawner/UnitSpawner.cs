using Godot;
using System.Collections.Generic;
using System.Linq;


public partial class UnitSpawner : Node, ITeamed
{
    private Node2D _unitContainer = null!;
    private Timer _spawnCooldown = null!;
    private Timer _waveSpawnTimer = null!;
    private SpawnPoint[] _spawnPoints = null!;

    [Export]
    private TeamName _teamName = TeamName.UNDEFINED;

    [Export]
    private PackedScene _unitFactory = null!;

    [Export]
    private bool _initialSpawnEnabled = true;

    [Export]
    private int _numberOfUnits = 10;

    private int _unitOnField = 0;

    private int _unitToSpawn = 0;

    /// <summary>
    /// <c>TeamName</c> of the <c>UnitSpawner</c>.
    /// </summary>
    public TeamName TeamName
    {
        get => _teamName;
    }

    public override void _Ready()
    {
        _unitContainer = GetNode<Node2D>("UnitContainer");
        _spawnCooldown = GetNode<Timer>("SpawnCooldown");
        _waveSpawnTimer = GetNode<Timer>("WaveSpawnTimer");
        _spawnPoints = GetChildrenSpawnPoints().ToArray();
        _unitToSpawn = _numberOfUnits;

        if (_initialSpawnEnabled)
        {
            SpawnAllUnits();
        }
    }

    public void SpawnAllUnits()
    {
        _spawnCooldown.Start();
    }

    public void SpawnOnAllPoints()
    {
        foreach (var spawnPoint in _spawnPoints)
        {
            Spawn(spawnPoint);
        }
    }

    public void Spawn(SpawnPoint spawnPoint)
    {
        if (spawnPoint.IsFree && _unitToSpawn > 0)
        {
            var actor = _unitFactory.Instantiate<Node2D>();
            actor!.GlobalPosition = spawnPoint.GlobalPosition;
            actor!.Connect(Actor.SignalName.OnDeath, new Callable(this, MethodName.HandleDeath));
            _unitContainer.AddChild(actor);

            _unitOnField++;
            _unitToSpawn--;
        }
        _spawnCooldown.Start();
    }

    private void HandleDeath()
    {
        if (_unitOnField > 0)
        {
            _unitOnField--;
            if (_waveSpawnTimer.IsStopped())
            {
                _waveSpawnTimer.Start();
            }
        }
    }

    private IEnumerable<SpawnPoint> GetChildrenSpawnPoints()
    {
        foreach (var node in GetChildren())
        {
            if (node is SpawnPoint spawnPoint)
            {
                yield return spawnPoint;
            }
        }
    }

    private void OnSpawnCooldownTimeout()
    {
        SpawnOnAllPoints();
    }

    private void OnWaveSpawnTimerTimeout()
    {
        _unitToSpawn = _numberOfUnits - _unitOnField;
        SpawnAllUnits();
    }
}
