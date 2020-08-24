
using System;
using System.Linq;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

internal class GameBoard
{
    private readonly GameCell[] _field;

    internal GameCell[] Field => _field;

    internal GameCell this[int x] => _field[x];

    internal GameCell this[int x, int y]
    {
        get => _field[MapIndex(x, y)];
        set => _field[MapIndex(x, y)] = value;
    }

    internal GameCell this[float x, float y]
    {
        get => this[(int) x, (int) y];
        set => this[(int) x, (int) y] = value;
    }

    internal GameCell this[Vector2 pos]
    {
        get => this[(int) pos.x, (int) pos.y];
        set => this[(int) pos.x, (int) pos.y] = value;
    }

    internal int TimesScored { get; private set; }

    private readonly Snake _snake;
    private Vector2 _snakeDirection = Vector2.zero;
    private Vector2 _targetPosition;
    private readonly int _width;
    private readonly int _height;
    private bool _passThroughBorder = false;

    internal GameBoard(int width, int height, int initSnakeSize, bool passThroughBorder = false)
    {
        _width = width;
        _height = height;
        _field = new GameCell[width * height];
        TimesScored = 0;
        _passThroughBorder = passThroughBorder;

        Vector2[] snakeCells = GetSnakeInitCells(width, height, initSnakeSize);
        _snake = new Snake(snakeCells);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var pos = new Vector2(i, j);

                this[i, j] = new GameCell(new Vector2(i, j), Array.IndexOf(snakeCells, pos) > -1 ? GameCellType.Snake : GameCellType.Empty);
            }
        }

        PlaceTargetOnBoard();
    }

    internal bool MoveSnake(Vector2 direction)
    {
        var dir = direction + _snakeDirection == Vector2.zero ? _snakeDirection : direction;

        var nextCellPos = _snake.Head + dir;
        
        if (_passThroughBorder)
        {
            if (nextCellPos.x < 0) nextCellPos = new Vector2(_width - 1, nextCellPos.y);
            if (nextCellPos.x >= _width) nextCellPos = new Vector2(0, nextCellPos.y);
            if (nextCellPos.y < 0) nextCellPos = new Vector2(nextCellPos.x, _height - 1);
            if (nextCellPos.y >= _height) nextCellPos = new Vector2(nextCellPos.x, 0);
        }
        else
        {
            if (nextCellPos.x < 0 || nextCellPos.x >= _width ||
                nextCellPos.y < 0 || nextCellPos.y >= _height)
            {
                return false;
            }
        }

        if (IsSnake(this[nextCellPos]))
        {
            return false;
        }

        var tail = _snake.Tail;

        _snake.MoveTo(this[nextCellPos]);

        if (_snake.Head == _targetPosition)
        {
            TimesScored++;
            PlaceTargetOnBoard();
        }

        _field[MapIndex(nextCellPos)].Type = GameCellType.Snake;

        if (IsSnake(this[tail]))
        {
            _field[MapIndex(tail)].Type = GameCellType.Empty;
        }

        _snakeDirection = dir;
        return true;
    }

    private int MapIndex(int x, int y)
    {
        return x * _width + y;
    }

    private int MapIndex(float x, float y)
    {
        return MapIndex((int)x, (int)y);
    }

    private int MapIndex(Vector2 pos)
    {
        return MapIndex((int)pos.x, (int)pos.y);
    }

    private bool IsSnake(GameCell cell)
    {
        return cell.Type == GameCellType.Snake;
    }

    private void PlaceTargetOnBoard()
    {
        var targetPos = GetTargetPosition();
        _field[MapIndex(targetPos)].Type = GameCellType.Target;
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
        var empties = _field.Where(c => c.Type == GameCellType.Empty &&
                                                       (_snake.Head - c.Position).sqrMagnitude > 2).ToList();

        var target = empties[Random.Range(0, empties.Count)].Position;
        return target;
    }
}
