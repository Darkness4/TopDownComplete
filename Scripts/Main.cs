using Godot;

/// <summary>
/// This is the <c>Main</c> class.
/// </summary>
public class Main : Node2D
{
  private BulletManager _bulletManager;
  private Player _player;

  public override void _Ready()
  {
    _bulletManager = GetNode<BulletManager>("BulletManager");
    _player = GetNode<Player>("Player");

    _player.Connect(nameof(Player.PlayerFiredBullet), _bulletManager, nameof(_bulletManager.SpawnBullet));
  }
}
