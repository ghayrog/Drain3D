using UnityEngine;
using DI;

namespace Game
{
    public sealed class GameContext : DependencyInstaller
    {
        [SerializeField,Service(typeof(GameManager))]
        private GameManager _gameManager;

        [SerializeField]
        private GameModuleInstaller[] _gameModules;

        private DIContext _gameContext;

        private void Awake()
        {
            _gameContext = new DIContext();
            _gameContext.AddModule(this);
            foreach (var module in _gameModules)
            {
                _gameContext.AddModule(module);
            }
            _gameContext.Initialize(gameObject.scene);

            AddListenersToGameManager();
            _gameManager.InitializeGame();
        }

        private void AddListenersToGameManager()
        {
            foreach (var module in _gameModules)
            {
                if (module is IGameListenerProvider listenersProvider)
                {
                    Debug.Log($"Adding multiple listeners from module {module} as a custom Listener Provider");
                    _gameManager.AddMultipleListeners(listenersProvider.ProvideListeners());
                }
                if (module is IGameListener tListener)
                {
                    Debug.Log($"Adding single listener from module {module}");
                    _gameManager.AddListener(tListener);
                }
            }

        }

    }
}
