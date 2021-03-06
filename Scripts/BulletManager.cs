using Godot;

/// <summary>
/// Manages all in-game bullet.
/// </summary>
public class BulletManager : Node
{
    [Export]
    private readonly PackedScene _bulletFactory = null!;

    /// <summary>
    /// Spawn a <c>Bullet</c>
    /// </summary>
    public void SpawnBullet(
      Vector2 position,
      Vector2 direction,
      TeamName teamName
    )
    {
        var bulletInstance = _bulletFactory.Instance() as Bullet;
        bulletInstance!.Initialize(
          position,
          direction,
          teamName
        );
        AddChild(bulletInstance);
    }
}
