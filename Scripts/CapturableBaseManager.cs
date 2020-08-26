using Godot;

public class CapturableBaseManager : Node
{

    [Signal]
    public delegate void BaseCaptured(TeamName teamName);

    public override void _Ready()
    {
        var currentBase = GetChildren().GetEnumerator();

        do
        {
            var node = currentBase.Current as CapturableBase;

            if (node != null)
            {
                node.Connect(nameof(CapturableBase.BaseCaptured), this, nameof(HandleBaseCaptured));
            }
        } while (currentBase.MoveNext());
    }

    private void HandleBaseCaptured(TeamName teamName)
    {
        EmitSignal(nameof(BaseCaptured), teamName);
    }
}
