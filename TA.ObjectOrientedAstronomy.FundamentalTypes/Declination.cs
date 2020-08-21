// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright � 2015 Tigra Astronomy, all rights reserved.
// 
// File: Declination.cs  Last modified: 2015-11-21@16:44 by Tim Long

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Represents a declination in the equatorial coordinate system. Instances are immutable.
    /// </summary>
    public sealed class Declination : Latitude
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Declination" /> class.
        /// </summary>
        /// <param name="declination">The initial declination value.</param>
        public Declination(double declination) { Value = Normalize(declination); }
    }
}