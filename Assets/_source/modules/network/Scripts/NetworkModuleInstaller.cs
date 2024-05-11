using DI;
using Game;
using UnityEngine;

namespace Network
{
    public sealed class NetworkModuleInstaller : GameModuleInstaller
    {
        [SerializeField, Service(typeof(FacilitySystem))]
        private FacilitySystem _facilitySystem;
    }
}
