using System;
using UnityEngine;
using DI;
using Game;

namespace Reservoir
{
    [Serializable]
    public sealed class ReservoirClicker :
        IGameStartListener, IGameUpdateListener
    {
        private float _gridSize = 2.5f;

        [SerializeField]
        private GameObject _cellMarkerPrefab;

        private ReservoirManager _reservoirManager;
        private Transform _parentTransform;
        //private Transform _cellMarker;
        private Vector3 _defaultCellMarkerPosition = new Vector3(-1000,1000,-1000);
        private int _currentCellI;
        private int _currentCellJ;
        private int _currentReservoirId;
        private Transform _cellMarker;

        public LoadingPriority Priority => LoadingPriority.Low;
        public int CurrentI => _currentCellI;
        public int CurrentJ => _currentCellJ;

        [Inject]
        private void Construct(ReservoirManager reservoirManager)
        {
            _gridSize = ReservoirLayer.CELL_INITIAL_SIZE;
            for (int i = 0; i < ReservoirLayer.DOWNSCALE_ITERATIONS; i++)
            {
                _gridSize = _gridSize / 2;
            }
            _parentTransform = reservoirManager.transform;
            _reservoirManager = reservoirManager;
        }

        public void OnGameStart()
        {
            _cellMarker = GameObject.Instantiate(_cellMarkerPrefab, _defaultCellMarkerPosition, Quaternion.identity, _parentTransform).transform;
        }

        private void CheckClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit)) return;
            //Debug.Log("Physics raycast hit ok");

            ReservoirLayer reservoir = _reservoirManager.GetReservoirByClosestDepth(hit.point.y);
            _currentReservoirId = _reservoirManager.GetReservoirId(reservoir);
            var referenceDepth = _reservoirManager.GetReservoirDepthById(_currentReservoirId);
            //Debug.Log($"Reference depth: {referenceDepth}");
            var intersection = ReservoirLayerVisualizer.GetRayIntersection(reservoir, ray, hit.point, referenceDepth);
            if (!intersection.isValid) return;
            //Debug.Log("Intersection is valid");
            _cellMarker.transform.position = new Vector3(intersection.x, 0, intersection.z);
            _currentCellI = intersection.i;
            _currentCellJ = intersection.j;
        }

        public void SetClickMarker(int i, int j)
        { 
            _currentCellI= i;
            _currentCellJ= j;
            Vector3 newPosition = ReservoirLayerVisualizer.GetCellPosition(_reservoirManager.GetReservoirByClosestDepth(0), _currentCellI, _currentCellJ);
            //Debug.Log(newPosition);
            _cellMarker.transform.position = new Vector3(newPosition.x, 0, newPosition.z);
            //_cellMarker.transform.position = new Vector3();
        }

        public void OnUpdate(float deltaTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                CheckClick();
            }
        }
    }
}
