
using UnityEngine;

internal struct GameCell
{
    internal Vector2 Position;
    internal GameCellType Type;

    internal GameCell(Vector2 position, GameCellType type)
    {
        Position = position;
        Type = type;
    }
}
