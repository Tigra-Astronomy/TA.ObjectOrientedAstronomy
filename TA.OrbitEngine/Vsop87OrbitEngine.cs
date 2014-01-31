// This file is part of the TA.Orbits project
// 
// Copyright © 2014 Tigra Astronomy, all rights reserved.
// 
// File: Vsop87OrbitEngine.cs  Last modified: 2014-01-29@05:35 by Tim Long

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TA.OrbitEngine.Vsop87
    {
    public class Vsop87OrbitEngine
        {
        const string Vsop87HeaderRegexPattern =
            @"^\s*VSOP87 VERSION (?<VsopVersion>\w+)\s+(?<Body>\w+)\s+VARIABLE (?<Variable>\d+)\s+\((?<Variables>\w+)\)\s+\*T\*\*(?<Power>\d+)\s+(?<Terms>\d+)\s+TERMS\s+(?<Description>.*)";
        const string Vsop87DataRowRegexPattern = @"^(\s+[0-9.]+){16}\s+(?<A>[0-9.]+)\s+(?<B>[0-9.]+)\s+(?<C>[0-9.]+).*$";
        internal static Regex Vsop87HeaderRegex = new Regex(Vsop87HeaderRegexPattern, RegexOptions.Compiled);
        internal static Regex Vsop87DataRowRegex = new Regex(Vsop87DataRowRegexPattern, RegexOptions.Compiled);
        public static double ComputeVsop87Term(double julianDate, double alpha, IEnumerable<Vsop87Term> series)
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

        public static double ComputeVsop87Series(double targetDate, IEnumerable<IEnumerable<Vsop87Term>> seriesData)
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

        public static object LoadVsop87DataFromFile(string filename)
            {

            }
        }
    }
