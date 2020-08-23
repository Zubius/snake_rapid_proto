
using System;
using System.Linq;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

internal class GameBoard
{
    internal readonly GameCell[,] Field;
    internal int TimesScored { get; private set; }

    private readonly Snake _snake;
    private Vector2 _snakeDirection = Vector2.zero;
    private Vector2 _targetPosition;

    internal GameBoard(int width, int height, int initSnakeSize)
    {
        Field = new GameCell[width, height];
        TimesScored = 0;

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

        PlaceTargetOnBoard();
    }

    internal bool MoveSnake(Vector2 direction)
    {
        var dir = direction + _snakeDirection == Vector2.zero ? _snakeDirection : direction;

        var nextCellPos = _snake.Head + dir;
        if (nextCellPos.x < 0 || nextCellPos.x >= Field.GetLength(0) ||
            nextCellPos.y < 0 || nextCellPos.y >= Field.GetLength(1) ||
            _snake.IsSnake(nextCellPos))
        {
            return false;
        }

        var tail = _snake.Tail;

        _snake.MoveTo(Field[(int)nextCellPos.x, (int)nextCellPos.y]);

        if (_snake.Head == _targetPosition)
        {
            TimesScored++;
            PlaceTargetOnBoard();
        }

        Field[(int) nextCellPos.x, (int) nextCellPos.y].Type = GameCellType.Snake;

        if (!_snake.IsSnake(tail))
        {
            Field[(int) tail.x, (int) tail.y].Type = GameCellType.Empty;
        }

        _snakeDirection = dir;
        return true;
    }


    private void PlaceTargetOnBoard()
    {
        var targetPos = GetTargetPosition();
        Field[(int) targetPos.x, (int) targetPos.y].Type = GameCellType.Target;
        _targetPosition = targetPos;
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
        var empties = Field.Cast<GameCell>().Where(c => c.Type == GameCellType.Empty &&
                                                       (_snake.Head - c.Position).sqrMagnitude > 2).ToList();

        var target = empties[Random.Range(0, empties.Count)].Position;
        return target;
    }
}
