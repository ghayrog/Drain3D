using System;
using UnityEngine.Rendering;

namespace Hydraulics
{
    public class Well
    {
        private const float PRESSURE_ERROR = 0.1f;
        private LayerWellConnection _layer;
        private float _measuredDepth;
        private float _verticalDepth;
        private float _innerDiameter;

        public Well(float md, float tvd, float innerDiameter, VolumeCell volumeCell, float layerProductivity)
        {
            _layer = new LayerWellConnection(volumeCell, layerProductivity);
            _measuredDepth = md;
            _verticalDepth = tvd;
            _innerDiameter = innerDiameter;
        }

        public float EstimateRate(float thp)
        {
            float rate = 0;

            float max_layer_pressure = _layer.EstimatePressure(0);
            float min_well_pressure = WellFlow.CalculatePressureGradient(thp, _layer.WGR, rate, _innerDiameter, _measuredDepth, _verticalDepth);

            float bhp_layer = _layer.EstimatePressure(rate);
            float bhp_well = thp + WellFlow.CalculatePressureGradient(thp, _layer.WGR, rate, _innerDiameter, _measuredDepth, _verticalDepth);
            float targetFunc = bhp_layer - bhp_well;
            if (targetFunc < 0)
            {
                return -1;
            }
            float targetFuncDerivative = _layer.EstimatePressureQDerivative(rate) - WellFlow.CalculatePressureQDerivative(_innerDiameter, _measuredDepth);
            while ((float)Math.Abs(targetFunc) > PRESSURE_ERROR)
            {
                rate += -targetFunc / targetFuncDerivative;
                bhp_layer = _layer.EstimatePressure(rate);
                bhp_well = thp + WellFlow.CalculatePressureGradient(thp, _layer.WGR, rate, _innerDiameter, _measuredDepth, _verticalDepth);
                targetFunc = bhp_layer - bhp_well;
                targetFuncDerivative = _layer.EstimatePressureQDerivative(rate) - WellFlow.CalculatePressureQDerivative(_innerDiameter, _measuredDepth);
            }
            return rate;
        }

        public WellMode GetWellMode(float thp)
        {
            float rate = EstimateRate(thp);
            var wellMode = new WellMode();

            wellMode.Rate = EstimateRate(thp);
            wellMode.TubingPressure = thp;
            wellMode.BottomPressure = _layer.EstimatePressure(rate);
            wellMode.ReservoirPressure = _layer.ReservoirPressure;
            wellMode.WGR = _layer.WGR;

            return wellMode;
        }

        public struct WellMode
        {
            public float TubingPressure;
            public float Rate;
            public float BottomPressure;
            public float ReservoirPressure;
            public float WGR;
        }
    }
}