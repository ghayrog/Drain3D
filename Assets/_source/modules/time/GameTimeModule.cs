using DI;
using Game;
using UnityEngine;

namespace GameTimeScale
{
    internal class GameTimeModule : GameModuleInstaller
    {
        [SerializeField,Listener, Service(typeof(GameTime))]
        private GameTime _gameTime;

        [SerializeField,Listener]
        private GameTimeGUI _gameTimeGUI;
    }
}
