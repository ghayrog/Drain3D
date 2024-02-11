using UnityEngine;

namespace Surface
{
    public static class MeshEditor
     {
        public static Mesh Join(Mesh mesh1, Mesh mesh2, bool recalculateNormals)
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

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            if (recalculateNormals)
            {
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.RecalculateTangents();
            }

            return mesh;

        }

        public static Mesh MakeMaskedMeshFromSurfaces(float X, float Y, float Z, SurfaceGrid structure, float structureMaxValue, SurfaceGrid thickness, bool[,] visibilityMask, float fluidContact, bool recalculateNormals)
        { 
            Mesh mesh = new Mesh();
            int verticesCount = 0;
            bool[,] verticesMask = new bool[structure.Grid.GetLength(0), structure.Grid.GetLength(1)];
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
            for (int i = 0; i < structure.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < structure.Grid.GetLength(1); j++)
                {
                }
            }
            
            Vector3[] topVertices = new Vector3[verticesCount];
            Vector3[] bottomVertices = new Vector3[verticesCount];
            return mesh;
        }

        public static Mesh MakeVolumeMesh(float X, float Y, float Z, SurfaceGrid structure, float structureMaxValue, SurfaceGrid thickness, float fluidContact, bool recalculateNormals)
        {
            Vector3[] topVertices = SurfaceEditor.GetVerticesFromSurface(structure, X, Y, Z, 1);
            int[] topTriangles = SurfaceEditor.GetTrianglesFromSurface(structure, SurfaceVisibilityType.Normal);
            Vector2[] topUv = SurfaceEditor.GetPaletteUVFromSurface(structure, fluidContact - structureMaxValue, fluidContact + structureMaxValue);

            Mesh mesh = new Mesh();
            mesh.vertices = topVertices;
            mesh.triangles = topTriangles;
            mesh.uv = topUv;

            SurfaceGrid bottom = structure.Subtract(thickness);
 
            Vector3[] bottomVertices = SurfaceEditor.GetVerticesFromSurface(bottom, X, Y, Z, 1);
            int[] bottomTriangles = SurfaceEditor.GetTrianglesFromSurface(bottom, SurfaceVisibilityType.Inverted);
            Vector2[] bottomUv = SurfaceEditor.GetPaletteUVFromSurface(bottom, fluidContact - structureMaxValue, fluidContact + structureMaxValue);

            Mesh bottomMesh = new Mesh();
            bottomMesh.vertices = bottomVertices;
            bottomMesh.triangles = bottomTriangles;
            bottomMesh.uv = bottomUv;

            mesh = Join(mesh, bottomMesh, recalculateNormals);

            Mesh edgeMesh = new Mesh();
            Vector3[] topEdgeVertices = SurfaceEditor.GetEdgeVerticesFromSurface(structure, X, Y, Z, 1);
            int edgeVerticesCount = topEdgeVertices.Length;
            Vector3[] bottomEdgeVertices = SurfaceEditor.GetEdgeVerticesFromSurface(bottom, X, Y, Z, 1);
            Vector3[] edgeVertices = new Vector3[edgeVerticesCount * 2];
            Vector2[] edgeUV = new Vector2[edgeVerticesCount * 2];
            Vector2[] edgeUVTop = SurfaceEditor.GetEdgePaletteUVFromSurface(structure, fluidContact - structureMaxValue, fluidContact + structureMaxValue);
            Vector2[] edgeUVBottom = SurfaceEditor.GetEdgePaletteUVFromSurface(bottom, fluidContact - structureMaxValue, fluidContact + structureMaxValue);

            for (int i = 0; i < edgeVerticesCount; i++)
            {
                edgeVertices[i] = topEdgeVertices[i];
                edgeUV[i] = edgeUVTop[i];

                edgeVertices[i + edgeVerticesCount] = bottomEdgeVertices[i];
                edgeUV[i + edgeVerticesCount] = edgeUVBottom[i];
            }
            edgeMesh.vertices = edgeVertices;

            int[] edgeTriangles = new int[edgeVerticesCount * 6];
            for (int i = 0; i < edgeVerticesCount - 1; i++)
            {
                int bottomI = i + edgeVerticesCount;
                edgeTriangles[i * 6] = i;
                edgeTriangles[i * 6 + 1] = i + 1;
                edgeTriangles[i * 6 + 2] = bottomI + 1;
                edgeTriangles[i * 6 + 3] = i;
                edgeTriangles[i * 6 + 4] = bottomI + 1;
                edgeTriangles[i * 6 + 5] = bottomI;
            }
            edgeTriangles[(edgeVerticesCount - 1) * 6] = edgeVerticesCount - 1;
            edgeTriangles[(edgeVerticesCount - 1) * 6 + 1] = 0;
            edgeTriangles[(edgeVerticesCount - 1) * 6 + 2] = edgeVerticesCount;
            edgeTriangles[(edgeVerticesCount - 1) * 6 + 3] = edgeVerticesCount - 1;
            edgeTriangles[(edgeVerticesCount - 1) * 6 + 4] = edgeVerticesCount;
            edgeTriangles[(edgeVerticesCount - 1) * 6 + 5] = edgeVerticesCount + edgeVerticesCount - 1;
            edgeMesh.triangles = edgeTriangles;

            edgeMesh.uv = edgeUV;

            return Join(mesh, edgeMesh, recalculateNormals);
        }
    }
};