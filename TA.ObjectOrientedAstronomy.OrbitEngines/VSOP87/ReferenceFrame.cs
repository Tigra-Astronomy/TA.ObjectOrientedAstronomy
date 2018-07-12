// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: ReferenceFrame.cs  Last modified: 2016-10-08@23:28 by Tim Long

namespace TA.ObjectOrientedAstronomy.OrbitEngines.VSOP87
    {
    /// <summary>
    ///     Enum ReferenceFrame - represents a reference frame in time against which the coordinates produced by VSOP87
    ///     are valid. For different times, precession must be used.
    /// </summary>
    public enum ReferenceFrame
        {
        EquinoxJ2000,
        EquinoxJNow
        }
    }