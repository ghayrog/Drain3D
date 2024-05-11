using System;
using System.Collections;
using UnityEngine;
using DI;
using Reservoir;
using Network;
using GameTimeScale;
using Wallet;
using log4net.Util;

namespace Land
{
    [Serializable]
    public sealed class LandExplorer
    {
        [SerializeField]
        private int _explorationRadius = 2;

        [SerializeField]
        private int _seismicDurationDays = 500;

        [SerializeField]
        private int _drillingDurationDays = 300;

        [SerializeField]
        private int _explorationWellCost = 1500000;

        [SerializeField]
        private int _seismicSurveyCost = 4000000;

        [SerializeField]
        private GameObject _seismicPlaceholderPrefab;

        private ReservoirManager _reservoirManager;
        private ReservoirClicker _reservoirClicker;
        private LandManager _landManager;
        private FacilitySystem _facilitySystem;
        private GameTime _gameTime;
        private WalletManager _walletManager;

        [Inject]
        private void Construct(ReservoirManager reservoirManager, LandManager landManager, ReservoirClicker reservoirClicker, FacilitySystem facilitySystem, GameTime gameTime, WalletManager walletManager)
        {
            _reservoirManager = reservoirManager;
            _landManager = landManager;
            _reservoirClicker = reservoirClicker;
            _facilitySystem = facilitySystem;
            _gameTime = gameTime;
            _walletManager = walletManager;
        }

        public void Explore()
        {
            var i = _reservoirClicker.CurrentI;
            var j = _reservoirClicker.CurrentJ;
            if (!_walletManager.RequestAmount(_seismicSurveyCost)) return;

            var placeholderObject = GameObject.Instantiate(
                            _seismicPlaceholderPrefab,
                            new Vector3(Network.NetworkView.CellIndexToCoord(i), 0, Network.NetworkView.CellIndexToCoord(j)),
                            Quaternion.identity,
                            _facilitySystem.transform);

            _gameTime.AddGameTimer(_seismicDurationDays, () =>
            {
                GameObject.Destroy(placeholderObject);
                ExploreCoroutine(i, j);
            }
            );            
        }

        public void Drill()
        {
            var i = _reservoirClicker.CurrentI;
            var j = _reservoirClicker.CurrentJ;
            if (_landManager.GetLandType(i, j) == LandType.Grass && _facilitySystem.CheckIfCellAvailable(i, j))
            {
                if (!_walletManager.RequestAmount(_explorationWellCost)) return;
                Debug.Log("Started drilling exploration well");
                _facilitySystem.AddNewFacility(FacilityType.Placeholder, i, j);
                _gameTime.AddGameTimer(_drillingDurationDays, () =>
                {
                    Debug.Log("Finished drillring ecploration well");
                    DrillCoroutine(i, j);

                }
                );
            }
        }

        private void ExploreCoroutine(int iCenter, int jCenter)
        {
            Debug.Log($"Starting exploring region: [{iCenter}, {jCenter}]");
            int minI = Mathf.Max(iCenter - _explorationRadius, 0);
            int maxI = Mathf.Min(iCenter + _explorationRadius, Land.GRID_SIZE - 1);
            int minJ = Mathf.Max(jCenter - _explorationRadius, 0);
            int maxJ = Mathf.Min(jCenter + _explorationRadius, Land.GRID_SIZE - 1);
            for (int i = minI; i <= maxI; i++)
            {
                for (int j = minJ; j <= maxJ; j++)
                {
                    if (_landManager.GetLandType(i, j) == LandType.Grass || _landManager.GetLandType(i, j) == LandType.Forest)
                    {
                        _reservoirManager.ExploreLayers(i, j);
                    }
                }
            }
            _reservoirManager.RedrawMeshes();
        }

        private void DrillCoroutine(int i, int j)
        {
            if (_landManager.GetLandType(i, j) == LandType.Grass && _facilitySystem.GetFacilityAtCell(i, j)?.FacilityType == FacilityType.Placeholder)
            {

                _reservoirManager.DrillLayers(i, j);
                _facilitySystem.RemoveFacilityAtCell(i, j);
                _facilitySystem.AddNewFacility(FacilityType.ExplorationWell, i, j);
                _reservoirManager.RedrawMeshes();
            }
        }
    }
}
