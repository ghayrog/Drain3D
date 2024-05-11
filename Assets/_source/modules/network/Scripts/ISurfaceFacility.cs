namespace Network
{
    public interface ISurfaceFacility
    {
        FacilityType FacilityType { get; }
        int CellI { get; }
        int CellJ { get; }

        void ConstructFacility(int cellI, int cellJ);
    }
}
