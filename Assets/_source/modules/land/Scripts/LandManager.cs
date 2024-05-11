using DI;
using Game;
using System;
using UnityEngine;
using Network;

namespace Land
{
    [Serializable]
    internal sealed class LandManager :
        IGameInitializeListener, IGameStartListener, IGameUpdateListener
    {
        [SerializeField]
        private GameObject[] _landTilePrefabs;

        [SerializeField]
        private Transform _tilesContainer;

        internal LandTile[,] LandTiles { get; private set; }

        public LoadingPriority Priority => LoadingPriority.Low;

        public bool IsInitialized => _isInitialized;
        private bool _isInitialized = false;

        private Land _land = new Land();
        private FacilitySystem _facilitySystem;

        [Inject]
        public void Construct(FacilitySystem facilitySystem)
        {
            _facilitySystem = facilitySystem;
        }

        private int GetRiverTileId(int i, int j)
        {
            bool topRiver = false;
            bool bottomRiver = false;
            bool leftRiver = false;
            bool rightRiver = false;

            int trueValues = 0;

            if (i == 0 || _land.LandGrid[i - 1, j] == LandType.River)
            { 
                leftRiver = true;
                trueValues++;
            }
            if (i == Land.GRID_SIZE - 1|| _land.LandGrid[i + 1, j] == LandType.River)
            {
                rightRiver = true;
                trueValues++;
            }
            if (j == 0 || _land.LandGrid[i, j - 1] == LandType.River)
            {
                bottomRiver = true;
                trueValues++;
            }
            if (j == Land.GRID_SIZE - 1 || _land.LandGrid[i, j + 1] == LandType.River)
            {
                topRiver = true;
                trueValues++;
            }

            if (trueValues >=3)
            {
                return 5;
            }
            if ((topRiver && leftRiver) || (leftRiver && bottomRiver) || (bottomRiver && rightRiver) || (rightRiver && topRiver))
            {
                return 4;
            }

            return 3;
        }

        private Quaternion GetRiverTileRotation(int i, int j)
        {
            bool topRiver = false;
            bool bottomRiver = false;
            bool leftRiver = false;
            bool rightRiver = false;

            int trueValues = 0;

            if (i == 0 || _land.LandGrid[i - 1, j] == LandType.River)
            {
                leftRiver = true;
                trueValues++;
            }
            if (i == Land.GRID_SIZE - 1 || _land.LandGrid[i + 1, j] == LandType.River)
            {
                rightRiver = true;
                trueValues++;
            }
            if (j == 0 || _land.LandGrid[i, j - 1] == LandType.River)
            {
                bottomRiver = true;
                trueValues++;
            }
            if (j == Land.GRID_SIZE - 1 || _land.LandGrid[i, j + 1] == LandType.River)
            {
                topRiver = true;
                trueValues++;
            }

            if (bottomRiver && leftRiver && rightRiver) return Quaternion.identity;
            if (bottomRiver && rightRiver && topRiver) return Quaternion.Euler(0, -90, 0);
            if (leftRiver && topRiver && rightRiver) return Quaternion.Euler(0, 180, 0);
            if (bottomRiver && leftRiver && topRiver) return Quaternion.Euler(0, 90, 0);

            if (bottomRiver && topRiver) return Quaternion.identity;
            if (leftRiver && rightRiver) return Quaternion.Euler(0, 90, 0);

            if (bottomRiver && rightRiver) return Quaternion.identity;
            if (rightRiver && topRiver) return Quaternion.Euler(0, -90, 0);
            if (leftRiver && topRiver) return Quaternion.Euler(0, 180, 0);
            if (leftRiver && bottomRiver) return Quaternion.Euler(0, 90, 0);

            return Quaternion.identity;
        }

        internal LandType GetLandType(int i, int j)
        {
            return _land.LandGrid[i, j];
        }

        private int GetTileId(int i, int j)
        {
            switch (_land.LandGrid[i, j])
            { 
                case LandType.Grass:
                    return 0;
                case LandType.Swamp:
                    return 1;
                case LandType.Forest:
                    return 2;
                case LandType.River:
                    return GetRiverTileId(i,j);
                default:
                    return -1;
            }
        }

        internal void RedrawLand()
        {
            for (int i = 0; i < Land.GRID_SIZE; i++)
            {
                for (int j = 0; j < Land.GRID_SIZE; j++)
                {
                    if (LandTiles[i, j] != null && LandTiles[i, j].LandType == LandType.River)
                    {
                        var gameObject = LandTiles[i, j].gameObject;
                        LandTiles[i, j] = null;
                        GameObject.Destroy(gameObject);
                        gameObject = null;
                    }
                }
            }
            for (int i = 0; i < Land.GRID_SIZE; i++)
            {
                for (int j = 0; j < Land.GRID_SIZE; j++)
                {
                    if (LandTiles[i, j] == null)
                    {
                        Quaternion _tileRotation = Quaternion.identity;
                        int _tilePrefabId = GetTileId(i, j);
                        if (_land.LandGrid[i, j] == LandType.River)
                        {
                            _tileRotation = GetRiverTileRotation(i, j);
                        }
                        var newLandTile = GameObject.Instantiate(
                            _landTilePrefabs[_tilePrefabId],
                            new Vector3(-Land.GRID_SIZE * Land.CELL_SIZE / 2 + (i + 0.5f) * Land.CELL_SIZE, 0, -Land.GRID_SIZE * Land.CELL_SIZE / 2 + (j + 0.5f) * Land.CELL_SIZE),
                            _tileRotation,
                            _tilesContainer);
                        LandTiles[i, j] = newLandTile.GetComponent<LandTile>();
                    }
                    else
                    if (LandTiles[i,j].LandType != _land.LandGrid[i,j])
                    {
                        GameObject.Destroy(LandTiles[i, j].gameObject);
                        Quaternion _tileRotation = Quaternion.identity;
                        int _tilePrefabId = GetTileId(i, j);
                        if (_land.LandGrid[i, j] == LandType.River)
                        {
                            _tileRotation = GetRiverTileRotation(i, j);
                        }
                        var newLandTile = GameObject.Instantiate(
                            _landTilePrefabs[_tilePrefabId],
                            new Vector3(-Land.GRID_SIZE * Land.CELL_SIZE / 2 + (i + 0.5f) * Land.CELL_SIZE, 0, -Land.GRID_SIZE * Land.CELL_SIZE / 2 + (j + 0.5f) * Land.CELL_SIZE),
                            _tileRotation,
                            _tilesContainer);
                        LandTiles[i, j] = newLandTile.GetComponent<LandTile>();
                    }
                }
            }
        }

        public void OnGameStart()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3[] interestPoints = {
                    new Vector3(Land.GRID_SIZE / 4 * Land.CELL_SIZE, 0, Land.GRID_SIZE / 4 * Land.CELL_SIZE),
                    new Vector3(Land.GRID_SIZE / 4 * Land.CELL_SIZE, 0, -Land.GRID_SIZE / 4 * Land.CELL_SIZE),
                    new Vector3(-Land.GRID_SIZE / 4 * Land.CELL_SIZE, 0, Land.GRID_SIZE / 4 * Land.CELL_SIZE),
                    new Vector3(-Land.GRID_SIZE / 4 * Land.CELL_SIZE, 0, -Land.GRID_SIZE / 4 * Land.CELL_SIZE)
                };
                _land.GenerateTerrain(interestPoints, true, 0.2f, 0.2f);
                RedrawLand();
            }
        }

        public void OnGameInitialize()
        {
            LandTiles = new LandTile[Land.GRID_SIZE, Land.GRID_SIZE];
            Vector3[] interestPoints = {
                new Vector3(Land.GRID_SIZE / 4 * Land.CELL_SIZE, 0, Land.GRID_SIZE / 4 * Land.CELL_SIZE),
                new Vector3(Land.GRID_SIZE / 4 * Land.CELL_SIZE, 0, -Land.GRID_SIZE / 4 * Land.CELL_SIZE),
                new Vector3(-Land.GRID_SIZE / 4 * Land.CELL_SIZE, 0, Land.GRID_SIZE / 4 * Land.CELL_SIZE),
                new Vector3(-Land.GRID_SIZE / 4 * Land.CELL_SIZE, 0, -Land.GRID_SIZE / 4 * Land.CELL_SIZE)
            };
            _land.GenerateTerrain(interestPoints, true, 0.2f, 0.2f);
            int factoryI = _land.FindFactorySpot();
            _facilitySystem.AddNewFacility(FacilityType.Factory, factoryI, 0);
            Debug.Log($"Add Factory [{factoryI}, {0}]");
            RedrawLand();
            _isInitialized = true;
        }
    }
}
