using Godot;
using System;

public class GlobalSignals : Node
{
    private static readonly Lazy<GlobalSignals> _instance = new Lazy<GlobalSignals>(() => new GlobalSignals());

    public static GlobalSignals Instance { get => _instance.Value; }

    private GlobalSignals() { }

    /// <summary>
    /// A bullet has been fired with the following parameters.
    /// </summary>
    [Signal]
    public delegate void BulletFired(
        Vector2 position,
        Vector2 direction,
        TeamName teamName = TeamName.UNDEFINED
    );
}
