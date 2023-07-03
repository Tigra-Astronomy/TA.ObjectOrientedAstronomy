// This file is part of the TA.Dpoint project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so. The Software comes with no warranty of any kind.
// You make use of the Software entirely at your own risk and assume all liability arising from your use thereof.
// 
// File: DomePosition.cs  Last modified: 2020-11-30@16:44 by Tim Long

using TA.ObjectOrientedAstronomy.FundamentalTypes;

namespace TA.ObjectOrientedAstronomy.Observatory
    {
    /// <summary>
    /// Describes the centre of an observing aperture through which the telescope must be able to see the sky. The position is relative to the dome centre of rotation. For a spherical dome, this will be the centre of the sphere of which the dome forms a part.
    /// </summary>
    public class DomePosition
        {
        public DomePosition(double azimuth, double elevation)
            {
            Azimuth = new Bearing(azimuth);
            Elevation=new Bearing(elevation);
            }
        public Bearing Azimuth { get; }
        public Bearing Elevation { get; }

        public static DomePosition FromRadians(double azimuth, double elevation)
            {
            var az = UnitConversionExtensions.RadiansToDegrees(azimuth);
            var el = UnitConversionExtensions.RadiansToDegrees(elevation);
            return new DomePosition(az,el);
            }

        /// <inheritdoc />
        public override string ToString()
            {
            return $"{nameof(Azimuth)}: {Azimuth}, {nameof(Elevation)}: {Elevation}";
            }
        }
    }