using DI;
using Game;
using UnityEngine;

namespace Land
{
    internal sealed class LandModuleInstaller : GameModuleInstaller
    {
        [SerializeField, Listener, Service(typeof(LandManager))]
        private LandManager _landManager;

        [SerializeField, Listener]
        private LandClicker _landClicker;

        [SerializeField, Service(typeof(LandExplorer))]
        private LandExplorer _landExplorer;
    }
}
