using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public sealed class FacilitySystem : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _facilityPrefabs;

        private List<ISurfaceFacility> _facilities = new List<ISurfaceFacility>();
        private List<Transform> _facilityObjects = new List<Transform>();

        private bool isFactoryExisting = false;

        public bool CheckIfCellAvailable(int cellI, int cellJ)
        {
            foreach (ISurfaceFacility facility in _facilities)
            {
                if (facility.CellI == cellI && facility.CellJ == cellJ)
                {
                    return false;
                }
            }
            return true;
        }

        public bool RemoveFacilityAtCell(int cellI, int cellJ)
        {
            foreach (ISurfaceFacility facility in _facilities)
            {
                if (facility.CellI == cellI && facility.CellJ == cellJ)
                {
                    _facilities.Remove(facility);
                    RedrawFacilities();
                    return true;
                }
            }
            return false;
        }

        public ISurfaceFacility GetFacilityAtCell(int cellI, int cellJ)
        {
            foreach (ISurfaceFacility facility in _facilities)
            {
                if (facility.CellI == cellI && facility.CellJ == cellJ)
                {
                    return facility;
                }
            }
            return null;
        }

        public void AddNewFacility(FacilityType facilityType, int cellI, int cellJ)
        {
            switch (facilityType)
            {
                case FacilityType.ExplorationWell:
                    var facility = new ExplorationWellFacility();
                    facility.ConstructFacility(cellI, cellJ);
                    _facilities.Add(facility);
                    RedrawFacilities();
                    break;
                case FacilityType.Factory:
                    if (!isFactoryExisting)
                    {
                        var factory = new FactoryFacility();
                        factory.ConstructFacility(cellI, cellJ);
                        _facilities.Add(factory);
                        RedrawFacilities();
                    }
                    break;
                case FacilityType.Placeholder:
                    var placeholder = new PlaceholderFacility();
                    placeholder.ConstructFacility(cellI, cellJ);
                    _facilities.Add(placeholder);
                    RedrawFacilities();
                    break;
                default:
                    break;
            }
        }

        public void RedrawFacilities()
        { 
            while (_facilityObjects.Count > 0)
            {
                var go = _facilityObjects[0].gameObject;
                _facilityObjects.RemoveAt(0);
                Destroy(go);
                go = null;
            }

            for (int i = 0; i < _facilities.Count; i++)
            {
                switch (_facilities[i].FacilityType)
                {
                    case FacilityType.ExplorationWell:
                        var newLandTile = GameObject.Instantiate(
                            _facilityPrefabs[GetPrefabId(FacilityType.ExplorationWell)],
                            new Vector3(NetworkView.CellIndexToCoord(_facilities[i].CellI), 0, NetworkView.CellIndexToCoord(_facilities[i].CellJ)),
                            Quaternion.identity,
                            transform);
                        _facilityObjects.Add(newLandTile.transform);
                        break;
                    case FacilityType.Factory:
                        var newFactory = GameObject.Instantiate(
                            _facilityPrefabs[GetPrefabId(FacilityType.Factory)],
                            new Vector3(NetworkView.CellIndexToCoord(_facilities[i].CellI), 0, NetworkView.CellIndexToCoord(_facilities[i].CellJ)),
                            Quaternion.identity,
                            transform);
                        _facilityObjects.Add(newFactory.transform);
                        break;
                    case FacilityType.Placeholder:
                        Debug.Log("Instantiating placeholder");
                        var newPlaceholder = GameObject.Instantiate(
                            _facilityPrefabs[GetPrefabId(FacilityType.Placeholder)],
                            new Vector3(NetworkView.CellIndexToCoord(_facilities[i].CellI), 0, NetworkView.CellIndexToCoord(_facilities[i].CellJ)),
                            Quaternion.identity,
                            transform);
                        _facilityObjects.Add(newPlaceholder.transform);
                        break;
                    default:
                        break;
                }
            }
        }

        public int GetPrefabId(FacilityType facilityType)
        {
            if (facilityType == FacilityType.ExplorationWell) return 0;
            if (facilityType == FacilityType.Factory) return 1;
            if (facilityType == FacilityType.Placeholder) return 5;
            return 0;
        }
    }
}
