// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: RightAscension.cs  Last modified: 2015-11-21@16:44 by Tim Long

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Right Ascension is the geocentric angular distance from the first point of Aries of an object on the celestial
    ///     sphere, expressed in hours (15 degrees equals one hour). Instances are immutable.
    /// </summary>
    public sealed class RightAscension : HourAngle
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RightAscension" /> class.
        /// </summary>
        /// <param name="rightAscension">The right ascension.</param>
        public RightAscension(double rightAscension) { Value = Normalize(rightAscension); }
    }
}