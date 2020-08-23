
using UnityEngine;

internal class GameBoard
{
    internal GameCell[,] Field;

    internal GameBoard(int width, int height, int initSnakeSize)
    {
        Field = new GameCell[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Field[i, j] = new GameCell(new Vector2(i, j), GameCellType.Empty);
            }
        }
    }
}
