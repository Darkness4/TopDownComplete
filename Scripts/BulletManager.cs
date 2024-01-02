using Godot;

/// <summary>
/// Manages all in-game bullet.
/// </summary>
public partial class BulletManager : Node
{
    [Export]
    private PackedScene _bulletFactory = null!;

    /// <summary>
    /// Spawn a <c>Bullet</c>
    /// </summary>
    public void SpawnBullet(
      Vector2 position,
      Vector2 direction,
      TeamName teamName
    )
    {
        var bulletInstance = _bulletFactory.Instantiate<Bullet>();
        bulletInstance!.Initialize(
          position,
          direction,
          teamName
        );
        AddChild(bulletInstance);
    }
}
