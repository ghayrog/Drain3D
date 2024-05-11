using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Surface
{
    public static class MeshImpostorEditor
    {
        public static Mesh ConvertImpostorToMesh(MeshImpostor meshImpostor, bool recalculateNormals)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = meshImpostor.vertices;
            mesh.uv = meshImpostor.uv;
            mesh.triangles = meshImpostor.triangles;

            if (recalculateNormals)
            {
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.RecalculateTangents();
            }

            return mesh;
        }

        public static MeshImpostor Join(MeshImpostor mesh1, MeshImpostor mesh2)
        {
            if (mesh1 == null)
            {
                return mesh2;
            }
            if (mesh2 == null)
            {
                return mesh1;
            }

            Vector3[] vertices = new Vector3[mesh1.vertices.Length + mesh2.vertices.Length];
            int[] triangles = new int[mesh1.triangles.Length + mesh2.triangles.Length];
            Vector2[] uv = new Vector2[mesh1.uv.Length + mesh2.uv.Length];

            int mesh1VerticesCount = mesh1.vertices.Length;
            int mesh2VerticesCount = mesh2.vertices.Length;
            int mesh1TrianglesCount = mesh1.triangles.Length;
            int mesh2TrianglesCount = mesh2.triangles.Length;
            int mesh1UVCount = mesh1.uv.Length;
            int mesh2UVCount = mesh2.uv.Length;

            for (int i = 0; i < mesh1VerticesCount; i++)
            {
                vertices[i] = mesh1.vertices[i];
            }
            for (int i = 0; i < mesh2VerticesCount; i++)
            {
                vertices[mesh1VerticesCount + i] = mesh2.vertices[i];
            }

            for (int i = 0; i < mesh1UVCount; i++)
            {
                uv[i] = mesh1.uv[i];
            }
            for (int i = 0; i < mesh2UVCount; i++)
            {
                uv[mesh1UVCount + i] = mesh2.uv[i];
            }

            for (int i = 0; i < mesh1TrianglesCount; i++)
            {
                triangles[i] = mesh1.triangles[i];
            }
            for (int i = 0; i < mesh2TrianglesCount; i++)
            {
                triangles[mesh1TrianglesCount + i] = mesh2.triangles[i] + mesh1VerticesCount;
            }

            var mesh = new MeshImpostor();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            return mesh;
        }

        public static async Task<MeshImpostor> MakeMaskedMeshFromSurfacesAsync(float X, float Y, float Z, SurfaceGrid structure, float structureMaxValue, SurfaceGrid thickness, bool[,] visibilityMask, float fluidContact)
        {
            return await Task.Run(() => MakeMaskedMeshFromSurfaces(X, Y, Z, structure, structureMaxValue, thickness, visibilityMask, fluidContact));
        }

        public static MeshImpostor MakeMaskedMeshFromSurfaces(float X, float Y, float Z, SurfaceGrid structure, float structureMaxValue, SurfaceGrid thickness, bool[,] visibilityMask, float fluidContact)
        {
            int verticesCount = 0;
            bool[,] verticesMask = new bool[structure.Grid.GetLength(0), structure.Grid.GetLength(1)];
            int[,] verticesIndices = new int[structure.Grid.GetLength(0), structure.Grid.GetLength(1)];
            for (int i = 0; i < structure.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < structure.Grid.GetLength(1); j++)
                {
                    bool isVertexVisible = false;
                    if (i > 0 && j > 0 && visibilityMask[i - 1, j - 1]) isVertexVisible = true;
                    if (i > 0 && j < structure.Grid.GetLength(1) - 1 && visibilityMask[i - 1, j]) isVertexVisible = true;
                    if (j > 0 && i < structure.Grid.GetLength(0) - 1 && visibilityMask[i, j - 1]) isVertexVisible = true;
                    if (i < structure.Grid.GetLength(0) - 1 && j < structure.Grid.GetLength(1) - 1 && visibilityMask[i, j]) isVertexVisible = true;
                    if (isVertexVisible)
                    {
                        verticesCount++;
                        verticesMask[i, j] = true;
                    }
                    else
                    {
                        verticesMask[i, j] = false;
                    }
                }
            }

            Vector3[] topVertices = new Vector3[verticesCount];
            Vector3[] bottomVertices = new Vector3[verticesCount];
            Vector2[] topUV = new Vector2[verticesCount];
            Vector2[] bottomUV = new Vector2[verticesCount];

            float fromValue = fluidContact - structureMaxValue;
            float toValue = fluidContact + structureMaxValue;

            int vertexIndex = 0;

            List<int> topTrianglesList = new List<int>();
            List<int> bottomTrianglesList = new List<int>();

            List<Vector3> edgeVertices = new List<Vector3>();
            List<Vector2> edgeUV = new List<Vector2>();
            int[,] edgeVerticesIndices = new int[structure.Grid.GetLength(0), structure.Grid.GetLength(1)];
            List<int> edgeTrianglesList = new List<int>();
            int edgeVertexId = 1;

            for (int i = 0; i < structure.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < structure.Grid.GetLength(1); j++)
                {
                    if (verticesMask[i, j])
                    {
                        float heightValue = structure.Grid[i, j];
                        float bottomHeightValue = heightValue - thickness.Grid[i, j];

                        verticesIndices[i, j] = vertexIndex;

                        topVertices[vertexIndex] = new Vector3(X + i * structure.DX, Y + heightValue, Z + j * structure.DY);
                        bottomVertices[vertexIndex] = new Vector3(X + i * structure.DX, Y + bottomHeightValue, Z + j * structure.DY);

                        if (heightValue < fromValue) heightValue = fromValue;
                        if (heightValue > toValue) heightValue = toValue;

                        if (bottomHeightValue < fromValue) bottomHeightValue = fromValue;
                        if (bottomHeightValue > toValue) bottomHeightValue = toValue;

                        topUV[vertexIndex] = new Vector2(0, (heightValue - fromValue) / (toValue - fromValue));
                        bottomUV[vertexIndex] = new Vector2(0, (bottomHeightValue - fromValue) / (toValue - fromValue));

                        vertexIndex++;
                    }
                }
            }

            for (int i = 0; i < structure.Grid.GetLength(0) - 1; i++)
            {
                if (visibilityMask[i, 0] && verticesMask[i, 0] && verticesMask[i + 1, 0])
                {
                    if (edgeVerticesIndices[i, 0] == 0)
                    {
                        edgeVertices.Add(topVertices[verticesIndices[i, 0]]);
                        edgeUV.Add(topUV[verticesIndices[i, 0]]);
                        edgeVerticesIndices[i, 0] = edgeVertexId;
                        edgeVertexId++;
                        edgeVertices.Add(bottomVertices[verticesIndices[i, 0]]);
                        edgeUV.Add(bottomUV[verticesIndices[i, 0]]);
                        edgeVertexId++;
                    }
                    if (edgeVerticesIndices[i + 1, 0] == 0)
                    {
                        edgeVertices.Add(topVertices[verticesIndices[i + 1, 0]]);
                        edgeUV.Add(topUV[verticesIndices[i + 1, 0]]);
                        edgeVerticesIndices[i + 1, 0] = edgeVertexId;
                        edgeVertexId++;
                        edgeVertices.Add(bottomVertices[verticesIndices[i + 1, 0]]);
                        edgeUV.Add(bottomUV[verticesIndices[i + 1, 0]]);
                        edgeVertexId++;
                    }
                    edgeTrianglesList.Add(edgeVerticesIndices[i, 0] - 1);
                    edgeTrianglesList.Add(edgeVerticesIndices[i + 1, 0] - 1);
                    edgeTrianglesList.Add(edgeVerticesIndices[i, 0]);
                    edgeTrianglesList.Add(edgeVerticesIndices[i, 0]);
                    edgeTrianglesList.Add(edgeVerticesIndices[i + 1, 0] - 1);
                    edgeTrianglesList.Add(edgeVerticesIndices[i + 1, 0]);
                }

                int lastJ = structure.Grid.GetLength(1) - 1;
                if (visibilityMask[i, lastJ - 1] && verticesMask[i, lastJ] && verticesMask[i + 1, lastJ])
                {
                    if (edgeVerticesIndices[i, lastJ] == 0)
                    {
                        edgeVertices.Add(topVertices[verticesIndices[i, lastJ]]);
                        edgeUV.Add(topUV[verticesIndices[i, lastJ]]);
                        edgeVerticesIndices[i, lastJ] = edgeVertexId;
                        edgeVertexId++;
                        edgeVertices.Add(bottomVertices[verticesIndices[i, lastJ]]);
                        edgeUV.Add(bottomUV[verticesIndices[i, lastJ]]);
                        edgeVertexId++;
                    }
                    if (edgeVerticesIndices[i + 1, lastJ] == 0)
                    {
                        edgeVertices.Add(topVertices[verticesIndices[i + 1, lastJ]]);
                        edgeUV.Add(topUV[verticesIndices[i + 1, lastJ]]);
                        edgeVerticesIndices[i + 1, lastJ] = edgeVertexId;
                        edgeVertexId++;
                        edgeVertices.Add(bottomVertices[verticesIndices[i + 1, lastJ]]);
                        edgeUV.Add(bottomUV[verticesIndices[i + 1, lastJ]]);
                        edgeVertexId++;
                    }
                    edgeTrianglesList.Add(edgeVerticesIndices[i, lastJ] - 1);
                    edgeTrianglesList.Add(edgeVerticesIndices[i, lastJ]);
                    edgeTrianglesList.Add(edgeVerticesIndices[i + 1, lastJ] - 1);
                    edgeTrianglesList.Add(edgeVerticesIndices[i, lastJ]);
                    edgeTrianglesList.Add(edgeVerticesIndices[i + 1, lastJ]);
                    edgeTrianglesList.Add(edgeVerticesIndices[i + 1, lastJ] - 1);
                }
            }

            for (int j = 0; j < structure.Grid.GetLength(1) - 1; j++)
            {
                if (visibilityMask[0, j] && verticesMask[0, j] && verticesMask[0, j + 1])
                {
                    if (edgeVerticesIndices[0, j] == 0)
                    {
                        edgeVertices.Add(topVertices[verticesIndices[0, j]]);
                        edgeUV.Add(topUV[verticesIndices[0, j]]);
                        edgeVerticesIndices[0, j] = edgeVertexId;
                        edgeVertexId++;
                        edgeVertices.Add(bottomVertices[verticesIndices[0, j]]);
                        edgeUV.Add(bottomUV[verticesIndices[0, j]]);
                        edgeVertexId++;
                    }
                    if (edgeVerticesIndices[0, j + 1] == 0)
                    {
                        edgeVertices.Add(topVertices[verticesIndices[0, j + 1]]);
                        edgeUV.Add(topUV[verticesIndices[0, j + 1]]);
                        edgeVerticesIndices[0, j + 1] = edgeVertexId;
                        edgeVertexId++;
                        edgeVertices.Add(bottomVertices[verticesIndices[0, j + 1]]);
                        edgeUV.Add(bottomUV[verticesIndices[0, j + 1]]);
                        edgeVertexId++;
                    }
                    edgeTrianglesList.Add(edgeVerticesIndices[0, j] - 1);
                    edgeTrianglesList.Add(edgeVerticesIndices[0, j]);
                    edgeTrianglesList.Add(edgeVerticesIndices[0, j + 1] - 1);
                    edgeTrianglesList.Add(edgeVerticesIndices[0, j]);
                    edgeTrianglesList.Add(edgeVerticesIndices[0, j + 1]);
                    edgeTrianglesList.Add(edgeVerticesIndices[0, j + 1] - 1);
                }

                int lastI = structure.Grid.GetLength(0) - 1;
                if (visibilityMask[lastI - 1, j] && verticesMask[lastI, j] && verticesMask[lastI, j + 1])
                {
                    if (edgeVerticesIndices[lastI, j] == 0)
                    {
                        edgeVertices.Add(topVertices[verticesIndices[lastI, j]]);
                        edgeUV.Add(topUV[verticesIndices[lastI, j]]);
                        edgeVerticesIndices[lastI, j] = edgeVertexId;
                        edgeVertexId++;
                        edgeVertices.Add(bottomVertices[verticesIndices[lastI, j]]);
                        edgeUV.Add(bottomUV[verticesIndices[lastI, j]]);
                        edgeVertexId++;
                    }
                    if (edgeVerticesIndices[lastI, j + 1] == 0)
                    {
                        edgeVertices.Add(topVertices[verticesIndices[lastI, j + 1]]);
                        edgeUV.Add(topUV[verticesIndices[lastI, j + 1]]);
                        edgeVerticesIndices[lastI, j + 1] = edgeVertexId;
                        edgeVertexId++;
                        edgeVertices.Add(bottomVertices[verticesIndices[lastI, j + 1]]);
                        edgeUV.Add(bottomUV[verticesIndices[lastI, j + 1]]);
                        edgeVertexId++;
                    }
                    edgeTrianglesList.Add(edgeVerticesIndices[lastI, j] - 1);
                    edgeTrianglesList.Add(edgeVerticesIndices[lastI, j + 1] - 1);
                    edgeTrianglesList.Add(edgeVerticesIndices[lastI, j]);
                    edgeTrianglesList.Add(edgeVerticesIndices[lastI, j]);
                    edgeTrianglesList.Add(edgeVerticesIndices[lastI, j + 1] - 1);
                    edgeTrianglesList.Add(edgeVerticesIndices[lastI, j + 1]);
                }
            }

            for (int i = 0; i < structure.Grid.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < structure.Grid.GetLength(1) - 1; j++)
                {
                    if (visibilityMask[i, j] && verticesMask[i, j] && verticesMask[i + 1, j + 1] && verticesMask[i, j + 1] && verticesMask[i + 1, j])
                    {
                        topTrianglesList.Add(verticesIndices[i, j]);
                        topTrianglesList.Add(verticesIndices[i, j + 1]);
                        topTrianglesList.Add(verticesIndices[i + 1, j + 1]);
                        topTrianglesList.Add(verticesIndices[i, j]);
                        topTrianglesList.Add(verticesIndices[i + 1, j + 1]);
                        topTrianglesList.Add(verticesIndices[i + 1, j]);

                        bottomTrianglesList.Add(verticesIndices[i, j]);
                        bottomTrianglesList.Add(verticesIndices[i + 1, j + 1]);
                        bottomTrianglesList.Add(verticesIndices[i, j + 1]);
                        bottomTrianglesList.Add(verticesIndices[i, j]);
                        bottomTrianglesList.Add(verticesIndices[i + 1, j]);
                        bottomTrianglesList.Add(verticesIndices[i + 1, j + 1]);
                    }
                    if (i > 0 && verticesMask[i, j] && verticesMask[i, j + 1] && visibilityMask[i, j] && !visibilityMask[i - 1, j])
                    {
                        if (edgeVerticesIndices[i, j] == 0)
                        {
                            edgeVertices.Add(topVertices[verticesIndices[i, j]]);
                            edgeUV.Add(topUV[verticesIndices[i, j]]);
                            edgeVerticesIndices[i, j] = edgeVertexId;
                            edgeVertexId++;
                            edgeVertices.Add(bottomVertices[verticesIndices[i, j]]);
                            edgeUV.Add(bottomUV[verticesIndices[i, j]]);
                            edgeVertexId++;
                        }
                        if (edgeVerticesIndices[i, j + 1] == 0)
                        {
                            edgeVertices.Add(topVertices[verticesIndices[i, j + 1]]);
                            edgeUV.Add(topUV[verticesIndices[i, j + 1]]);
                            edgeVerticesIndices[i, j + 1] = edgeVertexId;
                            edgeVertexId++;
                            edgeVertices.Add(bottomVertices[verticesIndices[i, j + 1]]);
                            edgeUV.Add(bottomUV[verticesIndices[i, j + 1]]);
                            edgeVertexId++;
                        }
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j] - 1);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j]);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j + 1] - 1);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j]);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j + 1]);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j + 1] - 1);
                    }
                    if (i > 0 && verticesMask[i, j] && verticesMask[i, j + 1] && !visibilityMask[i, j] && visibilityMask[i - 1, j])
                    {
                        if (edgeVerticesIndices[i, j] == 0)
                        {
                            edgeVertices.Add(topVertices[verticesIndices[i, j]]);
                            edgeUV.Add(topUV[verticesIndices[i, j]]);
                            edgeVerticesIndices[i, j] = edgeVertexId;
                            edgeVertexId++;
                            edgeVertices.Add(bottomVertices[verticesIndices[i, j]]);
                            edgeUV.Add(bottomUV[verticesIndices[i, j]]);
                            edgeVertexId++;
                        }
                        if (edgeVerticesIndices[i, j + 1] == 0)
                        {
                            edgeVertices.Add(topVertices[verticesIndices[i, j + 1]]);
                            edgeUV.Add(topUV[verticesIndices[i, j + 1]]);
                            edgeVerticesIndices[i, j + 1] = edgeVertexId;
                            edgeVertexId++;
                            edgeVertices.Add(bottomVertices[verticesIndices[i, j + 1]]);
                            edgeUV.Add(bottomUV[verticesIndices[i, j + 1]]);
                            edgeVertexId++;
                        }
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j] - 1);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j + 1] - 1);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j]);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j]);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j + 1] - 1);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j + 1]);
                    }
                    if (j > 0 && verticesMask[i, j] && verticesMask[i + 1, j] && visibilityMask[i, j] && !visibilityMask[i, j - 1])
                    {
                        if (edgeVerticesIndices[i, j] == 0)
                        {
                            edgeVertices.Add(topVertices[verticesIndices[i, j]]);
                            edgeUV.Add(topUV[verticesIndices[i, j]]);
                            edgeVerticesIndices[i, j] = edgeVertexId;
                            edgeVertexId++;
                            edgeVertices.Add(bottomVertices[verticesIndices[i, j]]);
                            edgeUV.Add(bottomUV[verticesIndices[i, j]]);
                            edgeVertexId++;
                        }
                        if (edgeVerticesIndices[i + 1, j] == 0)
                        {
                            edgeVertices.Add(topVertices[verticesIndices[i + 1, j]]);
                            edgeUV.Add(topUV[verticesIndices[i + 1, j]]);
                            edgeVerticesIndices[i + 1, j] = edgeVertexId;
                            edgeVertexId++;
                            edgeVertices.Add(bottomVertices[verticesIndices[i + 1, j]]);
                            edgeUV.Add(bottomUV[verticesIndices[i + 1, j]]);
                            edgeVertexId++;
                        }
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j] - 1);
                        edgeTrianglesList.Add(edgeVerticesIndices[i + 1, j] - 1);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j]);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j]);
                        edgeTrianglesList.Add(edgeVerticesIndices[i + 1, j] - 1);
                        edgeTrianglesList.Add(edgeVerticesIndices[i + 1, j]);
                    }
                    if (j > 0 && verticesMask[i, j] && verticesMask[i + 1, j] && !visibilityMask[i, j] && visibilityMask[i, j - 1])
                    {
                        if (edgeVerticesIndices[i, j] == 0)
                        {
                            edgeVertices.Add(topVertices[verticesIndices[i, j]]);
                            edgeUV.Add(topUV[verticesIndices[i, j]]);
                            edgeVerticesIndices[i, j] = edgeVertexId;
                            edgeVertexId++;
                            edgeVertices.Add(bottomVertices[verticesIndices[i, j]]);
                            edgeUV.Add(bottomUV[verticesIndices[i, j]]);
                            edgeVertexId++;
                        }
                        if (edgeVerticesIndices[i + 1, j] == 0)
                        {
                            edgeVertices.Add(topVertices[verticesIndices[i + 1, j]]);
                            edgeUV.Add(topUV[verticesIndices[i + 1, j]]);
                            edgeVerticesIndices[i + 1, j] = edgeVertexId;
                            edgeVertexId++;
                            edgeVertices.Add(bottomVertices[verticesIndices[i + 1, j]]);
                            edgeUV.Add(bottomUV[verticesIndices[i + 1, j]]);
                            edgeVertexId++;
                        }
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j] - 1);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j]);
                        edgeTrianglesList.Add(edgeVerticesIndices[i + 1, j] - 1);
                        edgeTrianglesList.Add(edgeVerticesIndices[i, j]);
                        edgeTrianglesList.Add(edgeVerticesIndices[i + 1, j]);
                        edgeTrianglesList.Add(edgeVerticesIndices[i + 1, j] - 1);
                    }
                }
            }

            var mesh = new MeshImpostor();
            mesh.vertices = topVertices;
            mesh.triangles = topTrianglesList.ToArray();
            mesh.uv = topUV;

            var bottomMesh = new MeshImpostor();
            bottomMesh.vertices = bottomVertices;
            bottomMesh.triangles = bottomTrianglesList.ToArray();
            bottomMesh.uv = bottomUV;

            var edgesMesh = new MeshImpostor();
            edgesMesh.vertices = edgeVertices.ToArray();
            edgesMesh.triangles = edgeTrianglesList.ToArray();
            edgesMesh.uv = edgeUV.ToArray();

            return Join(Join(mesh, bottomMesh), edgesMesh);
        }
    }
};