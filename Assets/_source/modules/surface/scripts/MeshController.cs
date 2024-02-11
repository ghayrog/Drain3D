using UnityEngine;

namespace Surface
{
    [RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
    internal sealed class MeshController : MonoBehaviour
    {
        [SerializeField]
        private Material _paletteMaterial;

        private MeshFilter _meshFilter;

        internal void JoinMesh(Mesh newMesh)
        {
            if (_meshFilter == null)
            {
                _meshFilter = gameObject.GetComponent<MeshFilter>();
                _meshFilter.mesh = newMesh;
                _meshFilter.mesh.RecalculateNormals();
                gameObject.GetComponent<MeshRenderer>().material = _paletteMaterial;
                return;
            }

            Vector3[] vertices = new Vector3[_meshFilter.mesh.vertices.Length + newMesh.vertices.Length];
            int[] triangles = new int[_meshFilter.mesh.triangles.Length + newMesh.triangles.Length];
            Vector2[] uv = new Vector2[_meshFilter.mesh.uv.Length + newMesh.uv.Length];

            for (int i = 0; i < _meshFilter.mesh.vertices.Length; i++)
            {
                vertices[i] = _meshFilter.mesh.vertices[i];
            }
            for (int i = 0; i < newMesh.vertices.Length; i++)
            {
                vertices[_meshFilter.mesh.vertices.Length + i] = newMesh.vertices[i];
            }

            for (int i = 0; i < _meshFilter.mesh.uv.Length; i++)
            {
                uv[i] = _meshFilter.mesh.uv[i];
            }
            for (int i = 0; i < newMesh.uv.Length; i++)
            {
                uv[_meshFilter.mesh.uv.Length + i] = newMesh.uv[i];
            }

            for (int i = 0; i < _meshFilter.mesh.triangles.Length; i++)
            {
                triangles[i] = _meshFilter.mesh.triangles[i];
            }
            for (int i = 0; i < newMesh.triangles.Length; i++)
            {
                triangles[_meshFilter.mesh.triangles.Length + i] = newMesh.triangles[i] + _meshFilter.mesh.vertices.Length;
            }
            _meshFilter.mesh.vertices = vertices;
            _meshFilter.mesh.uv = uv;
            _meshFilter.mesh.triangles = triangles;
            _meshFilter.mesh.RecalculateNormals();
            _meshFilter.mesh.RecalculateBounds();
        }

        internal void MakeThickMesh(float X, float Y, float Z, SurfaceGrid structure, SurfaceGrid thickness)
        {

            Vector3[] topVertices = SurfaceEditor.GetVerticesFromSurface(structure, X,Y,Z, 1);
            int[] topTriangles = SurfaceEditor.GetTrianglesFromSurface(structure, SurfaceVisibilityType.Normal);
            Vector2[] topUv = SurfaceEditor.GetPaletteUVFromSurface(structure, 0, 0);

            Mesh mesh = new Mesh();
            mesh.vertices = topVertices;
            mesh.triangles = topTriangles;
            mesh.uv = topUv;

            _meshFilter = gameObject.GetComponent<MeshFilter>();
            _meshFilter.mesh = mesh;
            _meshFilter.mesh.RecalculateNormals();
            gameObject.GetComponent<MeshRenderer>().material = _paletteMaterial;

            SurfaceGrid bottom = structure.Subtract(thickness);
            Debug.Log(bottom == null);
            Vector3[] bottomVertices = SurfaceEditor.GetVerticesFromSurface(bottom, X, Y, Z, 1);
            int[] bottomTriangles = SurfaceEditor.GetTrianglesFromSurface(bottom, SurfaceVisibilityType.Inverted);
            Vector2[] bottomUv = SurfaceEditor.GetPaletteUVFromSurface(bottom, 0, 0);

            Mesh bottomMesh = new Mesh();
            bottomMesh.vertices = bottomVertices;
            bottomMesh.triangles = bottomTriangles;
            bottomMesh.uv = bottomUv;

            JoinMesh(bottomMesh);

            Mesh edgeMesh = new Mesh();
            Vector3[] topEdgeVertices = SurfaceEditor.GetEdgeVerticesFromSurface(structure, X, Y, Z, 1);
            int edgeVerticesCount = topEdgeVertices.Length;
            Vector3[] bottomEdgeVertices = SurfaceEditor.GetEdgeVerticesFromSurface(bottom, X, Y, Z, 1);
            Vector3[] edgeVertices = new Vector3[edgeVerticesCount * 2];
            for (int i = 0; i < edgeVerticesCount; i++)
            {
                edgeVertices[i] = topEdgeVertices[i];
                edgeVertices[i + edgeVerticesCount] = bottomEdgeVertices[i];
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
            edgeMesh.triangles= edgeTriangles;

            Vector2[] edgeUV = new Vector2[edgeVerticesCount * 2];
            edgeMesh.uv= edgeUV;

            JoinMesh(edgeMesh);
        }

        internal void MakeFlatMesh(float X, float Y, float Z, float valueScale, SurfaceGrid surfaceGrid, float fromValue, float toValaue)
        {
            Vector3[] vertices = SurfaceEditor.GetVerticesFromSurface(surfaceGrid,X, Y, Z, valueScale);
            int[] triangles = SurfaceEditor.GetTrianglesFromSurface(surfaceGrid,SurfaceVisibilityType.Normal);
            Vector2[] uv = SurfaceEditor.GetPaletteUVFromSurface(surfaceGrid, fromValue, toValaue);

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;

            _meshFilter = gameObject.GetComponent<MeshFilter>();
            _meshFilter.mesh = mesh;
            _meshFilter.mesh.RecalculateNormals();
            gameObject.GetComponent<MeshRenderer>().material = _paletteMaterial;
        }

        internal void UpdateFlatMesh(float X, float Y, float Z, float valueScale, SurfaceGrid surfaceGrid, float fromValue, float toValaue)
        {
            Vector3[] vertices = SurfaceEditor.GetVerticesFromSurface(surfaceGrid, X, Y, Z, valueScale);
            Vector2[] uv = SurfaceEditor.GetPaletteUVFromSurface(surfaceGrid, fromValue, toValaue);

            _meshFilter.mesh.vertices = vertices;
            _meshFilter.mesh.uv = uv;
            _meshFilter.mesh.RecalculateNormals();
        }

        internal void RemakeFlatMesh(float X, float Y, float Z, float valueScale, SurfaceGrid surfaceGrid, float fromValue, float toValaue)
        {
            Vector3[] vertices = SurfaceEditor.GetVerticesFromSurface(surfaceGrid, X, Y, Z, valueScale);
            int[] triangles = SurfaceEditor.GetTrianglesFromSurface(surfaceGrid, SurfaceVisibilityType.Normal);
            Vector2[] uv = SurfaceEditor.GetPaletteUVFromSurface(surfaceGrid, fromValue, toValaue);

            _meshFilter.mesh.vertices = vertices;
            _meshFilter.mesh.uv = uv;
            _meshFilter.mesh.triangles = triangles;
            _meshFilter.mesh.RecalculateNormals();

        }
    }
}
