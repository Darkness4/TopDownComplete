using Godot;

public class SpawnPoint : Position2D
{
    private int _bodyCount = 0;

    private void _on_AntiCollisionArea_body_entered(Node body)
    {
        _bodyCount++;
    }

    private void _on_AntiCollisionArea_body_exited(Node body)
    {
        _bodyCount--;
    }

    public bool IsFree
    {
        get => _bodyCount == 0;
    }

    public bool IsOccupied
    {
        get => _bodyCount != 0;
    }
}
