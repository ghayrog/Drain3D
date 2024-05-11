using UnityEngine;
using Game;

namespace Reservoir
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    internal sealed class MeshController : MonoBehaviour
    {
        [SerializeField]
        private Material _paletteMaterial;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public LoadingPriority Priority => LoadingPriority.Medium;

        public void OnGameInitialize()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        internal void SetMesh(Mesh mesh)
        {
            _meshFilter.mesh = mesh;
            _meshFilter.mesh.RecalculateBounds();
            _meshFilter.mesh.RecalculateNormals();
            _meshRenderer.material= _paletteMaterial;
        }
    }
}
