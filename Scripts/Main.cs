using Godot;

/// <summary>
/// This is the <c>Main</c> class.
/// </summary>
public class Main : Node2D
{
    private BulletManager _bulletManager = null!;
    private Player _player = null!;

    public override void _Ready()
    {
        _bulletManager = GetNode<BulletManager>("BulletManager")!;
        _player = GetNode<Player>("Player")!;

        GlobalSignals.Instance.Connect(
            nameof(GlobalSignals.BulletFired),
            _bulletManager,
            nameof(_bulletManager.SpawnBullet)
        );
    }
}
