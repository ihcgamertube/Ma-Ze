using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeRender : MonoBehaviour
{
    [SerializeField]
    [Range(1, 50)]
    private int _width = 10;

    [SerializeField]
    [Range(1, 50)]
    private int _height = 10;

    [SerializeField]
    private Transform _wallPrefab = null;

    [SerializeField]
    private Transform _floorPrefab = null;

    [SerializeField]
    private Transform _player;

    [SerializeField]
    private Transform _winningPrefab;

    private float _wallWidth = 1f;
    private float _wallHeight = 1f;

    void Start()
    {
        _wallWidth = _wallPrefab.localScale.x;
        _wallHeight = _wallPrefab.localScale.y;
        var maze = Maze.Generate(_width, _height);
        Draw(maze);


        //var randomCell = maze[Random.Range(0, _width), Random.Range(0, _height)];
        _player.position = GetCellStartPosition(Random.Range(0, _width), Random.Range(0, _height));

        Cell _winningCell = null;
        if (Random.Range(0, 1) == 1)
        {
            var x = Random.Range(0, _width);
            //Row
            if (Random.Range(0, 1) == 1)
            {
                //Above
                _winningCell = maze[x, 0];
            }
            else
            {
                //Bottom
                _winningCell = maze[x, _height - 1];
            }
        }
        else
        {
            var y = Random.Range(0, _height);
            //Column
            if (Random.Range(0, 1) == 1)
            {
                //Left
                _winningCell = maze[0, y];
            }
            else
            {
                //Right
                _winningCell = maze[_width - 1, y];
            }
        }

        var winningElement = Instantiate(_winningPrefab, transform);
        winningElement.position = GetCellStartPosition(_winningCell.Row, _winningCell.Column);
    }

    private Vector3 GetCellStartPosition(int i, int j)
    {
        var position = new Vector3(
            -_width / 2 + i * _wallWidth,
            0,
            -_height / 2 + j * _wallWidth);
        return position;
    }

    private void Draw(Cell[,] maze)
    {
        //var floor = Instantiate(_floorPrefab, transform);
        //floor.localScale = new Vector3(_width, 1, _height);

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                var cell = maze[i, j];
                var position = GetCellStartPosition(i, j) + new Vector3(0, _wallHeight / 2, 0);

                if (cell.Walls.HasFlag(WallState.Up))
                {
                    var topWall = Instantiate(_wallPrefab, transform) as Transform;
                    topWall.position = position + new Vector3(0, 0, _wallWidth / 2);
                    topWall.localScale = new Vector3(_wallWidth, topWall.localScale.y, topWall.localScale.z);
                }

                if (cell.Walls.HasFlag(WallState.Left))
                {
                    var leftWall = Instantiate(_wallPrefab, transform) as Transform;
                    leftWall.position = position + new Vector3(-_wallWidth / 2, 0, 0);
                    leftWall.localScale = new Vector3(_wallWidth, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }

                if (i == _width - 1)
                {
                    if (cell.Walls.HasFlag(WallState.Right))
                    {
                        var rightWall = Instantiate(_wallPrefab, transform) as Transform;
                        rightWall.position = position + new Vector3(+_wallWidth / 2, 0, 0);
                        rightWall.localScale = new Vector3(_wallWidth, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                if (j == 0)
                {
                    if (cell.Walls.HasFlag(WallState.Down))
                    {
                        var bottomWall = Instantiate(_wallPrefab, transform) as Transform;
                        bottomWall.position = position + new Vector3(0, 0, -_wallWidth / 2);
                        bottomWall.localScale = new Vector3(_wallWidth, bottomWall.localScale.y, bottomWall.localScale.z);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
