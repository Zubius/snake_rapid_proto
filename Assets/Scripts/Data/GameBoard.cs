
using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

internal class GameBoard
{
    internal readonly GameCell[,] Field;
    private Snake _snake;

    internal GameBoard(int width, int height, int initSnakeSize)
    {
        Field = new GameCell[width, height];

        Vector2[] snakeCells = GetSnakeInitCells(width, height, initSnakeSize);
        _snake = new Snake(snakeCells);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var pos = new Vector2(i, j);

                Field[i, j] = new GameCell(new Vector2(i, j), Array.IndexOf(snakeCells, pos) > -1 ? GameCellType.Snake : GameCellType.Empty);
            }
        }

        SetTargetOnBoard();
    }

    internal bool MoveSnake(Vector2 direction)
    {
        var nextCellPos = _snake.Head + direction;
        if (nextCellPos.x < 0 || nextCellPos.x >= Field.GetLength(0) ||
            nextCellPos.y < 0 || nextCellPos.y >= Field.GetLength(1))
        {
            return false;
        }

        var tail = _snake.Tail;

        _snake.MoveTo(Field[(int)nextCellPos.x, (int)nextCellPos.y]);

        Field[(int) nextCellPos.x, (int) nextCellPos.y].Type = GameCellType.Snake;

        if (!_snake.IsSnake(tail))
        {
            Field[(int) tail.x, (int) tail.y].Type = GameCellType.Empty;
        }
        else
        {
            return false;
        }

        return true;
    }


    private void SetTargetOnBoard()
    {
        var targetPos = GetTargetPosition();
        Field[(int) targetPos.x, (int) targetPos.y].Type = GameCellType.Target;
    }

    private Vector2[] GetSnakeInitCells(int width, int height, int initSnakeSize)
    {
        //Place snake in center horizontaly
        var snake = new Vector2[initSnakeSize];
        var center = new Vector2(width / 2, height / 2);
        var snakeCenter = initSnakeSize / 2;
        var left = snakeCenter - initSnakeSize;
        var right = initSnakeSize + left;
        var snakeIndx = 0;

        for (int i = left; i < right; i++)
        {
            snake[snakeIndx++] = new Vector2(center.x + i, center.y);
        }

        return snake;
    }

    private Vector2 GetTargetPosition()
    {
        return Vector2.zero;
    }
}
