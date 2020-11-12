using Godot;

/// <summary>
/// This is the <c>Main</c> class.
/// </summary>
public class Main : Node2D
{
    private BulletManager _bulletManager = null!;
    private Camera2D _camera = null!;

    [Export]
    private PackedScene _playerFactory = null!;

    public override void _Ready()
    {
        _bulletManager = GetNode<BulletManager>("BulletManager")!;
        _camera = GetNode<Camera2D>("Camera2D")!;

        GlobalSignals.Instance.Connect(
            nameof(GlobalSignals.BulletFired),
            _bulletManager,
            nameof(_bulletManager.SpawnBullet)
        );

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var player = _playerFactory.Instance() as Player;
        AddChild(player);
        player!.SetCameraTransform(_camera.GetPath());
        player.Connect(nameof(Player.OnDeath), this, nameof(SpawnPlayer));
    }
}
