using Game;
using UnityEngine;

namespace CameraAxis
{
    internal sealed class CameraInput :
        IGameUpdateListener
    {
        private const string HORIZONTAL = "Horizontal";
        private const string VERTICAL = "Vertical";
        private const string ZOOM = "Zoom";

        internal float RotationInput { get; private set; }
        internal float ElevationInput { get; private set; }
        internal float ZoomInput { get; private set; }

        public LoadingPriority Priority => LoadingPriority.Low;

        public void OnUpdate(float deltaTime)
        {
            RotationInput = Input.GetAxisRaw(HORIZONTAL);
            ElevationInput= Input.GetAxisRaw(ZOOM);
            ZoomInput = Input.GetAxisRaw(VERTICAL);
        }
    }
}
