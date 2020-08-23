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
    private GameBoard _board;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        var size = cube.GetComponent<Renderer>().bounds.size.x;
        _board = new GameBoard(fieldWidth, fieldHeight, 3);

        gameBoardView.InitView(cube, fieldWidth, fieldHeight, size + distance);
        gameBoardView.SetBoardState(_board.Field);

        if (camera.orthographic)
        {
            var width = fieldWidth * size + distance * (fieldHeight - 1) + size;//leave size for a bit gap at screen sides

            camera.orthographicSize = Mathf.Max(5, width / 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isPaused)
        {
            _timePassed += Time.deltaTime;
            if (_timePassed >= gameTickLengthInSec)
            {
                _timePassed = 0;
                PerformGameTick();
            }
        }
    }

    private void PerformGameTick()
    {
        OnGlobalGameTick?.Invoke();
        gameBoardView.SetBoardState(_board.Field);
    }
}
