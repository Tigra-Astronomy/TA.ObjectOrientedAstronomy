// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: Vsop87OrbitEngine.cs  Last modified: 2016-10-09@03:33 by Tim Long

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using TA.ObjectOrientedAstronomy.FundamentalTypes;

namespace TA.ObjectOrientedAstronomy.OrbitEngines.VSOP87
    {
    public sealed class Vsop87OrbitEngine
        {
        private static double ComputeVsop87Term(double julianDate, double alpha, IEnumerable<Vsop87Term> series)
            {
            Contract.Requires(series != null);
            // Iteratively apply the formula Tn = AT^alpha Cos(B + CT)
            // Sum all Tn and return the sum.
            var thousandsOfJulianDays = (julianDate - 2451545.0) / 365250.0; // Thousands of Julian Days since JD2000.0
            var tjdPowerAlpha = Math.Pow(thousandsOfJulianDays, alpha);
            return
                series.Sum(
                    term =>
                        term.AmplitudeA * tjdPowerAlpha *
                        Math.Cos(term.PhaseB + term.FrequencyC * thousandsOfJulianDays));
            }

        public static double ComputeVsop87Series(double targetDate, IEnumerable<IEnumerable<Vsop87Term>> seriesData)
            {
            Contract.Requires(seriesData != null);
            var alpha = 0; // series power
            var sum = 0.0;
            foreach (var term in seriesData)
                {
                sum += ComputeVsop87Term(targetDate, alpha, term);
                ++alpha;
                }
            return sum;
            }

        /// <summary>
        ///     Computes the spherical coordinates for a target body at a given date and with the specified reference
        ///     frame.
        /// </summary>
        /// <param name="targetDate">The target date.</param>
        /// <param name="solarSystemBody">The solar system body.</param>
        /// <param name="referenceFrame">The reference frame.</param>
        /// <returns>SphericalCoordinates.</returns>
        public static SphericalCoordinates ComputeSphericalCoordinates(double targetDate,
            SolarSystemBody solarSystemBody,
            ReferenceFrame referenceFrame)
            {
            var file = Vsop87DataReader.SelectDataFile(solarSystemBody,
                CoordinateSystem.HeliocentricSphericalCoordinates,
                referenceFrame);
            var vsop87SolutionData = Vsop87DataReader.LoadVsop87DataFromFile(file);
            var latitude = ComputeVsop87Series(targetDate, vsop87SolutionData.CoordinateVariableSeriesData['L']);
            var longitude = ComputeVsop87Series(targetDate, vsop87SolutionData.CoordinateVariableSeriesData['B']);
            var radius = ComputeVsop87Series(targetDate, vsop87SolutionData.CoordinateVariableSeriesData['R']);
            return new SphericalCoordinates(latitude, longitude, radius);
            }

        /// <summary>
        ///     Computes the rectangular coordinates for a target body at a given date and with the specified reference frame.
        /// </summary>
        /// <param name="targetDate">The target date.</param>
        /// <param name="solarSystemBody">The solar system body.</param>
        /// <param name="referenceFrame">The reference frame.</param>
        /// <returns>RectangularCoordinates expressed in Astronomical Units (AU).</returns>
        public static RectangularCoordinates ComputeRectangularCoordinates(double targetDate,
            SolarSystemBody solarSystemBody,
            ReferenceFrame referenceFrame)
            {
            var file = Vsop87DataReader.SelectDataFile(solarSystemBody,
                CoordinateSystem.HeliocentricRectangularCoordinates,
                referenceFrame);
            var vsop87SolutionData = Vsop87DataReader.LoadVsop87DataFromFile(file);
            var x = ComputeVsop87Series(targetDate, vsop87SolutionData.CoordinateVariableSeriesData['X']);
            var y = ComputeVsop87Series(targetDate, vsop87SolutionData.CoordinateVariableSeriesData['Y']);
            var z = ComputeVsop87Series(targetDate, vsop87SolutionData.CoordinateVariableSeriesData['Z']);
            return new RectangularCoordinates(x, y, z);
            }
        }
    }