// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: CoordinateSystem.cs  Last modified: 2016-10-08@23:28 by Tim Long

namespace TA.ObjectOrientedAstronomy.OrbitEngines.VSOP87
    {
    /// <summary>
    ///     Enum CoordinateSystem - represents a coordinate system that can be produced by VSOP87
    /// </summary>
    public enum CoordinateSystem
        {
        HeliocentricEllipticElements,
        HeliocentricRectangularCoordinates,
        HeliocentricSphericalCoordinates,
        BarycentricRectangularCoordinates
        }
    }