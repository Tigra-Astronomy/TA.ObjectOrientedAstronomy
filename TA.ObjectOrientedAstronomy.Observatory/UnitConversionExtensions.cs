using System;
using System.Collections.Generic;
using System.Text;
using TA.ObjectOrientedAstronomy.FundamentalTypes;

namespace TA.ObjectOrientedAstronomy.Observatory
    {
    internal static class UnitConversionExtensions
        {
        public static double DegreesToRadians(double degrees) => Math.PI * degrees / 180.0;

        public static double RadiansToDegrees(double radians) => radians * 180.0 / Math.PI;

        public static double InDegrees(this HourAngle ha)
            {
            return ha.Value * 15.0;
            }

        public static double InRadians(this HourAngle ha)
            {
            var degrees = ha.InDegrees();
            return degrees / 360.0 * (Math.PI * 2.0);
            }

        public static double InRadians(this Declination dec)
            {
            return dec.Value / 360.0 * (Math.PI * 2.0);
            }

        public static double InRadians(this Latitude lat)
            {
            return lat.Value / 360.0 * (Math.PI * 2.0);
            }

        public static double AngularDistanceTo(this TelescopeMechanicalPosition from, TelescopeMechanicalPosition to)
            {
            var haDistance = to.HourAngle.InDegrees() - from.HourAngle.InDegrees();
            var decDistance = to.Declination.Value - from.Declination.Value;
            var vectorSum = HypotenuseDistance(haDistance, decDistance);
            return vectorSum;
            }

        private static double HypotenuseDistance(double opposite, double adjacent)
            {
            return Math.Sqrt(opposite * opposite + adjacent * adjacent);
            }
        }
    }