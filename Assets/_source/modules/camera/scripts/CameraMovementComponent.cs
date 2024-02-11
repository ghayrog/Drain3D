using DI;
using Game;
using UnityEngine;

namespace CameraAxis
{
    internal sealed class CameraMovementComponent : MonoBehaviour,
        IGameStartListener, IGameUpdateListener
    {
        [SerializeField]
        private float _rotationSpeed;

        [SerializeField]
        private float _elevationSpeed;

        [SerializeField]
        private AnimationCurve _animationCurve;

        [SerializeField]
        private float _zoomSpeed;
        
        [SerializeField]
        private float _minZoomDistance;

        [SerializeField]
        private float _maxZoomDistance;

        [SerializeField]
        private Transform[] _elevationMarkers;
        private int _currentElevationIndex;

        private Camera _camera;
        private float _targetYPosition;
        private float _previousYPosition;
        private bool _isMovementBlocked = false;

        public LoadingPriority Priority => LoadingPriority.Low;

        [Inject]
        public void Construct(Camera camera)
        { 
            _camera= camera;
            Debug.Log("Construct: Camera Movement Component");
        }

        public void OnGameStart()
        {
            _currentElevationIndex = 0;
            transform.position = new Vector3(0, _elevationMarkers[_currentElevationIndex].position.y, 0);
            _targetYPosition = transform.position.y;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_targetYPosition != transform.position.y)
            {
                _isMovementBlocked = true;
                float direction = Mathf.Sign(_targetYPosition - transform.position.y);
                float magnitude = Mathf.Min(_elevationSpeed * deltaTime * _animationCurve.Evaluate((_targetYPosition - transform.position.y)/(_targetYPosition - _previousYPosition)), Mathf.Abs(_targetYPosition - transform.position.y));
                transform.Translate(Vector3.up * direction * magnitude);
            }
            else
            {
                _isMovementBlocked = false;
            }
        }

        internal void MoveCameraAxis(float amount)
        {
            if (_isMovementBlocked)
            {
                return;
            }

            if (amount > 0)
            {
                _currentElevationIndex--;

            }

            if (amount < 0)
            { 
                _currentElevationIndex++;
            }

            _currentElevationIndex = Mathf.Max(0, _currentElevationIndex);
            _currentElevationIndex = Mathf.Min(_elevationMarkers.Length - 1, _currentElevationIndex);
            _previousYPosition = _targetYPosition;
            _targetYPosition = _elevationMarkers[_currentElevationIndex].position.y;
        }

        internal void RotateCameraAxis(float amount)
        {
            transform.Rotate(Vector3.up, amount * _rotationSpeed);
        }

        internal void ZoomCamera(float amount)
        {
            var newPosition = _camera.transform.position + _camera.transform.forward * (amount * _zoomSpeed);
            float distanceToAxis = Mathf.Sqrt(newPosition.x * newPosition.x + newPosition.z * newPosition.z);
            if (distanceToAxis >= _minZoomDistance && distanceToAxis <= _maxZoomDistance)
            {
                _camera.transform.position = newPosition;
            }
        }
    }
}
