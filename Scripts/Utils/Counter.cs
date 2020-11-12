using Godot;

/// <summary>
/// A <c>Node2D</c> <c>Counter</c> class equipped with signals.
/// </summary>
public abstract class Counter : Node
{
    [Export]
    private int _value = 2;

    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            EmitSignal(nameof(Changed));
            if (_value == 0)
            {
                EmitSignal(nameof(IsZero));
            }
        }
    }

    [Signal]
    public delegate void IsZero();

    [Signal]
    public delegate void Changed();

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

    public override bool Equals(object obj)
    {
        return Equals(obj as Counter);
    }

    public bool Equals(Counter? other)
    {
        return other != null && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return this.Value.GetHashCode();
    }
}