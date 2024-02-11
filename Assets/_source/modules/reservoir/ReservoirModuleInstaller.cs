using UnityEngine;
using Game;

namespace Reservoir
{
    public sealed class ReservoirModuleInstaller : GameModuleInstaller
    {
        [SerializeField,Listener]
        private ReservoirManager _reservoirManager;
    }
}
