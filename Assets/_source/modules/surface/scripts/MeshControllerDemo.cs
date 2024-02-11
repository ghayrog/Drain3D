using System.Collections;
using UnityEngine;

namespace Surface
{

    internal sealed class MeshControllerDemo : MonoBehaviour
    {
        private const int DEMO_GRID_SIZE = 7;

        [SerializeField]
        private MeshController _meshController;

        [SerializeField]
        private float _demoSpeed = 1f;

        [SerializeField]
        private float _demoTimestep = 0.25f;

        private SurfaceGrid _demoGrid;
        private SurfaceGrid _demoThicknessGrid;

        private void MakeSurfaceDemo()
        {
            SurfaceGrid surfaceGrid = new SurfaceGrid(1.75f, 1.75f, new float[DEMO_GRID_SIZE, DEMO_GRID_SIZE]);
            _meshController.MakeFlatMesh(0, 0, 0, 1, surfaceGrid, 0, 0);
            StartCoroutine(ModifySurfaceCoroutine(surfaceGrid));
        }

        private IEnumerator ModifySurfaceCoroutine(SurfaceGrid surfaceGrid)
        {
            int i = 25;
            int j = 25;
            int direction = 1;
            while (true)
            {
                yield return new WaitForSeconds(_demoTimestep);
                for (int k = 0; k < 25; k++)
                {
                    surfaceGrid.SetGridValueAt(i, j, surfaceGrid.Grid[i, j] + direction * _demoTimestep * _demoSpeed);
                    i = Random.Range(Mathf.Max(i - 1, 0), Mathf.Min(i + 1 + 1, DEMO_GRID_SIZE-1));
                    j = Random.Range(Mathf.Max(j - 1, 0), Mathf.Min(j + 1 + 1, DEMO_GRID_SIZE-1));
                }
                _meshController.UpdateFlatMesh(0, 0, 0, 1, surfaceGrid, 0, 0);
                direction = Random.Range(0, 2) * 2 - 1;
            }
        }

        private void DownscaleDemo()
        {
            _demoGrid = new SurfaceGrid(10, 10, new float[DEMO_GRID_SIZE, DEMO_GRID_SIZE]);
            _demoThicknessGrid = new SurfaceGrid(10, 10, new float[DEMO_GRID_SIZE, DEMO_GRID_SIZE]);
            _demoThicknessGrid = _demoThicknessGrid.LinearTransform(0, 5);
            ModifySurface(_demoGrid, 500);
            ModifySurface(_demoThicknessGrid, 100);
            _demoGrid = _demoGrid.Downscale();
            _demoThicknessGrid = _demoThicknessGrid.Downscale();
            _demoGrid = _demoGrid.Downscale();
            _demoThicknessGrid = _demoThicknessGrid.Downscale();
            //_demoGrid = _demoGrid.MakeSubgrid(10, 20, 10 , 20);
            //_demoThicknessGrid = _demoThicknessGrid.MakeSubgrid(10, 20, 10, 20);
            _meshController.MakeThickMesh(-30, 0, -30, _demoGrid, _demoThicknessGrid);
//          _meshController.RemakeFlatMesh(0, 0, 0, 1, _demoGrid, 0, 0);
        }

        private void ModifySurface(SurfaceGrid surfaceGrid, int iterations)
        { 
            int i = surfaceGrid.Grid.GetLength(0) / 2;
            int j = surfaceGrid.Grid.GetLength(1) / 2;
            int maxI = surfaceGrid.Grid.GetLength(0);
            int maxJ = surfaceGrid.Grid.GetLength(1);
            int direction = 1;

            for (int iter = 0; iter < iterations; iter++)
            {
                for (int k = 0; k < 25; k++)
                {
                    surfaceGrid.SetGridValueAt(i, j, surfaceGrid.Grid[i, j] + direction * _demoTimestep * _demoSpeed);
                    i = Random.Range(Mathf.Max(i - 1, 0), Mathf.Min(i + 1, maxI - 1) + 1);
                    j = Random.Range(Mathf.Max(j - 1, 0), Mathf.Min(j + 1, maxJ - 1) + 1);
                }
                direction = Random.Range(0, 2) * 2 - 1;
            }
            //_meshController.UpdateFlatMesh(0, 0, 0, 1, surfaceGrid, 0, 0);
        }

        private void Start()
        {
            DownscaleDemo();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DownscaleDemo();
            }
        }
    }
}
