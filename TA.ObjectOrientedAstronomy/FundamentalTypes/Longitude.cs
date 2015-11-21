// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: Longitude.cs  Last modified: 2015-11-21@16:44 by Tim Long

using System;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Geographical longitude represented in degrees, minutes and seconds of arc, in the range -179°59'59" to
    ///     +180°00'00". West is negative, East is positive.
    /// </summary>
    public class Longitude : Bearing
    {
        /// <summary>
        ///     Construct a longitude from a decimal angle.
        /// </summary>
        /// <param name="dAngle">Arbitrary decimal angle in degrees</param>
        public Longitude(double dAngle)
        {
            Value = Normalize(dAngle);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Longitude" /> class.
        /// </summary>
        /// <param name="degrees">
        ///     The degrees part of the angle, signed integer from -180 to +180. Values outside that range will
        ///     be 'normalized' to that range.
        /// </param>
        /// <param name="minutes">The minutes part of tha angle, positive integer 0 to 59.</param>
        /// <param name="seconds">The seconds part of the angle, positive integer from 0 to 59.</param>
        public Longitude(int degrees, uint minutes, uint seconds)
        {
            Value = Normalize(FromDms(degrees, minutes, seconds));
        }

        /// <summary>
        ///     True if the lattitude is in the Northern hemisphere
        /// </summary>
        public bool IsEast { get { return Value >= 0.0; } }

        /// <summary>
        ///     True if the lattitude is in the Southern hemisphere
        /// </summary>
        public bool IsWest { get { return Value < 0.0; } }

        /// <summary>
        ///     Convert an arbitrary angle to an East/West longitude in the range -180 to +180.
        ///     All angles are referenced to the prime meridian at Greenwich.
        /// </summary>
        /// <param name="hours">Arbitrary angle in decimal degrees</param>
        /// <returns>Longitude in decimal degrees east or west of the prime meridian at Greenwich</returns>
        public override double Normalize(double hours)
        {
            hours = base.Normalize(hours); // Convert to positive angle 0..360 degrees
            if (hours > 180.0)
                hours = -180.0 + (hours - 180.0);
            return hours;
        }

        /// <summary>
        ///     Convert an abitrary integral angle to an integral East/West longitude in range -179 to +180
        ///     All angles are referenced to the prime meridian at Greenwich.
        /// </summary>
        /// <param name="nDegrees">Arbitrary angle in whole degrees</param>
        /// <returns>Whole degrees east (+ve) or west (-ve) of Greenwich</returns>
        public override int Normalize(int nDegrees)
        {
            var dAngle = Normalize((double) nDegrees);

            // Truncate towards zero ( 1.5 -> 1.0; -1.5 -> -1.0)
            if (dAngle >= 0) // Positive angles
                return (int) Math.Floor(dAngle);
            return (int) Math.Ceiling(dAngle);
        }

        /// <summary>
        ///     Output the lattitude as a formatted string h dd°mm'ss"
        ///     (h is the hemisphere, N or S, dd is positive degrees).
        /// </summary>
        /// <returns>string object containing formatted result</returns>
        public override string ToString()
        {
            var strLat = string.Format("{0} {1:D3}°{2:D2}'{3:D2}\"", IsEast ? 'E' : 'W', Degrees, Minutes, Seconds);
            return strLat;
        }
    }
}