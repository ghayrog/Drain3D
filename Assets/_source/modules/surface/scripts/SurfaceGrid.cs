using UnityEngine;

namespace Surface
{
    public sealed class SurfaceGrid
    {
        public float DX { get; private set; }
        public float DY { get; private set; }
        public float[,] Grid { get; private set; }
        public float MaxValue { get; private set; }
        public float MinValue { get; private set; }

        public SurfaceGrid(float dx, float dy, float[,] grid)
        {
            DX = dx;
            DY = dy;
            Grid = grid;
            CalculateMaxAndMin();
        }

        private void CalculateMaxAndMin()
        {
            if (Grid == null) return;
            float maxValue = Grid[0, 0];
            float minValue = maxValue;
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    float newValue = Grid[i, j];
                    if (newValue < minValue) minValue = newValue;
                    if (newValue > maxValue) maxValue = newValue;
                }
            }
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public SurfaceGrid Add(SurfaceGrid surfaceGrid)
        {
            if (surfaceGrid.Grid.GetLength(0) == Grid.GetLength(0) && surfaceGrid.Grid.GetLength(1) == Grid.GetLength(1))
            {
                float[,] grid = new float[Grid.GetLength(0), Grid.GetLength(1)];
                for (int i = 0; i < Grid.GetLength(0); i++)
                {
                    for (int j = 0; j < Grid.GetLength(1); j++)
                    {
                        grid[i, j] = Grid[i, j] + surfaceGrid.Grid[i, j];
                    }
                }

                return new SurfaceGrid(DX, DY, grid);
            }
            return null;
        }

        public SurfaceGrid Invert()
        {
            float[,] grid = new float[Grid.GetLength(0), Grid.GetLength(1)];
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    grid[i, j] = -Grid[i, j];
                }
            }

            return new SurfaceGrid(DX, DY, grid);
        }

        public SurfaceGrid Subtract(SurfaceGrid surfaceGrid)
        {
            if (surfaceGrid.Grid.GetLength(0) == Grid.GetLength(0) && surfaceGrid.Grid.GetLength(1) == Grid.GetLength(1))
            {
                float[,] grid = new float[Grid.GetLength(0), Grid.GetLength(1)];
                for (int i = 0; i < Grid.GetLength(0); i++)
                {
                    for (int j = 0; j < Grid.GetLength(1); j++)
                    {
                        grid[i, j] = Grid[i, j] - surfaceGrid.Grid[i, j];
                    }
                }

                return new SurfaceGrid(DX, DY, grid);
            }
            Debug.Log($"Wrong grid size {surfaceGrid.Grid.GetLength(0)} by {surfaceGrid.Grid.GetLength(1)} instead of {Grid.GetLength(0)} by {Grid.GetLength(1)}");
            return null;
        }

        public SurfaceGrid MakeSubgrid(int startI, int endI, int startJ, int endJ)
        {
            float[,] grid = new float[endI - startI + 1, endJ - startJ + 1];
            for (int i = startI; i <= endI; i++)
            {
                for (int j = startJ; j <= endJ; j++)
                {
                    grid[i - startI, j - startJ] = Grid[i, j];
                }
            }
            return new SurfaceGrid(DX,DY, grid);
        }

        public SurfaceGrid LinearTransform(float a, float b)
        {
            float[,] grid = new float[Grid.GetLength(0), Grid.GetLength(1)];
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    grid[i, j] = a * Grid[i, j] + b;
                }
            }
            return new SurfaceGrid(DX, DY, grid);
        }

        public SurfaceGrid Downscale()
        {
            const float MAIN_WEIGHT = 1 / 2.5f;
            const float CROSS_WEIGHT = (1 - MAIN_WEIGHT) / 1.6f / 4;
            const float DIAG_WEIGHT = (1 - MAIN_WEIGHT - CROSS_WEIGHT * 4) / 4;

            float scaledDX = DX / 2;
            float scaledDY = DY / 2;
            int dimensionX = Grid.GetLength(0);
            int dimensionY = Grid.GetLength(1);
            int scaledDimensionX = dimensionX * 2 - 1;
            int scaledDimensionY = dimensionY * 2 - 1;
            float[,] scaledGrid = new float[scaledDimensionX, scaledDimensionY];
            float[,] smoothedGrid = new float[scaledDimensionX, scaledDimensionY];

            for (int i = 0; i < dimensionX - 1; i++)
            {
                for (int j = 0; j < dimensionY - 1; j++)
                {
                    scaledGrid[i * 2, j * 2] = Grid[i, j];
                    float gradientILeft = 0;
                    float gradientICenter = Grid[i + 1, j] - Grid[i, j];
                    float gradientIRight = 0;

                    float gradientJLeft = 0;
                    float gradientJCenter = Grid[i, j + 1] - Grid[i, j];
                    float gradientJRight = 0;

                    float gradientIJLeft = 0;
                    float gradientIJCenter = Grid[i + 1, j + 1] - Grid[i, j];
                    float gradientIJRight = 0;

                    if (i == 0)
                    {
                        gradientILeft = gradientICenter;
                    }
                    else
                    {
                        gradientILeft = Grid[i, j] - Grid[i - 1, j];
                    }

                    if (j == 0)
                    {
                        gradientJLeft = gradientJCenter;
                    }
                    else
                    {
                        gradientJLeft = Grid[i, j] - Grid[i, j - 1];
                    }

                    if (i == 0 || j == 0)
                    {
                        gradientIJLeft = gradientIJCenter;
                    }
                    else
                    {
                        gradientIJLeft = Grid[i, j] - Grid[i - 1, j - 1];
                    }

                    if (i == dimensionX - 2)
                    {
                        gradientIRight = gradientICenter;
                    }
                    else
                    {
                        gradientIRight = Grid[i + 2, j] - Grid[i + 1, j];
                    }

                    if (j == dimensionY - 2)
                    {
                        gradientJRight = gradientJCenter;
                    }
                    else
                    {
                        gradientJRight = Grid[i, j + 2] - Grid[i, j + 1];
                    }

                    if (i == dimensionX - 2 || j == dimensionY - 2)
                    {
                        gradientIJRight = gradientIJCenter;
                    }
                    else
                    {
                        gradientIJRight = Grid[i + 2, j + 2] - Grid[i + 1, j + 1];
                    }

                    float valueI = (Grid[i, j] + gradientILeft / 2) / 3 + (Grid[i, j] + gradientICenter / 2) / 3 + (Grid[i + 1, j] - gradientIRight / 2) / 3;
                    float valueJ = (Grid[i, j] + gradientJLeft / 2) / 3 + (Grid[i, j] + gradientJCenter / 2) / 3 + (Grid[i, j + 1] - gradientJRight / 2) / 3;
                    float valueIJ = (Grid[i, j] + gradientIJLeft / 2) / 3 + (Grid[i, j] + gradientIJCenter / 2) / 3 + (Grid[i+1, j + 1] - gradientIJRight / 2) / 3;

                    scaledGrid[i * 2 + 1, j * 2] = valueI;
                    scaledGrid[i * 2, j * 2 + 1] = valueJ;
                    scaledGrid[i * 2 + 1, j * 2 + 1] = valueIJ;
                }
                scaledGrid[i * 2, scaledDimensionY - 1] = Grid[i, dimensionY - 1];

                float edgeILeft = 0;
                float edgeICenter = Grid[i + 1, dimensionY - 1] - Grid[i, dimensionY - 1];
                float edgeIRight = 0;

                if (i == 0)
                {
                    edgeILeft = edgeICenter;
                }
                else
                {
                    edgeILeft = Grid[i, dimensionY - 1] - Grid[i - 1, dimensionY - 1];
                }

                if (i == dimensionX - 2)
                {
                    edgeIRight = edgeICenter;
                }
                else
                {
                    edgeIRight = Grid[i + 2, dimensionY - 1] - Grid[i + 1, dimensionY - 1];
                }

                float edgeValueI = (Grid[i, dimensionY - 1] + edgeILeft / 2) / 3 + (Grid[i, dimensionY - 1] + edgeICenter / 2) / 3 + (Grid[i + 1, dimensionY - 1] - edgeIRight / 2) / 3;
                scaledGrid[i * 2 + 1, scaledDimensionY - 1] = edgeValueI;
            }
            for (int j = 0; j < dimensionY - 1; j++)
            {
                scaledGrid[scaledDimensionX - 1, j * 2] = Grid[dimensionX - 1, j];

                float edgeJLeft = 0;
                float edgeJCenter = Grid[dimensionX - 1, j + 1] - Grid[dimensionX - 1, j];
                float edgeJRight = 0;

                if (j == 0)
                {
                    edgeJLeft = edgeJCenter;
                }
                else
                {
                    edgeJLeft = Grid[dimensionX-1, j] - Grid[dimensionX - 1, j - 1];
                }

                if (j == dimensionY - 2)
                {
                    edgeJRight = edgeJCenter;
                }
                else
                {
                    edgeJRight = Grid[dimensionX - 1, j + 2] - Grid[dimensionX - 1, j + 1];
                }

                float edgeValueJ = (Grid[dimensionX - 1, j] + edgeJLeft / 2) / 3 + (Grid[dimensionX - 1, j] + edgeJCenter / 2) / 3 + (Grid[dimensionX - 1, j + 1] - edgeJRight / 2) / 3;
                scaledGrid[scaledDimensionX - 1, j * 2 + 1] = edgeValueJ;
            }
            scaledGrid[scaledDimensionX - 1, scaledDimensionY - 1] = Grid[dimensionX - 1, dimensionY - 1];

            for (int i = 0; i < dimensionX ; i++)
            {
                for (int j = 0; j < dimensionY; j++)
                {
                    int localI = i * 2;
                    int localJ = j * 2;
                    smoothedGrid[localI, localJ] = scaledGrid[localI, localJ];

                    localI = i * 2 - 1;
                    localJ = j * 2 - 1;
                    if (localI > 0 && localI < scaledDimensionX - 1 && localJ > 0 && localJ < scaledDimensionY - 1)
                    {
                        smoothedGrid[localI, localJ] =
                            scaledGrid[localI, localJ] * MAIN_WEIGHT +
                            scaledGrid[localI + 1, localJ] * CROSS_WEIGHT +
                            scaledGrid[localI, localJ + 1] * CROSS_WEIGHT +
                            scaledGrid[localI - 1, localJ] * CROSS_WEIGHT +
                            scaledGrid[localI, localJ - 1] * CROSS_WEIGHT +
                            scaledGrid[localI + 1, localJ + 1] * DIAG_WEIGHT +
                            scaledGrid[localI - 1, localJ + 1] * DIAG_WEIGHT +
                            scaledGrid[localI - 1, localJ - 1] * DIAG_WEIGHT +
                            scaledGrid[localI + 1, localJ - 1] * DIAG_WEIGHT;
                    }
                    else
                    if (localI >= 0 && localI <= scaledDimensionX - 1 && localJ >= 0 && localJ <= scaledDimensionY - 1)
                    {
                        smoothedGrid[localI, localJ] = scaledGrid[localI, localJ];
                    }

                    localI = i * 2 - 1;
                    localJ = j * 2;
                    if (localI > 0 && localI < scaledDimensionX - 1 && localJ > 0 && localJ < scaledDimensionY - 1)
                    {
                        smoothedGrid[localI, localJ] =
                        scaledGrid[localI, localJ] * MAIN_WEIGHT +
                        scaledGrid[localI + 1, localJ] * CROSS_WEIGHT +
                        scaledGrid[localI, localJ + 1] * CROSS_WEIGHT +
                        scaledGrid[localI - 1, localJ] * CROSS_WEIGHT +
                        scaledGrid[localI, localJ - 1] * CROSS_WEIGHT +
                        scaledGrid[localI + 1, localJ + 1] * DIAG_WEIGHT +
                        scaledGrid[localI - 1, localJ + 1] * DIAG_WEIGHT +
                        scaledGrid[localI - 1, localJ - 1] * DIAG_WEIGHT +
                        scaledGrid[localI + 1, localJ - 1] * DIAG_WEIGHT;
                    }
                    else
                    if (localI >= 0 && localI <= scaledDimensionX - 1 && localJ >= 0 && localJ <= scaledDimensionY - 1)

                    {
                        smoothedGrid[localI, localJ] = scaledGrid[localI, localJ];
                    }

                    localI = i * 2;
                    localJ = j * 2 - 1;
                    if (localI > 0 && localI < scaledDimensionX - 1 && localJ > 0 && localJ < scaledDimensionY - 1)
                    {

                        smoothedGrid[localI, localJ] =
                        scaledGrid[localI, localJ] * MAIN_WEIGHT +
                        scaledGrid[localI + 1, localJ] * CROSS_WEIGHT +
                        scaledGrid[localI, localJ + 1] * CROSS_WEIGHT +
                        scaledGrid[localI - 1, localJ] * CROSS_WEIGHT +
                        scaledGrid[localI, localJ - 1] * CROSS_WEIGHT +
                        scaledGrid[localI + 1, localJ + 1] * DIAG_WEIGHT +
                        scaledGrid[localI - 1, localJ + 1] * DIAG_WEIGHT +
                        scaledGrid[localI - 1, localJ - 1] * DIAG_WEIGHT +
                        scaledGrid[localI + 1, localJ - 1] * DIAG_WEIGHT;
                    }
                    else
                    if (localI >= 0 && localI <= scaledDimensionX - 1 && localJ >= 0 && localJ <= scaledDimensionY - 1)
                    {
                        smoothedGrid[localI, localJ] = scaledGrid[localI, localJ];
                    }
                }
            }

            //Debug.Log($"Downscaled grid from {dimensionX} by {dimensionY} to {scaledDimensionX} by {scaledDimensionY}");
            //Debug.Log($"Resulting cell size {scaledDX} by {scaledDY}");
            return new SurfaceGrid(scaledDX, scaledDY, smoothedGrid);
        }

        internal void SetGridValueAt(int i, int j, float gridValue)
        {
            Grid[i, j] = gridValue;
            CalculateMaxAndMin();
        }
    }
}
