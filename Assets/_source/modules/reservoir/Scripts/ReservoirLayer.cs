using UnityEngine;
using Surface;
using Hydraulics;

namespace Reservoir
{
    public sealed class ReservoirLayer
    {
        public const int GRID_INITIAL_SIZE = 7;
        public const float CELL_INITIAL_SIZE = 10;
        public const int DOWNSCALE_ITERATIONS = 2;
        private const int RANDOM_ITERATIONS = 500;

        internal SurfaceGrid StructureGrid { get; private set; }
        internal SurfaceGrid ThicknessGrid { get; private set; }
        internal bool[,] VisibilityMask { get; private set; }
        internal bool[,] DrilledMask { get; private set; }
        private SurfaceGrid _permeabilityGrid;
        private SurfaceGrid _porosityGrid;
        private SurfaceGrid _netGrid;
        private VolumeCell[,] _cells;
        internal float FluidContact { get; private set; }
        public int SizeX => StructureGrid.Grid.GetLength(0);
        public int SizeY => StructureGrid.Grid.GetLength(1);

        internal float Contact => FluidContact;

        internal void DrillCell(int i, int j)
        {
            DrilledMask[i, j] = true;
        }

        public bool GetVisibility(int i, int j)
        { 
            return VisibilityMask[i, j];
        }

        public float GetAverageDepth(int i, int j)
        {
            return (StructureGrid.Grid[i, j] + StructureGrid.Grid[i + 1, j] + StructureGrid.Grid[i, j + 1] + StructureGrid.Grid[i + 1, j + 1])/4;
        }

        internal float GetBottomDepth()
        {
            float bottomDepth = StructureGrid.Grid[0, 0] - ThicknessGrid.Grid[0, 0];
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    bottomDepth = Mathf.Min(bottomDepth, StructureGrid.Grid[i, j] - ThicknessGrid.Grid[i, j]);
                }
            }
            return bottomDepth;
        }

        internal void MaskEntireReservoir()
        {
            for (int i = 0; i < VisibilityMask.GetLength(0); i++)
            {
                for (int j = 0; j < VisibilityMask.GetLength(1); j++)
                {
                    VisibilityMask[i, j] = false;
                }
            }
        }

        internal void UnmaskEntireReservoir()
        {
            for (int i = 0; i < VisibilityMask.GetLength(0); i++)
            {
                for (int j = 0; j < VisibilityMask.GetLength(1); j++)
                {
                    VisibilityMask[i, j] = true;
                }
            }
        }

        internal void SetCellMask(int i, int j, bool value)
        {
            VisibilityMask[i, j] = value;
        }

        internal void GenerateRandomReservoir(float productiveFraction, float reservoirHeight, float minPermeability, float maxPermeability, float porosity, float netToGross, bool isVisible)
        { 
            StructureGrid = SurfaceEditor.CreateRandomGrid(GRID_INITIAL_SIZE, GRID_INITIAL_SIZE, CELL_INITIAL_SIZE, CELL_INITIAL_SIZE, -reservoirHeight/2, reservoirHeight/2, RANDOM_ITERATIONS);
            ThicknessGrid = SurfaceEditor.CreateRandomGrid(GRID_INITIAL_SIZE, GRID_INITIAL_SIZE, CELL_INITIAL_SIZE, CELL_INITIAL_SIZE, 1, reservoirHeight / 2, RANDOM_ITERATIONS);
            _permeabilityGrid = SurfaceEditor.CreateRandomGrid(GRID_INITIAL_SIZE, GRID_INITIAL_SIZE, CELL_INITIAL_SIZE, CELL_INITIAL_SIZE, minPermeability, maxPermeability, RANDOM_ITERATIONS);
            _porosityGrid = SurfaceEditor.CreateRandomGrid(GRID_INITIAL_SIZE, GRID_INITIAL_SIZE, CELL_INITIAL_SIZE, CELL_INITIAL_SIZE, porosity, porosity, RANDOM_ITERATIONS);
            _netGrid = SurfaceEditor.CreateRandomGrid(GRID_INITIAL_SIZE, GRID_INITIAL_SIZE, CELL_INITIAL_SIZE, CELL_INITIAL_SIZE, netToGross, netToGross, RANDOM_ITERATIONS);

            SurfaceGrid contactGrid = StructureGrid.Subtract(ThicknessGrid.LinearTransform(0.5f, 0));
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
                FluidContact = (productiveFraction - 0.5f) * 2 * (StructureGrid.MinValue - contactValue) + contactValue;
            }
            else
            {
                FluidContact = (0.5f - productiveFraction) * 2 * (StructureGrid.MaxValue - contactValue) + contactValue;
            }
            //Debug.Log($"Contact: {_fluidContact}, Max: {_structureGrid.MaxValue}, Min: {_structureGrid.MinValue}, Middle: {contactValue}");

            for (int i = 0; i < DOWNSCALE_ITERATIONS; i++)
            {
                StructureGrid = StructureGrid.Downscale();
                ThicknessGrid = ThicknessGrid.Downscale();
                _permeabilityGrid = _permeabilityGrid.Downscale();
                _porosityGrid = _porosityGrid.Downscale();
                _netGrid = _netGrid.Downscale();
            }


            VisibilityMask = new bool[StructureGrid.Grid.GetLength(0) - 1, StructureGrid.Grid.GetLength(1) - 1];
            DrilledMask = new bool[StructureGrid.Grid.GetLength(0) - 1, StructureGrid.Grid.GetLength(1) - 1];
            for (int i = 0; i < StructureGrid.Grid.GetLength(0)-1; i++)
            {
                for (int j = 0; j < StructureGrid.Grid.GetLength(1)-1; j++)
                {
                    VisibilityMask[i, j] = isVisible;
                    DrilledMask[i, j] = false;
                    //VisibilityMask[i, j] = (Random.Range((float)0, (float)1) > 0.5f) ? true : false;
                    //_visibilityMask[i,j] = (j % 3 == 1 || i % 5 == 1);
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
    }
}
