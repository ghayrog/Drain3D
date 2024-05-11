using Game;
using System;

namespace GameTimeScale
{
    internal sealed class GameTimer
    {
        internal event OnEndTimerHandler OnEndTimer;

        private GameTime _gameTime;
        private DateTime _endTime;
        private bool _isTimerElapsed = false;
        private OnEndTimerHandler _endTimeHandler;

        public bool IsTimerElapsed => _isTimerElapsed;

        internal GameTimer(GameTime gameTime, DateTime endTime, OnEndTimerHandler endTimeHandler, GameManager gameManager)
        {
            _gameTime = gameTime;
            _endTime = endTime;
            _endTimeHandler = endTimeHandler;
            OnEndTimer += _endTimeHandler;
        }

        internal bool CheckTimer()
        {
            if (_isTimerElapsed) return false;
            if (_gameTime.CurrentDateTime < _endTime) return false;
            _isTimerElapsed = true;
            OnEndTimer?.Invoke();
            OnEndTimer -= _endTimeHandler;
            return true;
        }
    }
}
