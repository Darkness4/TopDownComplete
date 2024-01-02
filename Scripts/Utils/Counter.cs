using Godot;

/// <summary>
/// A <c>Node2D</c> <c>Counter</c> class equipped with signals.
/// </summary>
public partial class Counter : Node
{
    [Export]
    private int _value = 2;

    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            EmitSignal(SignalName.Changed);
            if (_value == 0)
            {
                EmitSignal(SignalName.IsZero);
            }
        }
    }

    [Signal]
    public delegate void IsZeroEventHandler();

    [Signal]
    public delegate void ChangedEventHandler();

    /// <summary>
    /// Decreases the <c>Counter</c>.
    /// </summary>
    public void Decrease()
    {
        Value--;
    }

    /// <summary>
    /// Increases the <c>Counter</c>.
    /// </summary>
    public void Increase()
    {
        Value++;
    }

    public override string ToString()
    {
        return $"Counter: {Value}";
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Counter);
    }

    public bool Equals(Counter? other)
    {
        return other != null && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
