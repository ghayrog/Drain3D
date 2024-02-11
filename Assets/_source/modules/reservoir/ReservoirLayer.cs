using UnityEngine;
using Surface;
using Hydraulics;

namespace Reservoir
{
    public sealed class ReservoirLayer
    {
        private const int GRID_INITIAL_SIZE = 7;
        private const float CELL_INITIAL_SIZE = 10;
        private const int RANDOM_ITERATIONS = 500;
        private const int DOWNSCALE_ITERATIONS = 2;

        private SurfaceGrid _structureGrid;
        private SurfaceGrid _thicknessGrid;
        private SurfaceGrid _permeabilityGrid;
        private SurfaceGrid _porosityGrid;
        private SurfaceGrid _netGrid;
        private bool[,] _visibilityMask;
        private VolumeCell[,] _cells;
        private float _fluidContact;
        public int SizeX => _structureGrid.Grid.GetLength(0);
        public int SizeY => _structureGrid.Grid.GetLength(1);

        internal float Contact => _fluidContact;

        internal void MaskEntireReservoir()
        {
            for (int i = 0; i < _visibilityMask.GetLength(0); i++)
            {
                for (int j = 0; j < _visibilityMask.GetLength(1); j++)
                {
                    _visibilityMask[i, j] = false;
                }
            }
        }

        internal void UnmaskEntireReservoir()
        {
            for (int i = 0; i < _visibilityMask.GetLength(0); i++)
            {
                for (int j = 0; j < _visibilityMask.GetLength(1); j++)
                {
                    _visibilityMask[i, j] = true;
                }
            }
        }

        internal void SetCellMask(int i, int j, bool value)
        {
            _visibilityMask[i, j] = value;
        }

        internal void GenerateRandomReservoir(float productiveFraction, float reservoirHeight, float minPermeability, float maxPermeability, float porosity, float netToGross, bool isVisible)
        { 
            _structureGrid = SurfaceEditor.CreateRandomGrid(GRID_INITIAL_SIZE, GRID_INITIAL_SIZE, CELL_INITIAL_SIZE, CELL_INITIAL_SIZE, -reservoirHeight/2, reservoirHeight/2, RANDOM_ITERATIONS);
            _thicknessGrid = SurfaceEditor.CreateRandomGrid(GRID_INITIAL_SIZE, GRID_INITIAL_SIZE, CELL_INITIAL_SIZE, CELL_INITIAL_SIZE, 1, reservoirHeight / 2, RANDOM_ITERATIONS);
            _permeabilityGrid = SurfaceEditor.CreateRandomGrid(GRID_INITIAL_SIZE, GRID_INITIAL_SIZE, CELL_INITIAL_SIZE, CELL_INITIAL_SIZE, minPermeability, maxPermeability, RANDOM_ITERATIONS);
            _porosityGrid = SurfaceEditor.CreateRandomGrid(GRID_INITIAL_SIZE, GRID_INITIAL_SIZE, CELL_INITIAL_SIZE, CELL_INITIAL_SIZE, porosity, porosity, RANDOM_ITERATIONS);
            _netGrid = SurfaceEditor.CreateRandomGrid(GRID_INITIAL_SIZE, GRID_INITIAL_SIZE, CELL_INITIAL_SIZE, CELL_INITIAL_SIZE, netToGross, netToGross, RANDOM_ITERATIONS);

            SurfaceGrid contactGrid = _structureGrid.Subtract(_thicknessGrid.LinearTransform(0.5f, 0));
            float contactValue = 0;
            int valuesCount = 0;
            for (int i = 0; i < contactGrid.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < contactGrid.Grid.GetLength(1); j++)
                {
                    contactValue += contactGrid.Grid[i, j];
                    valuesCount++;
                }
            }
            contactValue /= valuesCount;

            if (productiveFraction > 0.5f)
            {
                _fluidContact = (productiveFraction - 0.5f) * 2 * (_structureGrid.MinValue - contactValue) + contactValue;
            }
            else
            {
                _fluidContact = (0.5f - productiveFraction) * 2 * (_structureGrid.MaxValue - contactValue) + contactValue;
            }
            //Debug.Log($"Contact: {_fluidContact}, Max: {_structureGrid.MaxValue}, Min: {_structureGrid.MinValue}, Middle: {contactValue}");

            for (int i = 0; i < DOWNSCALE_ITERATIONS; i++)
            {
                _structureGrid = _structureGrid.Downscale();
                _thicknessGrid = _thicknessGrid.Downscale();
                _permeabilityGrid = _permeabilityGrid.Downscale();
                _porosityGrid = _porosityGrid.Downscale();
                _netGrid = _netGrid.Downscale();
            }


            _visibilityMask = new bool[_structureGrid.Grid.GetLength(0) - 1, _structureGrid.Grid.GetLength(1) - 1];
            for (int i = 0; i < _structureGrid.Grid.GetLength(0)-1; i++)
            {
                for (int j = 0; j < _structureGrid.Grid.GetLength(1)-1; j++)
                {
                    _visibilityMask[i, j] = isVisible;
                    _visibilityMask[i, j] = (Random.Range((float)0, (float)1) > 0.01f) ? true : false;
                }
            }
        }

        internal float CalculateTotalVolume()
        {
            return 0;
        }

        internal float CalculateProductiveVolume()
        {
            return 0;
        }

        internal Mesh CreateReservoirMeshNoMask()
        {
            Mesh mesh = MeshEditor.MakeVolumeMesh(-(GRID_INITIAL_SIZE-1) * CELL_INITIAL_SIZE / 2, 0, -(GRID_INITIAL_SIZE-1) * CELL_INITIAL_SIZE / 2,
                _structureGrid, _structureGrid.MaxValue,
                _thicknessGrid, _fluidContact, true);
            return mesh;
        }

        internal Mesh CreateReservoirMeshMasked()
        {
            Mesh mesh = null;
            float coordOrigin = -(GRID_INITIAL_SIZE - 1) * CELL_INITIAL_SIZE / 2;
            for (int i = 0; i < _visibilityMask.GetLength(0); i++)
            {
                for (int j = 0; j < _visibilityMask.GetLength(1); j++)
                {
                    if (_visibilityMask[i, j])
                    {
                        Mesh oneCellMesh = MeshEditor.MakeVolumeMesh(
                            coordOrigin + i * _structureGrid.DX,
                            0,
                            coordOrigin + j * _structureGrid.DY,
                            _structureGrid.MakeSubgrid(i, i + 1, j, j + 1),
                            _structureGrid.MaxValue,
                            _thicknessGrid.MakeSubgrid(i, i + 1, j, j + 1),
                            _fluidContact, false
                            );
                        mesh = MeshEditor.Join(mesh, oneCellMesh, false);
                    }
                }
            }

            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
