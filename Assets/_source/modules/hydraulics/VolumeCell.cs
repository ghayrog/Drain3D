using System;

namespace Hydraulics
{
    public sealed class VolumeCell
    {
        public bool IsDepleted => (_currentVolume == _abandonmentVolume);
        public float CurrentPressure => _currentPressure;
        public float CurrentVolume => _currentVolume;
        public float WGR => GetCellWGR();

        private const float ABANDONMENT_PRESSURE_FRACTION = 0.05f;
        private const float MAX_WGR = 1000;
        private float _initialVolume;
        private float _initialPressure;
        private float _initialWGR;
        private float _waterInflowRatio;

        private float _currentVolume;
        private float _currentPressure;

        private float _abandonmentPressure;
        private float _abandonmentVolume;

        public VolumeCell(float initialVolume, float initialPressure, float initialWGR, float waterInflowRatio)
        {
            if (initialVolume <= 0 || initialPressure <= 0)
            {

                throw new ArgumentException("Creation of Volume Cell with zero or negative volume or pressure is not allowed");
            }
            _initialVolume = initialVolume;
            _initialPressure = initialPressure;
            _initialWGR = Math.Max(0, initialWGR);
            _waterInflowRatio = Math.Max(0, waterInflowRatio);
            _currentVolume = initialVolume;
            _currentPressure = initialPressure;

            _abandonmentPressure = ABANDONMENT_PRESSURE_FRACTION * initialPressure;
            _abandonmentVolume = PressureToVolume(_abandonmentPressure);
        }

        public float ProduceFromCell(float producedVolume)
        {
            float availableProduction = _currentVolume - _abandonmentVolume;
            if (availableProduction == 0 || producedVolume < 0)
            {
                return 0;
            }
            if (producedVolume > availableProduction)
            {
                _currentVolume = _abandonmentVolume;
                _currentPressure = _abandonmentPressure;
                return availableProduction;
            }
            else
            {
                _currentVolume -= producedVolume;
                _currentPressure = VolumeToPressure(_currentVolume);
                return producedVolume;
            }
        }

        public float InjectIntoCell(float injectedVolume)
        {
            if (injectedVolume < 0)
            {
                return 0;
            }

            _currentVolume += injectedVolume;
            _currentPressure = VolumeToPressure(_currentVolume);
            return injectedVolume;
        }

        private float VolumeToPressure(float volume)
        {
            return volume / _initialVolume * _initialPressure;
        }

        private float PressureToVolume(float pressure)
        {
            return pressure / _initialPressure * _initialVolume;
        }

        private float GetCellWGR()
        {
            return (MAX_WGR * _waterInflowRatio + (float)Math.Sqrt(_currentVolume / _initialVolume) * (_initialWGR - MAX_WGR * _waterInflowRatio));
        }
    }
}