namespace Network
{
    internal sealed class ExplorationWellFacility :
        ISurfaceFacility
    {
        public FacilityType FacilityType => FacilityType.ExplorationWell;
        public int CellI { get; private set; }
        public int CellJ { get; private set; }

        public void ConstructFacility(int cellI, int cellJ)
        {
            CellI = cellI;
            CellJ = cellJ;
        }
    }
}
