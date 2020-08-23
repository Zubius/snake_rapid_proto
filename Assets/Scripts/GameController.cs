using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;

public class GameController : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameTickLengthInSec = 1f;
    [SerializeField] private int fieldWidth;
    [SerializeField] private int fieldHeight;
    [SerializeField] private float distance = 0.1f;

    [Space]
    [Header("References")]
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject cube;
    [SerializeField] private GameBoardView gameBoardView;

    internal event Action OnGlobalGameTick;
    internal static GameController instance;

    private float _timePassed = 0;
    private bool _isPaused = false;
    private bool _isEnd = false;
    private GameBoard _board;
    private Vector2 _direction = Vector2.left;
    private int _topScores = 0;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        var size = cube.GetComponent<Renderer>().bounds.size.x;
        gameBoardView.InitView(cube, fieldWidth, fieldHeight, size + distance);

        _topScores = PlayerPrefs.GetInt(nameof(_board.TimesScored), 0);

        Restart();

        if (camera.orthographic)
        {
            var width = fieldWidth * size + distance * (fieldHeight - 1) + size;//leave size for a bit gap at screen sides

            camera.orthographicSize = Mathf.Max(5, width / 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        if (!_isPaused && !_isEnd)
        {
            _timePassed += Time.deltaTime;
            if (_timePassed >= gameTickLengthInSec)
            {
                _timePassed = 0;
                PerformGameTick();
            }
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyUp(KeyCode.W))
            _direction = Vector2.up;
        else if (Input.GetKeyUp(KeyCode.S))
            _direction = Vector2.down;
        else if (Input.GetKeyUp(KeyCode.A))
            _direction = Vector2.left;
        else if (Input.GetKeyUp(KeyCode.D))
            _direction = Vector2.right;

        else if (Input.GetKeyUp(KeyCode.Escape))
            SetPause(!_isPaused);

        else if (Input.GetKeyUp(KeyCode.R))
            Restart();
    }

    private void Restart()
    {
        _board = new GameBoard(fieldWidth, fieldHeight, 3);
        gameBoardView.SetBoardState(_board.Field);
        _isPaused = false;
        _timePassed = 0;
    }

    private void OnDestroy()
    {
        SaveTopScores();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveTopScores();
    }

    private void SaveTopScores()
    {
        if (_board.TimesScored > _topScores)
        {
            PlayerPrefs.SetInt(nameof(_board.TimesScored), _board.TimesScored);
            PlayerPrefs.Save();
        }
    }

    private void SetPause(bool isPaused)
    {
        _isPaused = isPaused;

    }

    private void PerformGameTick()
    {
        OnGlobalGameTick?.Invoke();
        var moved = _board.MoveSnake(_direction);
        gameBoardView.SetBoardState(_board.Field);

        if (!moved) _isEnd = true;
    }
}
