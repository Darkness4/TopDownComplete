public enum TeamName
{
    UNDEFINED,
    PLAYER,
    ENEMY
}

public interface ITeamed
{
    TeamName TeamName { get; }
}
