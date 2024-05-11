using UnityEngine;

namespace Land
{
    internal sealed class Land
    {
        internal const int GRID_SIZE = 24;
        internal const float CELL_SIZE = 2.5f;

        internal LandType[,] LandGrid { get; private set; } = new LandType[GRID_SIZE, GRID_SIZE];

        internal int FindFactorySpot()
        {
            int index = GRID_SIZE / 2;
            for (int i = GRID_SIZE / 3; i < GRID_SIZE * 2 / 3; i++)
            { 
                if (LandGrid[i,0] == LandType.Grass)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        internal void GenerateTerrain(Vector3[] interestPoints, bool generateRiver, float forestDensity, float swampDensity)
        {
            Debug.Log("GENERATING TERRAIN...");
            FillWithGrass();
            int forestIterations = (int)(GRID_SIZE * GRID_SIZE * forestDensity);
            int swampIterations = (int)(GRID_SIZE * GRID_SIZE * swampDensity);
            int totalNumberOfForests = (int)(interestPoints.Length * forestDensity / (forestDensity + swampDensity));
            int totalNumberOfSwamps = (int)(interestPoints.Length * swampDensity / (forestDensity + swampDensity));
            Debug.Log($"Total number of swamps: {totalNumberOfSwamps} forests: {totalNumberOfForests}");
            int forestCounter = 0;
            int swampCounter = 0;
            for (int i = 0; i < interestPoints.Length; i++)
            {
                bool isSwamp = true;
                if (swampCounter < totalNumberOfSwamps && forestCounter < totalNumberOfForests)
                {
                    isSwamp = (Random.Range(0f, 1f) > 0.5f) ? false : true;
                }
                if (swampCounter == totalNumberOfSwamps)
                {
                    isSwamp = false;
                }
                if (isSwamp)
                {
                    AddSwamp(CoordToIndex(interestPoints[i].x), CoordToIndex(interestPoints[i].z), swampIterations);
                    Debug.Log($"Created a swamp at {CoordToIndex(interestPoints[i].x)}, {CoordToIndex(interestPoints[i].z)}");
                    swampCounter++;
                }
                if (!isSwamp)
                { 
                    AddForest(CoordToIndex(interestPoints[i].x), CoordToIndex(interestPoints[i].z), forestIterations);
                    Debug.Log($"Created a forest at {CoordToIndex(interestPoints[i].x)}, {CoordToIndex(interestPoints[i].z)}");
                    forestCounter++;
                }
            }
            if (generateRiver) AddRiver(Land.GRID_SIZE / 4 + UnityEngine.Random.Range(0, Land.GRID_SIZE / 2), Land.GRID_SIZE / 4 + UnityEngine.Random.Range(0, Land.GRID_SIZE / 2), UnityEngine.Random.Range(0f, 1f) > 0.5f ? true : false);
        }

        private int CoordToIndex(float coord)
        {
            return (int)((coord + GRID_SIZE * CELL_SIZE / 2) / CELL_SIZE);
        }

        internal void FillWithForest()
        {
            for (int i = 0; i < GRID_SIZE; i++)
            {
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    LandGrid[i, j] = LandType.Forest;
                }
            }
        }

        internal void FillWithGrass()
        {
            for (int i = 0; i < GRID_SIZE; i++)
            {
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    LandGrid[i, j] = LandType.Grass;
                }
            }
        }

        internal void AddForest(int startI, int startJ, int iterations)
        {
            int i = startI;
            int j = startJ;
            for (int k = 0; k < iterations; k++)
            {
                LandGrid[i, j] = LandType.Forest;
                i = Random.Range(Mathf.Max(i - 1, 0), Mathf.Min(i + 1, GRID_SIZE - 1) + 1);
                j = Random.Range(Mathf.Max(j - 1, 0), Mathf.Min(j + 1, GRID_SIZE - 1) + 1);
            }
        }

        internal void AddSwamp(int startI, int startJ, int iterations)
        {
            int i = startI;
            int j = startJ;
            for (int k = 0; k < iterations; k++)
            {
                LandGrid[i, j] = LandType.Swamp;
                i = Random.Range(Mathf.Max(i - 1, 0), Mathf.Min(i + 1, GRID_SIZE - 1) + 1);
                j = Random.Range(Mathf.Max(j - 1, 0), Mathf.Min(j + 1, GRID_SIZE - 1) + 1);
            }
        }

        internal void AddRiver(int startI, int startJ, bool isBranched)
        {
            int orientation = (Random.Range(0f, 1f) > 0.5f) ? 1 : -1;
            int i = startI;
            int j = startJ;
            while (i >=0 && j >= 0 && j < GRID_SIZE)
            {
                LandGrid[i, j] = LandType.River;
                if (j == 0 || j == GRID_SIZE - 1)
                {
                    j -= orientation;
                }
                else if (i == 0 || i == startI || Random.Range(0f, 1f) > 0.5f)
                {
                    i--;
                }
                else
                {
                    j -= orientation;
                }

            }

            i = startI;
            j = startJ;
            while (i < GRID_SIZE && j < GRID_SIZE && j >= 0)
            {
                LandGrid[i, j] = LandType.River;
                if (j == 0 || j == GRID_SIZE - 1)
                {
                    j += orientation;
                }
                else if (i == GRID_SIZE-1 || i == startI || Random.Range(0f, 1f) > 0.5f)
                {
                    i++;
                }
                else
                {
                    j += orientation;
                }
            }

            if (!isBranched) return;

            orientation = -orientation;
            i = startI + 1;
            j = startJ + orientation;
            while (i < GRID_SIZE && j < GRID_SIZE && j >= 0)
            {
                LandGrid[i, j] = LandType.River;
                if (j == startJ + orientation || j == 0 || j == GRID_SIZE - 1)
                {
                    j += orientation;
                }
                else if (i != startI && (i == GRID_SIZE - 1 || Random.Range(0f, 1f) > 0.5f))
                {
                    i++;
                }
                else
                {
                    j += orientation;
                }
            }
        }

    }
}
