namespace Network
{
    public sealed class NetworkView
    {
        internal const float CELL_SIZE = 2.5f;
        internal const int GRID_SIZE = 24;

        public static float CellIndexToCoord(int index)
        {
            return ((float)index + 0.5f) * CELL_SIZE - GRID_SIZE / 2 * CELL_SIZE;
        }
    }
}
