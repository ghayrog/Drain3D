using System.Collections.Generic;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;
using Utilities;
using DI;

namespace Game
{
    public sealed class GameManager : MonoBehaviour
    {
        [ReadOnlyInspector,SerializeField]
        private GameState _gameState;

        private readonly List<IGameStartListener> gameStartListeners = new();
        private readonly List<IGameFinishListener> gameFinishListeners = new();
        private readonly List<IGamePauseListener> gamePauseListeners = new();
        private readonly List<IGameResumeListener> gameResumeListeners = new();

        private readonly List<IGameInitializeListener> gameInitializeListeners = new();
        private readonly List<IGameUpdateListener> gameUpdateListeners = new();
        private readonly List<IGameLateUpdateListener> gameLateUpdateListeners = new();
        private readonly List<IGameFixedUpdateListener> gameFixedUpdateListeners = new();

        internal void InitializeGame()
        {
            if (_gameState != GameState.None) return;
            for (var i = 0; i < gameInitializeListeners.Count; i++)
            {
                gameInitializeListeners[i].OnGameInitialize();
            }
        }

        private void FixedUpdate()
        {
            if (_gameState != GameState.Playing) return;
            for (var i = 0; i < gameFixedUpdateListeners.Count; i++)
            {
                gameFixedUpdateListeners[i].OnFixedUpdate(Time.fixedDeltaTime);
            }
        }

        private void Update()
        {
            if (_gameState != GameState.Playing) return;
            for (var i = 0; i < gameUpdateListeners.Count; i++)
            {
                gameUpdateListeners[i].OnUpdate(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (_gameState != GameState.Playing) return;
            for (var i = 0; i < gameLateUpdateListeners.Count; i++)
            {
                gameLateUpdateListeners[i].OnLateUpdate(Time.deltaTime);
            }
        }

        internal void AddMultipleListeners(IEnumerable<IGameListener> listeners)
        {
            foreach (var listener in listeners)
            {
                AddListener(listener);
            }
        }

        internal void AddListener(IGameListener listener)
        {
            if (listener is IGameInitializeListener awakeListener)
            {
                gameInitializeListeners.Add(awakeListener);
                gameInitializeListeners.Sort(
                    (IGameInitializeListener listener1, IGameInitializeListener listener2) =>
                    {
                        return (int)Mathf.Sign(listener1.Priority - listener2.Priority);
                    }
                    );
            }

            if (listener is IGameStartListener startListener)
            {
                gameStartListeners.Add(startListener);
                gameStartListeners.Sort(
                    (IGameStartListener listener1, IGameStartListener listener2) =>
                    {
                        return (int)Mathf.Sign(listener1.Priority - listener2.Priority);
                    }
                    );
            }

            if (listener is IGameFinishListener finishListener)
            {
                gameFinishListeners.Add(finishListener);
                gameFinishListeners.Sort(
                    (IGameFinishListener listener1, IGameFinishListener listener2) =>
                    {
                        return (int)Mathf.Sign(listener1.Priority - listener2.Priority);
                    }
                    );
            }

            if (listener is IGamePauseListener pauseListener)
            {
                gamePauseListeners.Add(pauseListener);
                gamePauseListeners.Sort(
                    (IGamePauseListener listener1, IGamePauseListener listener2) =>
                    {
                        return (int)Mathf.Sign(listener1.Priority - listener2.Priority);
                    }
                    );
            }

            if (listener is IGameResumeListener resumeListener)
            {
                gameResumeListeners.Add(resumeListener);
                gameResumeListeners.Sort(
                    (IGameResumeListener listener1, IGameResumeListener listener2) =>
                    {
                        return (int)Mathf.Sign(listener1.Priority - listener2.Priority);
                    }
                    );
            }

            if (listener is IGameUpdateListener updateListener)
            {
                gameUpdateListeners.Add(updateListener);
                gameUpdateListeners.Sort(
                    (IGameUpdateListener listener1, IGameUpdateListener listener2) =>
                    {
                        return (int)Mathf.Sign(listener1.Priority - listener2.Priority);
                    }
                    );
            }

            if (listener is IGameFixedUpdateListener fixedUpdateListener)
            {
                gameFixedUpdateListeners.Add(fixedUpdateListener);
                gameFixedUpdateListeners.Sort(
                    (IGameFixedUpdateListener listener1, IGameFixedUpdateListener listener2) =>
                    {
                        return (int)Mathf.Sign(listener1.Priority - listener2.Priority);
                    }
                    );
            }

            if (listener is IGameLateUpdateListener lateUpdateListener)
            {
                gameLateUpdateListeners.Add(lateUpdateListener);
                gameLateUpdateListeners.Sort(
                    (IGameLateUpdateListener listener1, IGameLateUpdateListener listener2) =>
                    {
                        return (int)Mathf.Sign(listener1.Priority - listener2.Priority);
                    }
                    );
            }
        }

        [ProPlayButton]
        public void StartGame()
        {
            bool isInitialized = true;

            for (int i = 0; i < gameInitializeListeners.Count; i++)
            {
                if (gameInitializeListeners[i].IsInitialized == false)
                { 
                    isInitialized = false;
                    break;
                }
            }
            if (!isInitialized)
            {
                Debug.Log("Cannot start game before all modules are initialized");
                return;
            }

            if (_gameState != GameState.None && _gameState != GameState.Finished) return;
            for (var i = 0; i < gameStartListeners.Count; i++)
            {
                gameStartListeners[i].OnGameStart();
            }
            _gameState = GameState.Playing;
        }

        [ProPlayButton]
        public void PauseGame()
        {
            if (_gameState != GameState.Playing) return;
            for (var i = 0; i < gamePauseListeners.Count; i++)
            {
                gamePauseListeners[i].OnGamePause();
            }
            _gameState = GameState.Paused;
        }

        [ProPlayButton]
        public void ResumeGame()
        {
            if (_gameState != GameState.Paused) return;
            for (var i = 0; i < gameResumeListeners.Count; i++)
            {
                gameResumeListeners[i].OnGameResume();
            }
            _gameState = GameState.Playing;
        }

        [ProPlayButton]
        public void FinishGame()
        {
            if (_gameState == GameState.None || _gameState == GameState.Finished) return;
            for (var i = 0; i < gameFinishListeners.Count; i++)
            {
                gameFinishListeners[i].OnGameFinish();
            }
            _gameState = GameState.Finished;
        }
    }
}