using Godot;

public class CapturableBaseManager : Node
{

    [Signal]
    public delegate void OnBaseCaptured();

    public override void _Ready()
    {
        foreach (var node in GetChildren())
        {
            if (node is CapturableBase currentBase)
            {
                currentBase.Connect(nameof(CapturableBase.OnBaseCaptured), this, nameof(HandleBaseCaptured));
            }
        }
    }

    private void HandleBaseCaptured(TeamName _)
    {
        EmitSignal(nameof(OnBaseCaptured));
    }
}
