using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    [SerializeField] private Text topScores;
    [SerializeField] private Text currentScores;
    [SerializeField] private GameObject pauseUi;
    [SerializeField] private DragController drags;

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

        _topScores = PlayerPrefs.GetInt(nameof(_topScores), 0);
        topScores.text = $"Top Scores: {_topScores}";

        Restart();
        SetPause(!_isPaused);


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
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyUp(KeyCode.W))
            _direction = Vector2.up;
        else if (Input.GetKeyUp(KeyCode.S))
            _direction = Vector2.down;
        else if (Input.GetKeyUp(KeyCode.A))
            _direction = Vector2.left;
        else if (Input.GetKeyUp(KeyCode.D))
            _direction = Vector2.right;

        else if (Input.GetKeyUp(KeyCode.Space))
            SetPause(!_isPaused);

        else if (Input.GetKeyUp(KeyCode.R))
            Restart();
        #elif UNITY_IOS || UNITY_ANDROID
            _direction = drags.Direction;
        #endif
    }

    public void StartButtonClicked()
    {
        SetPause(false);
        if (_isEnd)
            Restart();
    }

    private void Restart()
    {
        _board = new GameBoard(fieldWidth, fieldHeight, 3);
        gameBoardView.SetBoardState(_board.Field);
        _isPaused = false;
        _isEnd = false;
        _timePassed = 0;
        CheckScores();
        SetPause(_isPaused);
    }

    private void OnDestroy()
    {
        CheckScores();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            CheckScores();
    }

    private int prevScoresValue = -1;
    private void CheckScores()
    {
        if (_board.TimesScored > _topScores)
        {
            _topScores = _board.TimesScored;
            topScores.text = $"Top Scores: {_topScores}";
            PlayerPrefs.SetInt(nameof(_topScores), _topScores);
            PlayerPrefs.Save();
        }

        if (prevScoresValue < _board.TimesScored)
        {
            currentScores.text = $"Scores: {_board.TimesScored}";
            prevScoresValue = _board.TimesScored;
        }

    }

    private void SetPause(bool isPaused)
    {
        _isPaused = isPaused;
        pauseUi.SetActive(_isPaused);
    }

    private void PerformGameTick()
    {
        OnGlobalGameTick?.Invoke();
        var moved = _board.MoveSnake(_direction);
        gameBoardView.SetBoardState(_board.Field);
        CheckScores();

        if (!moved)
        {
            _isEnd = true;
            SetPause(_isEnd);
        }
    }
}
