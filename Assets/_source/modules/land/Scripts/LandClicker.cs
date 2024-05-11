using System;
using UnityEngine;
using DI;
using Game;
using Reservoir;

namespace Land
{
    [Serializable]
    internal sealed class LandClicker : 
        IGameUpdateListener
    {
        public LoadingPriority Priority => LoadingPriority.Low;

        [SerializeField]
        private float _thresholdDepth;

        private ReservoirClicker _reservoirClicker;

        [Inject]
        private void Construct(ReservoirClicker reservoirClicker)
        {
            _reservoirClicker = reservoirClicker;
        }

        private void CheckClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit) || hit.point.y < _thresholdDepth) return;

            int i = (int)((hit.point.x + Land.CELL_SIZE * Land.GRID_SIZE / 2) / Land.CELL_SIZE);
            int j = (int)((hit.point.z + Land.CELL_SIZE * Land.GRID_SIZE / 2) / Land.CELL_SIZE);

            //Debug.Log($"Click surface grid: {i}, {j}");
            if (i >= 0 && j >= 0 && i < Land.GRID_SIZE && j < Land.GRID_SIZE)
            {
                _reservoirClicker.SetClickMarker(i, j);
            }
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
