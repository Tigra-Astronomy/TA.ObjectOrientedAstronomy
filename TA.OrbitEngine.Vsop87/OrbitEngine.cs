using System;
using System.Runtime.Remoting.Channels;

namespace TA.OrbitEngine.Vsop87
    {
    public class OrbitEngine
        {
        public double ComputeL0(double julianDate)
            {
            //return 1.75192386367183;
            // Iteratively apply the formula Tn = AT^alpha Cos(B + CT)
            // Sum all Tn and return the sum.
            var alpha = 0;  // Power of the term being computed
            var thousandsOfJulianDays = (julianDate - 2451545.0) / 365250.0; // Thousands of Julian Days since JD2000.0
            var tjdPowerAlpha = Math.Pow(thousandsOfJulianDays, alpha);
            double sum = 0.0;
            foreach (var term in Vsop87Data.Vsop87B_Earth_L0)
                {
                sum += term.AmplitudeA*tjdPowerAlpha*Math.Cos(term.PhaseB + term.FrequencyC*thousandsOfJulianDays);
                }
            return sum;
            }
        }
    }