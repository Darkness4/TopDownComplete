using Godot;

public partial class SpawnPoint : Marker2D
{
    private int _bodyCount = 0;

    private void OnAntiCollisionAreaBodyEntered(Node2D body)
    {
        _bodyCount++;
    }

    private void OnAntiCollisionAreaBodyExited(Node2D body)
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
