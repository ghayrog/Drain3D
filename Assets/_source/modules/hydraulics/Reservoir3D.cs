using System;
using System.Collections.Generic;

namespace Hydraulics
{
    public sealed class Reservoir3D
    {
        private const int X_DIMENSION = 10;
        private const int Y_DIMENSION = 10;
        private const int Z_DIMENSION = 10;
        private int[,,] _zoneIndex = new int[X_DIMENSION-1, Y_DIMENSION-1, Z_DIMENSION-1]; //Zone 0 is non-reservoir
        private float[,,] _permeability = new float[X_DIMENSION-1,Y_DIMENSION-1,Z_DIMENSION-1];
        private float[,,] _thickness = new float[X_DIMENSION-1, Y_DIMENSION-1, Z_DIMENSION-1];
        private VolumeCell[,,] _volumeCells = new VolumeCell[X_DIMENSION-1, Y_DIMENSION-1, Z_DIMENSION-1];
        private List<VolumeCellConnection> _cellConnections = new List<VolumeCellConnection>();

        private Reservoir3D()
        {
            for (int i = 0; i < X_DIMENSION; i++)
            {
                for (int j = 0; i < Y_DIMENSION; j++)
                {
                    for (int k = 0; k < Z_DIMENSION; k++)
                    {
                        _zoneIndex[i, j, k] = 0;
                        _permeability[i, j, k] = 0;
                        _thickness[i, j, k] = 0;
                        _volumeCells[i, j, k] = null;
                    }
                }
            }
        }

        public static Reservoir3D GenerateReservoir(int zonesCount, float maxPermeability, float maxThickness, float totalVolume)
        {
            Reservoir3D reservoir= new Reservoir3D();

            List<int> reservoirZones = new List<int>();
            for (int i = 0; i < Z_DIMENSION; i++)
            { 
                reservoirZones.Add(i);
            }
            var rand = new Random();
            for (int i = 0; i < Z_DIMENSION-zonesCount; i++)
            { 

                reservoirZones.RemoveAt(rand.Next(reservoirZones.Count));
            }

            return reservoir;
        }
    }
}