using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum WallState
{
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8,
}

public class Cell
{
    public int Row;
    public int Column;
    public bool Visited;
    public WallState Walls;
}

public class Neighbour
{
    public Cell Cell;
    public WallState SharedWall;
}


public class Maze
{
    private static readonly WallState AllWalls = WallState.Right | WallState.Left | WallState.Up | WallState.Down;

    public static Cell[,] Generate(int width, int height)
    {
        Cell[,] maze = new Cell[width, height];
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                maze[i, j] = new Cell()
                {
                    Row = i,
                    Column = j,
                    Visited = false,
                    Walls = AllWalls,
                };
            }
        }

        return ApplyRecursiveBacktracker(maze, width, height);
    }

    private static Cell[,] ApplyRecursiveBacktracker(Cell[,] maze, int width, int height)
    {
        var stack = new Stack<Cell>();
        var currentCell = maze[Random.Range(0, width), Random.Range(0, height)];
        currentCell.Visited = true;
        stack.Push(currentCell);

        while (stack.Count > 0)
        {
            currentCell = stack.Pop();
            var neighbours = GetUnvisitedNeighbours(currentCell, maze, width, height);

            if (neighbours.Length > 0)
            {
                stack.Push(currentCell);
                var randomNeighbour = neighbours[Random.Range(0, neighbours.Length)];

                maze[currentCell.Row, currentCell.Column].Walls &= ~randomNeighbour.SharedWall;
                maze[randomNeighbour.Cell.Row, randomNeighbour.Cell.Column].Walls &= ~GetOppositeWall(randomNeighbour.SharedWall);
                maze[randomNeighbour.Cell.Row, randomNeighbour.Cell.Column].Visited = true;

                stack.Push(randomNeighbour.Cell);
            }
        }

        return maze;
    }


    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.Right: return WallState.Left;
            case WallState.Left: return WallState.Right;
            case WallState.Up: return WallState.Down;
            case WallState.Down: return WallState.Up;
            default: return WallState.Left;
        }
    }

    private static Neighbour[] GetUnvisitedNeighbours(
        Cell cell,
        Cell[,] maze,
        int width,
        int height)
    {
        var row = cell.Row;
        var column = cell.Column;
        var list = new List<Neighbour>(4);

        //Left
        if (row > 0)
        {
            if (!maze[row - 1, column].Visited)
            {
                list.Add(new Neighbour()
                {
                    Cell = maze[row - 1, column],
                    SharedWall = WallState.Left,
                });
            }
        }

        //Down
        if (column > 0)
        {
            if (!maze[row, column - 1].Visited)
            {
                list.Add(new Neighbour()
                {
                    Cell = maze[row, column - 1],
                    SharedWall = WallState.Down,
                });
            }
        }

        //Up
        if (column < height - 1)
        {
            if (!maze[row, column + 1].Visited)
            {
                list.Add(new Neighbour()
                {
                    Cell = maze[row, column + 1],
                    SharedWall = WallState.Up,
                });
            }
        }

        //Right
        if (row < width - 1)
        {
            if (!maze[row + 1, column].Visited)
            {
                list.Add(new Neighbour()
                {
                    Cell = maze[row + 1, column],
                    SharedWall = WallState.Right,
                });
            }
        }

        return list.ToArray();
    }
}
