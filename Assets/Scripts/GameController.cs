using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private float gameTickLengthInSec = 1f;
    [SerializeField] private int fieldWidth;
    [SerializeField] private int fieldHeight;

    internal event Action OnGlobalGameTick;
    internal static GameController instance;

    private float _timePassed = 0;
    private bool _isPaused = false;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

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
                OnGlobalGameTick?.Invoke();
            }
        }
    }
}
