using Godot;
using System;

public partial class GlobalSignals : Node
{
    private static readonly Lazy<GlobalSignals> _instance = new(() => new GlobalSignals());

    /// <summary>
    /// Get a lazy instance of <c>GlobalSignals</c>.
    /// </summary>
    public static GlobalSignals Instance { get => _instance.Value; }

    private GlobalSignals() { }

    /// <summary>
    /// A bullet has been fired with the following parameters.
    /// </summary>
    [Signal]
    public delegate void BulletFiredEventHandler(
        Vector2 position,
        Vector2 direction,
        TeamName teamName = TeamName.UNDEFINED
    );
}
