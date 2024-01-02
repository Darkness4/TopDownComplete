using Godot;

public partial class CapturableBaseManager : Node
{

    [Signal]
    public delegate void OnBaseCapturedEventHandler();

    public override void _Ready()
    {
        foreach (var node in GetChildren())
        {
            if (node is CapturableBase currentBase)
            {
                currentBase.Connect(CapturableBase.SignalName.OnBaseCaptured, new Callable(this, MethodName.HandleBaseCaptured));
            }
        }
    }

    private void HandleBaseCaptured(TeamName _)
    {
        EmitSignal(SignalName.OnBaseCaptured);
    }
}
