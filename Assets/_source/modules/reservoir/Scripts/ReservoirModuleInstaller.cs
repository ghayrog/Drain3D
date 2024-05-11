using UnityEngine;
using Game;
using DI;

namespace Reservoir
{
    public sealed class ReservoirModuleInstaller : GameModuleInstaller
    {
        [SerializeField,Listener,Service(typeof(ReservoirManager))]
        private ReservoirManager _reservoirManager;

        [SerializeField,Listener,Service(typeof(ReservoirClicker))]
        private ReservoirClicker _reservoirClicker;
    }
}
