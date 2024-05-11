using UnityEngine;
using DI;
using Game;
using Surface;
using Hydraulics;
using Wallet;

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
        public int ReservoirCount => _reservoirCount;

        private WalletManager _walletManager;

        [Inject]
        public void Construct(WalletManager walletManager)
        {
            _walletManager = walletManager;
        }

        public void OnGameInitialize()
        {
            foreach (var meshController in _meshControllers)
            {
                meshController.OnGameInitialize();
            }
            GenerateReservoirs();
            _isInitialized = true;
        }

        public void ExploreLayers(int i, int j)
        {
            if (i < 0 || j < 0 || i >= _reservoirLayers[0].SizeX || j >= _reservoirLayers[0].SizeY) return;
            for (int k = 0; k < _reservoirLayers.Length; k++)
            {
                _reservoirLayers[k].SetCellMask(i, j, true);
            }
        }

        public void DrillLayers(int i, int j)
        {
            if (i < 0 || j < 0 || i >= _reservoirLayers[0].SizeX || j >= _reservoirLayers[0].SizeY) return;
            for (int k = 0; k < _reservoirLayers.Length; k++)
            {
                _reservoirLayers[k].SetCellMask(i, j, true);
                _reservoirLayers[k].DrillCell(i, j);
            }
        }

        public ReservoirLayer GetReservoirByClosestDepth(float depth)
        {
            int reservoirId = 0;
            //ReservoirLayer reservoir = _reservoirLayers[reservoirId];

            for (int i = 1; i < _reservoirLayers.Length; i++)
            {
                if (Mathf.Abs(depth - _reservoirDepthMarkerks[reservoirId].position.y) > Mathf.Abs(depth - _reservoirDepthMarkerks[i].position.y))
                { 
                    reservoirId = i;
                }
            }
            //Debug.Log(reservoirId);
            return _reservoirLayers[reservoirId];
        }

        internal int GetReservoirId(ReservoirLayer reservoir)
        {
            for (int i = 0; i < _reservoirLayers.Length; i++)
            {
                if (_reservoirLayers[i] == reservoir)
                { 
                    return i;
                }
            }
            return -1;
        }

        internal float GetReservoirDepthById(int id)
        {
            return _reservoirDepthMarkerks[id].position.y;
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
                _reservoirLayers[i].GenerateRandomReservoir(productiveFraction, reservoirHeight, LOW_PERM, HIGH_PERM, porosity, 1 - productiveFraction / 2, false);
                _contacts[i].localPosition = Vector3.up * _reservoirLayers[i].GetBottomDepth();
                _meshControllers[i].SetMesh(ReservoirLayerVisualizer.CreateReservoirMesh(_reservoirLayers[i]));
            }
        }

        public void RedrawMeshes()
        {
            for (int i = 0; i < _reservoirLayers.Length; i++)
            {
                //_meshControllers[i].SetMesh(ReservoirLayerVisualizer.CreateReservoirMesh(_reservoirLayers[i]));
                //ReservoirLayerVisualizer.CreateReservoirMesh(_reservoirLayers[i]);
                RedrawMeshAsync(_meshControllers[i], _reservoirLayers[i]);
            }
        }

        private async void RedrawMeshAsync(MeshController meshController, ReservoirLayer reservoirLayer)
        {
            Mesh mesh = await ReservoirLayerVisualizer.CreateReservoirMeshAsync(reservoirLayer);
            meshController.SetMesh(mesh);
        }
    }
}
