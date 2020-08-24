using Godot;

/// <summary>
/// Manages all in-game bullet.
/// </summary>
public class BulletManager : Node2D
{
  [Export]
  private PackedScene _bullet;

  /// <summary>
  /// Spawn a bullet
  /// </summary>
  public void SpawnBullet(
    Vector2 position,
    Vector2 direction
  )
  {
    var bulletInstance = (Bullet)_bullet.Instance();
    bulletInstance.GlobalPosition = position;
    bulletInstance.Direction = direction;
    AddChild(bulletInstance);
  }
}
