
using UnityEngine;

public struct GameCell
{
    public Vector2 Position;
    public GameCellType Type;

    public GameCell(Vector2 position, GameCellType type)
    {
        Position = position;
        Type = type;
    }

    public void SetPosition(Vector2 position)
    {
        Position = position;
    }
}
