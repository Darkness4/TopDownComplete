using Godot;

/// <summary>
/// A <c>Node2D</c> <c>Counter</c> class equipped with signals.
/// </summary>
public abstract class Counter : Node2D
{
    [Export]
    private int _value = 2;

    public int Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            if (_value == 0)
            {
                EmitSignal(nameof(IsZero));
            }
            EmitSignal(nameof(Changed));
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