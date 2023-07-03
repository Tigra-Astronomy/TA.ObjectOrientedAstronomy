// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright � 2015 Tigra Astronomy, all rights reserved.
// 
// File: GeodeticCoordinates.cs  Last modified: 2015-11-21@16:44 by Tim Long

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Describes a location upon The Earth, expressed as a <see cref="FundamentalTypes.Latitude" /> (angular distance
    ///     from the equator) and a <see cref="Longitude" /> (angular distance from the Prime Meridian).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The Prime Meridian is, by convention, the arc segment that includes the north and south geogrpahic poles and
    ///         the Royal Observatory, Greenwich UK.
    ///     </para>
    /// </remarks>
    public sealed class GeodeticCoordinates
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GeodeticCoordinates" /> class.
        /// </summary>
        public GeodeticCoordinates()
        {
            Latitude = new Latitude(0.0);
            Longitude = new Longitude(0.0);
        }

        /// <summary>
        ///     Site lattitude
        /// </summary>
        public Latitude Latitude { get; set; }

        /// <summary>
        ///     Site longitude
        /// </summary>
        public Longitude Longitude { get; set; }

        /// <summary>
        ///     Convert a geographical coordinate to string representation.
        /// </summary>
        /// <returns>string, formatted representation of the coordinates.</returns>
        public override string ToString() { return Latitude + ", " + Longitude; }
    }
}