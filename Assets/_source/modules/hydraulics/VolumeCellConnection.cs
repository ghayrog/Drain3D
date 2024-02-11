using System;

namespace Hydraulics
{

    public sealed class VolumeCellConnection
    {
        private float _transmissibility;
        private VolumeCell _volumeCell1;
        private VolumeCell _volumeCell2;
        private float _pressureThreshold;

        public VolumeCellConnection(VolumeCell cell1, VolumeCell cell2, float transmissibility, float pressureThreshold)
        {
            if (transmissibility < 0 || pressureThreshold < 0)
            {
                throw new ArgumentException("Creation of Volume Cell Connection with negative transmissibility or pressure threshold is not allowed");
            }

            _transmissibility = transmissibility;
            _pressureThreshold = pressureThreshold;
            _volumeCell1 = cell1;
            _volumeCell2 = cell2;
        }

        public void PerformTimestep(float timestep)
        {
            float pressureGradient = _volumeCell2.CurrentPressure - _volumeCell1.CurrentPressure;
            if (Math.Abs(pressureGradient) < _pressureThreshold)
            {
                return;
            }

            float deltaVolume = _transmissibility * pressureGradient * timestep;

            if (deltaVolume > 0)
            {
                float actualVolume = _volumeCell1.ProduceFromCell(deltaVolume);
                _volumeCell2.InjectIntoCell(actualVolume);
            }
            else
            {
                float actualVolume = _volumeCell2.ProduceFromCell(-deltaVolume);
                _volumeCell1.InjectIntoCell(actualVolume);
            }
        }
    }
}