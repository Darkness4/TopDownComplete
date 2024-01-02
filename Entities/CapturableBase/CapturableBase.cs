using Godot;
using System.Collections.Generic;

public partial class CapturableBase : Area2D, ITeamed
{
    private Timer _captureTimer = null!;
    private Sprite2D _sprite = null!;

    private readonly Color PLAYER_COLOR = new(0, 0.431373f, 0);
    private readonly Color ENEMY_COLOR = new(0, 0.188725f, 0.431373f);
    private readonly Color UNDEFINED_COLOR = new(1, 1, 1);

    private readonly Dictionary<TeamName, int> unitCount = new() {
        {TeamName.PLAYER, 0},
        {TeamName.ENEMY, 0}
    };
    private TeamName _capturingTeam = TeamName.UNDEFINED;

    [Export]
    private TeamName _teamName = TeamName.UNDEFINED;

    [Signal]
    public delegate void OnBaseCapturedEventHandler(TeamName teamName);

    /// <summary>
    /// <c>TeamName</c> of the <c>CapturableBase</c>.
    /// </summary>
    public TeamName TeamName
    {
        get => _teamName;
        set
        {
            if (_teamName != value)
            {
                EmitSignal(SignalName.OnBaseCaptured, (int)_teamName);
            }
            _teamName = value;
            switch (_teamName)
            {
                case TeamName.PLAYER:
                    _sprite.Modulate = PLAYER_COLOR;
                    break;
                case TeamName.ENEMY:
                    _sprite.Modulate = ENEMY_COLOR;
                    break;
                case TeamName.UNDEFINED:
                    _sprite.Modulate = UNDEFINED_COLOR;
                    break;
            }
        }
    }

    public override void _Ready()
    {
        _captureTimer = GetNode<Timer>("CaptureTimer")!;
        _sprite = GetNode<Sprite2D>("Sprite2D")!;
    }

    private void OnCapturableBaseBodyEntered(Node2D body)
    {
        if (body is ITeamed teamed && body is IUnit)
        {
            try
            {
                unitCount[teamed.TeamName]++;
                CanBeCaptured();
            }
            catch (KeyNotFoundException e)
            {
                GD.Print($"{e}: Are you sure that {body} is in a Team ?");
            }
        }
    }

    private void OnCapturableBaseBodyExited(Node2D body)
    {
        if (body is ITeamed teamed && body is IUnit)
        {
            try
            {
                unitCount[teamed.TeamName]--;
                CanBeCaptured();
            }
            catch (KeyNotFoundException e)
            {
                GD.Print($"{e}: Are you sure that {body} is in a Team ?");
            }
        }
    }

    private void CanBeCaptured()
    {
        var majorityTeam = GetTeamWithMajority();
        if (majorityTeam == TeamName.UNDEFINED)
        {
            GD.Print("Not capturing.");
            return;
        }
        else if (majorityTeam == TeamName)
        {
            GD.Print("Capture blocked.");
            _capturingTeam = TeamName.UNDEFINED;
            if (!_captureTimer.IsStopped())
            {
                _captureTimer.Stop();
            }
        }
        else
        {
            GD.Print($"{majorityTeam} is capturing");
            _capturingTeam = majorityTeam;

            if (_captureTimer.IsStopped())
            {
                _captureTimer.Start();
            }
        }
    }

    private TeamName GetTeamWithMajority()
    {
        if (unitCount[TeamName.PLAYER] == unitCount[TeamName.ENEMY])
        {
            return TeamName.UNDEFINED;
        }
        else if (unitCount[TeamName.PLAYER] > unitCount[TeamName.ENEMY])
        {
            return TeamName.PLAYER;
        }
        else
        {
            return TeamName.ENEMY;
        }
    }

    private void OnCaptureTimerTimeout()
    {
        TeamName = _capturingTeam;
    }
}
