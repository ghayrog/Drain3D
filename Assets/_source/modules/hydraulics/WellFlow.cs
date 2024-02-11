using System;

namespace Hydraulics
{
    public static class WellFlow
    {
        private const float THPK = 0.04f;
        private const float THPB = 1.2f;
        private const float WGRK = 0.0025f;
        private const float QK = 0.025f;
        private const float DK = 0.0275f;
        private const float DB = -1.8871f;
        private const float MD = 1200f;
        private const float TVD = 700;

        //                                                  bar       g/m3  1000m3/day                m         m          m
        public static float CalculatePressureGradient(float thp, float wgr, float qgas, float innerDiam, float md, float tvd)
        {
            if (thp < 0 || wgr < 0 || qgas < 0 || innerDiam < 0)
            {
                throw new ArgumentException("Well flow calculation cannot be performed with negative parameters");
            }

            float dP = (THPK * thp + THPB) * (1 + WGRK * wgr) * tvd / TVD + QK * qgas * (DK / innerDiam / innerDiam + DB) * md / MD;
            return dP;
        }

        public static float CalculatePressureQDerivative(float innerDiam, float md)
        {
            return QK * (DK / innerDiam / innerDiam + DB) * md / MD;
        }

        public static float CalculateRate(float thp, float bhp, float wgr, float innerDiam, float md, float tvd)

        {
            if (thp < 0 || wgr < 0 || bhp < 0 || innerDiam < 0)
            {
                throw new ArgumentException("Well flow calculation cannot be performed with negative parameters");
            }

            float q = ((bhp - thp) - (THPK * thp + THPB) * (1 + WGRK * wgr) * tvd / TVD) / (QK * (DK / innerDiam / innerDiam + DB) * md / MD);
            return q;

        }

        public static float CalculateTubingPressure(float bhp, float wgr, float qgas, float innerDiam, float md, float tvd)
        {
            if (bhp < 0 || wgr < 0 || qgas < 0 || innerDiam < 0)
            {
                throw new ArgumentException("Well flow calculation cannot be performed with negative parameters");
            }
            float thp = (bhp - QK * qgas * (DK / innerDiam / innerDiam + DB) * md / MD - THPB * (1 + WGRK * wgr) * tvd / TVD) / (1 + THPK * (1 + WGRK * wgr) * tvd / TVD);
            return thp;
        }
    }
}