using UnityEngine;
using DI;
using Game;

namespace CameraAxis
{
    internal sealed class CameraAxisModule : GameModuleInstaller
    {
        public LoadingPriority Priority => LoadingPriority.Low;

        [Service(typeof(Camera)),SerializeField]
        private Camera _camera;

        [Service(typeof(CameraMovementComponent)),SerializeField,Listener]
        private CameraMovementComponent _movementComponent;

        [Service(typeof(CameraInput)),Listener]
        private CameraInput _cameraInput = new();

        [Listener]
        private CameraAxisMovement _cameraAxisMovement = new();
    }
}
