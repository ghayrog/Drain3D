using System;
using UnityEngine;
using Game;
using Utilities;
using System.Collections.Generic;
using DI;

namespace GameTimeScale
{

    public delegate void OnEndTimerHandler();

    [Serializable]
    public sealed class GameTime :
        IGameStartListener, IGameUpdateListener, IGamePauseListener, IGameResumeListener, IGameFinishListener
    {
        public LoadingPriority Priority => LoadingPriority.Low;

        [SerializeField]
        private int _initialYear;

        [SerializeField]
        private int _initialMonth;

        [SerializeField]
        private int _initialDay;

        private DateTime _initialDateTime;
        private DateTime _currentDateTime;

        public DateTime CurrentDateTime => _currentDateTime;

        [SerializeField]
        private string _currentDateString;

        public float ElapsedDays => (float)(_currentDateTime - _initialDateTime).TotalDays;

        private float[] _timeScaleMultipliers = {1, 60, 60*60, 60*60*24, 60*60*24*30.5f};

        [SerializeField]
        private int _timeScaleMode = 0;

        private bool _isTimerActive = false;

        private List<GameTimer> _gameTimers;

        private GameManager _gameManager;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void OnGameStart()
        {
            _initialDateTime = new DateTime(_initialYear, _initialMonth, _initialDay);
            _currentDateTime = _initialDateTime;
            _gameTimers = new List<GameTimer>();
            _isTimerActive = true;
        }

        public void AddGameTimer(int days, OnEndTimerHandler handler)
        {
            var timer = new GameTimer(this, _currentDateTime.AddDays(days), handler, _gameManager);
            _gameTimers.Add(timer);
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_isTimerActive) return;
            _currentDateTime = _currentDateTime.AddMinutes(Time.deltaTime / 60 * _timeScaleMultipliers[_timeScaleMode]);
            _currentDateString = _currentDateTime.ToString("G");
            var timersCount = _gameTimers.Count;
            var currentTimer = 0;
            for (int i = 0; i < timersCount; i++)
            {
                var checkTimer = _gameTimers[currentTimer].CheckTimer();
                if (checkTimer)
                {
                    _gameTimers.Remove(_gameTimers[currentTimer]);
                }
                else
                {
                    currentTimer++;
                }
            }
        }

        public void SetTimeScaleToSeconds()
        {
            _gameManager.ResumeGame();
            _timeScaleMode = 0;
        }

        public void SetTimeScaleToMinutes()
        {
            _gameManager.ResumeGame();
            _timeScaleMode = 1;
        }

        public void SetTimeScaleToHours()
        {
            _gameManager.ResumeGame();
            _timeScaleMode = 2;
        }

        public void SetTimeScaleToDays()
        {
            _gameManager.ResumeGame();
            _timeScaleMode = 3;
        }

        public void SetTimeScaleToMonths()
        {
            _gameManager.ResumeGame();
            _timeScaleMode = 4;
        }


        public void OnGamePause()
        {
            _isTimerActive = false;
        }

        public void OnGameResume()
        {
            _isTimerActive = true;
        }

        public void OnGameFinish()
        {
            _isTimerActive = false;
        }
    }
}
