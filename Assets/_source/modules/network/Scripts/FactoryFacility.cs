namespace Network
{
    internal sealed class FactoryFacility :
        ISurfaceFacility
    {
        public FacilityType FacilityType => FacilityType.Factory;
        public int CellI { get; private set; }
        public int CellJ { get; private set; }

        public void ConstructFacility(int cellI, int cellJ)
        {
            CellI = cellI;
            CellJ = cellJ;
        }
    }

}
