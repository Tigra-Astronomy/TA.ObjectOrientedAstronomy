// This file is part of the TA.Orbits project
// 
// Copyright © 2014 Tigra Astronomy, all rights reserved.
// 
// File: OrbitEngine.cs  Last modified: 2014-01-29@05:35 by Tim Long

using System;
using System.Collections.Generic;

namespace TA.OrbitEngine.Vsop87
    {
    public class OrbitEngine
        {
        public double ComputeVsop87Term(double julianDate, double alpha, IList<Vsop87Term> series)
            {
            // Iteratively apply the formula Tn = AT^alpha Cos(B + CT)
            // Sum all Tn and return the sum.
            var thousandsOfJulianDays = (julianDate - 2451545.0)/365250.0; // Thousands of Julian Days since JD2000.0
            var tjdPowerAlpha = Math.Pow(thousandsOfJulianDays, alpha);
            var sum = 0.0;
            foreach (var term in series)
                sum += term.AmplitudeA*tjdPowerAlpha*Math.Cos(term.PhaseB + term.FrequencyC*thousandsOfJulianDays);
            return sum;
            }

        public double ComputeVsop87Series(double targetDate, List<List<Vsop87Term>> seriesData)
            {
            var alpha = 0;  // series power
            var sum = 0.0;
            foreach (var term in seriesData)
                {
                sum += ComputeVsop87Term(targetDate, alpha, term);
                ++alpha;
                }
            return sum;
            }

        public object LoadVsop87DataFromFile(string filename)
            {
            throw new NotImplementedException();
            }
        }
    }
