using UnityEngine;
using DI;
using Game;
using Surface;

namespace Reservoir
{
    public sealed class ReservoirManager : MonoBehaviour,
        IGameInitializeListener
    {
        private const float VERY_LOW_PERM = 1f;
        private const float LOW_PERM = 20f;
        private const float MEDIUM_PERM = 50f;
        private const float HIGH_PERM = 200f;
        private const float VERY_HIGH_PERM = 1000f;
        private const float LOW_PORO = 0.15f;
        private const float HIGH_PORO = 0.25f;
        private ReservoirLayer[] _reservoirLayers;

        [SerializeField]
        private Transform[] _reservoirDepthMarkerks;

        [SerializeField]
        private MeshController[] _meshControllers;

        [SerializeField]
        private Transform[] _contacts;

        private int _reservoirCount;
        private bool _isInitialized = false;

        public LoadingPriority Priority => LoadingPriority.Medium;

        public bool IsInitialized => _isInitialized;

        public void OnGameInitialize()
        {
            foreach (var meshController in _meshControllers)
            { 
                meshController.OnGameInitialize();
            }
            GenerateReservoirs();
            _isInitialized = true;
        }

        private void GenerateReservoirs()
        {
            Debug.Log("Generating reservoirs...");
            _reservoirCount = _reservoirDepthMarkerks.Length;
            _reservoirLayers = new ReservoirLayer[_reservoirCount];
            for (int i = 0; i < _reservoirCount; i++)
            {
                float productiveFraction = Random.Range(0.25f, 0.6f);
                float reservoirHeight = Random.Range(20f, 30f);
                float porosity = Random.Range(LOW_PORO, HIGH_PORO);
                Debug.Log($"Reservoir parameters: {productiveFraction} Productive Fraction, {reservoirHeight} Height, {porosity} Porosity");
                _reservoirLayers[i] = new();
                _reservoirLayers[i].GenerateRandomReservoir(productiveFraction, reservoirHeight, LOW_PERM, HIGH_PERM, porosity, 1 - productiveFraction / 2, true);
                _contacts[i].localPosition = Vector3.up * _reservoirLayers[i].Contact;
                _meshControllers[i].SetMesh(_reservoirLayers[i].CreateReservoirMeshMasked());
            }
        }
    }
}
