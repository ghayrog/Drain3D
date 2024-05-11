using DI;
using Game;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameTimeScale
{
    [Serializable]
    internal sealed class GameTimeGUI: IGameStartListener, IGameFinishListener, IGamePauseListener, IGameResumeListener, IGameUpdateListener
    {
        private GameTime _gameTime;
        private GameManager _gameManager;

        [SerializeField]
        private Text _dateText;

        [SerializeField]
        private Text _timeText;

        [SerializeField]
        private Button _pauseButton;

        [SerializeField]
        private Button _speed0Button;

        [SerializeField]
        private Button _speed1Button;

        [SerializeField]
        private Button _speed2Button;

        [SerializeField]            
        private Button _speed3Button;

        [SerializeField]
        private Button _speed4Button;

        public LoadingPriority Priority => LoadingPriority.Low;

        [Inject]
        public void Construct(GameTime gameTime, GameManager gameManager)
        { 
            _gameTime = gameTime;
            _gameManager = gameManager;
        }

        public void OnGameFinish()
        {
            _pauseButton.onClick.RemoveListener(_gameManager.PauseGame);
            _speed0Button.onClick.RemoveListener(_gameTime.SetTimeScaleToSeconds);
            _speed1Button.onClick.RemoveListener(_gameTime.SetTimeScaleToMinutes);
            _speed2Button.onClick.RemoveListener(_gameTime.SetTimeScaleToHours);
            _speed3Button.onClick.RemoveListener(_gameTime.SetTimeScaleToDays);
            _speed4Button.onClick.RemoveListener(_gameTime.SetTimeScaleToMonths);

        }

        public void OnGamePause()
        {
        }

        public void OnGameResume()
        {
        }

        public void OnGameStart()
        {
            _pauseButton.onClick.AddListener(_gameManager.PauseGame);
            _speed0Button.onClick.AddListener(_gameTime.SetTimeScaleToSeconds);
            _speed1Button.onClick.AddListener(_gameTime.SetTimeScaleToMinutes);
            _speed2Button.onClick.AddListener(_gameTime.SetTimeScaleToHours);
            _speed3Button.onClick.AddListener(_gameTime.SetTimeScaleToDays);
            _speed4Button.onClick.AddListener(_gameTime.SetTimeScaleToMonths);
        }

        public void OnUpdate(float deltaTime)
        {
            _dateText.text = _gameTime.CurrentDateTime.ToString("D");
            _timeText.text = _gameTime.CurrentDateTime.ToString("T");
        }
    }
}
