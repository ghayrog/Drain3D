using System.ComponentModel;
using UnityEditor.Graphs;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Surface
{
    public static class SurfaceEditor
    {
        public static SurfaceGrid CreateRandomGrid(int sizeX, int sizeY, float DX, float DY, float minValue, float maxValue, int iterations)
        {
            var surfaceGrid = new SurfaceGrid(DX, DY, new float[sizeX, sizeY]);
            int i = surfaceGrid.Grid.GetLength(0) / 2;
            int j = surfaceGrid.Grid.GetLength(1) / 2;
            int maxI = surfaceGrid.Grid.GetLength(0);
            int maxJ = surfaceGrid.Grid.GetLength(1);
            int direction = 1;

            const int oneDirectionPoints = 25;

            for (int iter = 0; iter < iterations; iter++)
            {
                for (int k = 0; k < oneDirectionPoints; k++)
                {
                    surfaceGrid.SetGridValueAt(i, j, surfaceGrid.Grid[i, j] + direction);
                    i = Random.Range(Mathf.Max(i - 1, 0), Mathf.Min(i + 1, maxI - 1) + 1);
                    j = Random.Range(Mathf.Max(j - 1, 0), Mathf.Min(j + 1, maxJ - 1) + 1);
                }
                direction = Random.Range(0, 2) * 2 - 1;
            }

            float temporaryMax = surfaceGrid.MaxValue;
            float temporaryMin = surfaceGrid.MinValue;
            for (i = 0; i < maxI; i++)
            {
                for (j = 0; j < maxJ; j++)
                {
                    float normalizedValue = (surfaceGrid.Grid[i, j] - temporaryMin) / (temporaryMax - temporaryMin) * (maxValue - minValue) + minValue;
                    surfaceGrid.SetGridValueAt(i, j, normalizedValue);
                }             
            }
            //Debug.Log($"Surface grid max {temporaryMax}->{surfaceGrid.MaxValue} and min {temporaryMin}->{surfaceGrid.MinValue}");
            return surfaceGrid;
        }

        public static Vector3[] GetVerticesFromSurface(SurfaceGrid surfaceGrid, float OriginX, float OriginY, float OriginZ, float valueScale)
        {
            Vector3[] vertices = new Vector3[surfaceGrid.Grid.Length];
            int xValuesCount = surfaceGrid.Grid.GetLength(0);
            int yValuesCount = surfaceGrid.Grid.GetLength(1);
            for (int i = 0; i < xValuesCount; i++)
            {
                for (int j = 0; j < yValuesCount; j++)
                {
                    vertices[i * yValuesCount + j] = new Vector3(OriginX + i * surfaceGrid.DX, OriginY + surfaceGrid.Grid[i, j] * valueScale, OriginZ + j * surfaceGrid.DY);
                }
            }
            return vertices;
        }

        public static Vector3[] GetEdgeVerticesFromSurface(SurfaceGrid surfaceGrid, float OriginX, float OriginY, float OriginZ, float valueScale)
        {
            int xValuesCount = surfaceGrid.Grid.GetLength(0);
            int yValuesCount = surfaceGrid.Grid.GetLength(1);
            Vector3[] vertices = new Vector3[xValuesCount * 2 + yValuesCount * 2 - 4];
            int vertexId = 0;
            for (int i = 0; i < xValuesCount - 1; i++)
            {
                vertices[vertexId] = new Vector3(OriginX + i * surfaceGrid.DX, OriginY + surfaceGrid.Grid[i, 0] * valueScale, OriginZ + 0 * surfaceGrid.DY);
                vertexId++;
            }
            for (int j = 0; j < yValuesCount - 1; j++)
            {
                vertices[vertexId] = new Vector3(OriginX + (xValuesCount - 1) * surfaceGrid.DX, OriginY + surfaceGrid.Grid[xValuesCount - 1, j] * valueScale, OriginZ + j * surfaceGrid.DY);
                vertexId++;
            }
            for (int i = xValuesCount - 1; i > 0; i--)
            {
                vertices[vertexId] = new Vector3(OriginX + i * surfaceGrid.DX, OriginY + surfaceGrid.Grid[i, yValuesCount - 1] * valueScale, OriginZ + (yValuesCount - 1) * surfaceGrid.DY);
                vertexId++;
            }
            for (int j = yValuesCount - 1; j > 0; j--)
            {
                vertices[vertexId] = new Vector3(OriginX + 0 * surfaceGrid.DX, OriginY + surfaceGrid.Grid[0, j] * valueScale, OriginZ + j * surfaceGrid.DY);
                vertexId++;
            }
            return vertices;
        }

        public static Vector2[] GetEdgePaletteUVFromSurface(SurfaceGrid surfaceGrid, float fromValue, float toValue)
        {
            float minValue = Mathf.Min(fromValue, toValue);
            float maxValue = Mathf.Max(fromValue, toValue);

            int xValuesCount = surfaceGrid.Grid.GetLength(0);
            int yValuesCount = surfaceGrid.Grid.GetLength(1);
            Vector2[] uv = new Vector2[xValuesCount * 2 + yValuesCount * 2 - 4];

            int vertexId = 0;

            for (int i = 0; i < xValuesCount - 1; i++)
            {
                int j = 0;
                float value = surfaceGrid.Grid[i, j];
                if (value < minValue) value = minValue;
                if (value > maxValue) value = maxValue;
                uv[vertexId] = new Vector2(0, (value - fromValue) / (toValue - fromValue));
                vertexId++;
            }
            for (int j = 0; j < yValuesCount - 1; j++)
            {
                int i = xValuesCount - 1;
                float value = surfaceGrid.Grid[i, j];
                if (value < minValue) value = minValue;
                if (value > maxValue) value = maxValue;
                uv[vertexId] = new Vector2(0, (value - fromValue) / (toValue - fromValue));
                vertexId++;
            }
            for (int i = xValuesCount - 1; i > 0; i--)
            {
                int j = yValuesCount - 1;
                float value = surfaceGrid.Grid[i, j];
                if (value < minValue) value = minValue;
                if (value > maxValue) value = maxValue;
                uv[vertexId] = new Vector2(0, (value - fromValue) / (toValue - fromValue));
                vertexId++;
            }
            for (int j = yValuesCount - 1; j > 0; j--)
            {
                int i = 0;
                float value = surfaceGrid.Grid[i, j];
                if (value < minValue) value = minValue;
                if (value > maxValue) value = maxValue;
                uv[vertexId] = new Vector2(0, (value - fromValue) / (toValue - fromValue));
                vertexId++;
            }
            return uv;
        }

        public static Vector2[] GetPaletteUVFromSurface(SurfaceGrid surfaceGrid, float fromValue, float toValue)
        {
            Vector2[] uv = new Vector2[surfaceGrid.Grid.Length];
            int xValuesCount = surfaceGrid.Grid.GetLength(0);
            int yValuesCount = surfaceGrid.Grid.GetLength(1);
            if (fromValue == toValue)
            {
                for (int i = 0; i < xValuesCount; i++)
                {
                    for (int j = 0; j < yValuesCount; j++)
                    {
                        uv[i * yValuesCount + j] = Vector2.zero;
                    }
                }
                return uv;
            }

            float minValue = Mathf.Min(fromValue, toValue);
            float maxValue = Mathf.Max(fromValue, toValue);
            for (int i = 0; i < xValuesCount; i++)
            {
                for (int j = 0; j < yValuesCount; j++)
                {
                    float value = surfaceGrid.Grid[i, j];
                    if (value < minValue) value = minValue;
                    if (value > maxValue) value = maxValue;
                    uv[i * yValuesCount + j] = new Vector2(0, (value - fromValue) / (toValue - fromValue));
                }
            }
            return uv;
        }

        public static int[] GetTrianglesFromSurface(SurfaceGrid surfaceGrid, SurfaceVisibilityType visibility)
        {
            int[] triangles;
            int xValuesCount = surfaceGrid.Grid.GetLength(0);
            int yValuesCount = surfaceGrid.Grid.GetLength(1);
            int cellCount = (xValuesCount - 1) * (yValuesCount - 1);
            if (visibility == SurfaceVisibilityType.Both)
            {
                triangles = new int[cellCount * 4 * 3];
                for (int i = 0; i < xValuesCount - 1; i++)
                {
                    for (int j = 0; j < yValuesCount - 1; j++)
                    {
                        //Normal
                        triangles[(i * (yValuesCount - 1) + j) * 3] = i * yValuesCount + j; // (i,j)
                        triangles[(i * (yValuesCount - 1) + j) * 3 + 1] = i * yValuesCount + j + 1; // (i,j+1)
                        triangles[(i * (yValuesCount - 1) + j) * 3 + 2] = (i + 1) * yValuesCount + j + 1; // (i+1,j+1)

                        triangles[cellCount * 3 + (i * (yValuesCount - 1) + j) * 3] = i * yValuesCount + j; // (i,j)
                        triangles[cellCount * 3 + (i * (yValuesCount - 1) + j) * 3 + 1] = (i + 1) * yValuesCount + j + 1; // (i+1,j+1)
                        triangles[cellCount * 3 + (i * (yValuesCount - 1) + j) * 3 + 2] = (i + 1) * yValuesCount + j; // (i+1,j)

                        //Inverted
                        triangles[cellCount * 3 * 2 + (i * (yValuesCount - 1) + j) * 3] = i * yValuesCount + j; // (i,j)
                        triangles[cellCount * 3 * 2 + (i * (yValuesCount - 1) + j) * 3 + 1] = (i + 1) * yValuesCount + j + 1; // (i+1,j+1)
                        triangles[cellCount * 3 * 2 + (i * (yValuesCount - 1) + j) * 3 + 2] = i * yValuesCount + j + 1; // (i,j+1)

                        triangles[cellCount * 3 * 3 + (i * (yValuesCount - 1) + j) * 3] = i * yValuesCount + j; // (i,j)
                        triangles[cellCount * 3 * 3 + (i * (yValuesCount - 1) + j) * 3 + 1] = (i + 1) * yValuesCount + j; // (i+1,j)
                        triangles[cellCount * 3 * 3 + (i * (yValuesCount - 1) + j) * 3 + 2] = (i + 1) * yValuesCount + j + 1; // (i+1,j+1)
                    }
                }
            }
            else
            if (visibility == SurfaceVisibilityType.Normal)
            {
                triangles = new int[cellCount * 2 * 3];
                for (int i = 0; i < xValuesCount - 1; i++)
                {
                    for (int j = 0; j < yValuesCount - 1; j++)
                    {
                        //Normal
                        triangles[(i * (yValuesCount - 1) + j) * 3] = i * yValuesCount + j; // (i,j)
                        triangles[(i * (yValuesCount - 1) + j) * 3 + 1] = i * yValuesCount + j + 1; // (i,j+1)
                        triangles[(i * (yValuesCount - 1) + j) * 3 + 2] = (i + 1) * yValuesCount + j + 1; // (i+1,j+1)

                        triangles[cellCount * 3 + (i * (yValuesCount - 1) + j) * 3] = i * yValuesCount + j; // (i,j)
                        triangles[cellCount * 3 + (i * (yValuesCount - 1) + j) * 3 + 1] = (i + 1) * yValuesCount + j + 1; // (i+1,j+1)
                        triangles[cellCount * 3 + (i * (yValuesCount - 1) + j) * 3 + 2] = (i + 1) * yValuesCount + j; // (i+1,j)
                    }
                }
            }
            else
            {
                triangles = new int[cellCount * 2 * 3];
                for (int i = 0; i < xValuesCount - 1; i++)
                {
                    for (int j = 0; j < yValuesCount - 1; j++)
                    {
                        //Inverted
                        triangles[(i * (yValuesCount - 1) + j) * 3] = i * yValuesCount + j; // (i,j)
                        triangles[(i * (yValuesCount - 1) + j) * 3 + 1] = (i + 1) * yValuesCount + j + 1; // (i+1,j+1)
                        triangles[(i * (yValuesCount - 1) + j) * 3 + 2] = i * yValuesCount + j + 1; // (i,j+1)

                        triangles[cellCount * 3 + (i * (yValuesCount - 1) + j) * 3] = i * yValuesCount + j; // (i,j)
                        triangles[cellCount * 3 + (i * (yValuesCount - 1) + j) * 3 + 1] = (i + 1) * yValuesCount + j; // (i+1,j)
                        triangles[cellCount * 3 + (i * (yValuesCount - 1) + j) * 3 + 2] = (i + 1) * yValuesCount + j + 1; // (i+1,j+1)
                    }
                }

            }
            return triangles;
        }
    }
}
