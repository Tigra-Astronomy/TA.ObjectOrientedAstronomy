// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: CompassPoint.cs  Last modified: 2015-11-21@16:44 by Tim Long

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
#pragma warning disable 1591 // Missing XML doc-comment, these names are intentionally self-describing.
    /// <summary>
    ///     Enumerate the sixteen major compass points
    /// </summary>
    public enum CompassPoint
    {
        North,
        East,
        South,
        West,
        NorthEast,
        SouthEast,
        SouthWest,
        NorthWest,
        NorthNorthEast,
        EastNorthEast,
        EastSouthEast,
        SouthSouthEast,
        SouthSouthWest,
        WestSouthWest,
        WestNorthWest,
        NorthNorthWest
    }
#pragma warning restore 1591
}