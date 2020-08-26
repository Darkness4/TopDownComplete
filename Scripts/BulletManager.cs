using Godot;

/// <summary>
/// Manages all in-game bullet.
/// </summary>
public class BulletManager : Node2D
{
    [Export]
    private PackedScene _bullet = null!;

    /// <summary>
    /// Spawn a <c>Bullet</c>
    /// </summary>
    public void SpawnBullet(
      Vector2 position,
      Vector2 direction,
      TeamName teamName
    )
    {
        var bulletInstance = (Bullet)_bullet!.Instance();
        bulletInstance.Initialize(
          position,
          direction,
          teamName
        );
        AddChild(bulletInstance);
    }
}
