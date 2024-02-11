using UnityEngine;
using DI;
using Game;

namespace CameraAxis
{
    internal sealed class CameraAxisMovement:
        IGameStartListener, IGameFinishListener, IGamePauseListener, IGameResumeListener,
        IGameFixedUpdateListener
    {
        private CameraInput _cameraInput;
        private CameraMovementComponent _cameraMovement;
        private bool _isActive = false;

        public LoadingPriority Priority => LoadingPriority.Low;

        public void OnGameStart()
        {
            _isActive = true;
        }

        public void OnGamePause()
        {
            _isActive = false;
        }

        public void OnGameResume()
        {
            _isActive = true;
        }

        public void OnGameFinish()
        {
            _isActive = false;
        }

        [Inject]
        internal void Construct(CameraInput cameraInput, CameraMovementComponent cameraMovement)
        {
            Debug.Log("Construct: Camera Axis Movement");
            _cameraInput = cameraInput;
            _cameraMovement = cameraMovement;
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
            if (!_isActive) return;
            if (_cameraInput.RotationInput != 0)
            { 
                _cameraMovement.RotateCameraAxis(_cameraInput.RotationInput * fixedDeltaTime);
            }

            if (_cameraInput.ElevationInput != 0)
            { 
                _cameraMovement.MoveCameraAxis(_cameraInput.ElevationInput * fixedDeltaTime);
            }

            if (_cameraInput.ZoomInput != 0)
            {
                _cameraMovement.ZoomCamera(_cameraInput.ZoomInput * fixedDeltaTime);
            }
        }
    }
}
