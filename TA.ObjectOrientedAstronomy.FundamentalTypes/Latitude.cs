// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright � 2015 Tigra Astronomy, all rights reserved.
// 
// File: Latitude.cs  Last modified: 2015-11-21@16:44 by Tim Long

using System;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Geographical lattitude, in degrees from the equator, in the range +90 to -90. Northern lattitudes are
    ///     positive, southern lattitudes are negative. Stored internally as a double, in the range +90 to -90.
    /// </summary>
    public class Latitude : Bearing
    {
        /// <summary>
        ///     Construct a lattitude from decimal degrees.
        /// </summary>
        /// <param name="degrees">Angle used to initialise the lattitude.</param>
        public Latitude(double degrees)
        {
            Value = Normalize(degrees);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Latitude" /> class.
        /// </summary>
        /// <param name="degrees">
        ///     The degrees part of the angle, signed integer from -180 to +180. Values outside that range will
        ///     be 'normalized' to that range.
        /// </param>
        /// <param name="minutes">The minutes part of tha angle, positive integer 0 to 59.</param>
        /// <param name="seconds">The seconds part of the angle, positive integer from 0 to 59.</param>
        public Latitude(int degrees, uint minutes, uint seconds)
        {
            Value = Normalize(FromDms(degrees, minutes, seconds));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Latitude" /> class.
        /// </summary>
        protected Latitude() {}

        /// <summary>
        ///     True if the lattitude is in the Northern hemisphere.
        ///     Latitudes exactly on the equator are considered to be in the Northern hemisphere.
        /// </summary>
        public bool IsNorth { get { return Value >= 0.0; } }

        /// <summary>
        ///     True if the lattitude is in the Southern hemisphere
        /// </summary>
        public bool IsSouth { get { return Value < 0.0; } }

        /// <summary>
        ///     Gets the maximum allowed value.
        /// </summary>
        /// <value>The maximum allowed value.</value>
        public override double MaxValue { get { return 90.0; } }

        /// <summary>
        ///     Gets the minimum allowed value.
        /// </summary>
        /// <value>The minimum allowed value.</value>
        public override double MinValue { get { return -90.0; } }

        /// <summary>
        ///     Ensures that the supplied argument is in the correct range for a geographic lattitude.
        /// </summary>
        /// <param name="degrees">Angular distance from the equator.</param>
        /// <returns>
        ///     An orthoganal value in the correct range for geographic latitudes
        ///     (North positive, range +/- 90 degrees).
        /// </returns>
        public override double Normalize(double degrees)
        {
            degrees %= 360; // First normalize to a full circle, then deal with quadrants.
            if (degrees < 0.0)
                degrees += 360.0; // Move negative angles into positive.
            // We could now be in any of the 4 quadrants.
            if (degrees <= 90.0)
                return degrees; // first quadrant, return 'as is'
            if (degrees <= 270.0 /* and >90 */)
                return 180.0 - degrees; // quadrants 2 and 3.
            return degrees - 360.0; // quadrant 4
        }

        /// <summary>
        ///     Ensures that the supplied argument is in the correct range for a geographic lattitude.
        /// </summary>
        /// <param name="nDegrees">Angular distance from the equator in whole degrees.</param>
        /// <returns>
        ///     The geographic latitude corresponding to the supplied angle, North positive, range +/- 90 degrees.
        /// </returns>
        public override int Normalize(int nDegrees)
        {
            var dAngle = Normalize((double) nDegrees);

            // Truncate towards zero ( 1.5 -> 1.0; -1.5 -> -1.0)
            if (dAngle >= 0) // Positive angles
                return (int) Math.Floor(dAngle);
            return (int) Math.Ceiling(dAngle);
        }

        /// <summary>
        ///     Output the lattitude as a formatted string h dd�mm'ss"
        ///     (h is the hemisphere, N or S, dd is positive degrees).
        /// </summary>
        /// <returns>string object containing formatted result</returns>
        public override string ToString()
        {
            var strLat = string.Format("{0} {1:D2}°{2:D2}'{3:D2}\"", IsNorth ? 'N' : 'S', Degrees, Minutes, Seconds);
            return strLat;
        }
    }
}