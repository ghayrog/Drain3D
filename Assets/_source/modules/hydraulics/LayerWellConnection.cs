using System;

namespace Hydraulics
{
    public class LayerWellConnection
    {
        public float WGR => _volumeCell.WGR;
        public float ReservoirPressure => _volumeCell.CurrentPressure;

        private VolumeCell _volumeCell;
        private float _productivity;

        public LayerWellConnection(VolumeCell volumeCell, float productivity)
        {
            _volumeCell = volumeCell;
            _productivity = productivity;
        }

        public float EstimateRate(float bhp)
        {
            if (bhp < 0)
            {
                throw new ArgumentException("Layer flow calculation cannot be performed with negative bhp");
            }

            float reservoirPressure = _volumeCell.CurrentPressure;
            if (bhp > reservoirPressure)
            {
                return 0;
            }

            float rate = _productivity * (reservoirPressure * reservoirPressure - bhp * bhp);
            return rate;
        }

        public float EstimatePressureQDerivative(float rate)
        {
            float reservoirPressure = _volumeCell.CurrentPressure;
            float bhp_square = reservoirPressure * reservoirPressure - rate / _productivity;
            return -0.5f / _productivity / (float)Math.Sqrt(bhp_square);
        }

        public float EstimatePressure(float rate)
        {
            if (rate < 0)
            {
                throw new ArgumentException("Layer flow calculation cannot be performed with negative rate");
            }

            float reservoirPressure = _volumeCell.CurrentPressure;
            float bhp_square = reservoirPressure * reservoirPressure - rate / _productivity;
            return (float)Math.Sqrt(bhp_square);
        }

        public float ProduceFromLayer(float bhp, float timestep)
        {
            float rate = EstimateRate(bhp);
            return _volumeCell.ProduceFromCell(rate * timestep);
        }
    }
}