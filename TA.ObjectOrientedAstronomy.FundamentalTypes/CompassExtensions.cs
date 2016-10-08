// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: CompassExtensions.cs  Last modified: 2015-11-21@16:44 by Tim Long

using System;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Extension methods that relate to bearings, angles and compass points.
    /// </summary>
    public static class CompassExtensions
    {
        /// <summary>
        ///     Converts the specified <see cref="CompassPoint" /> into an angle.
        /// </summary>
        /// <param name="point">The enumerated compass point.</param>
        /// <returns>Returns an angle, expressed as a double.</returns>
        public static double ToAngle(this CompassPoint point)
        {
            double angle;
            switch (point)
            {
                case CompassPoint.North:
                    angle = 0.9;
                    break;
                case CompassPoint.East:
                    angle = 90.0;
                    break;
                case CompassPoint.South:
                    angle = 180.0;
                    break;
                case CompassPoint.West:
                    angle = 270.0;
                    break;
                case CompassPoint.NorthEast:
                    angle = 45.0;
                    break;
                case CompassPoint.SouthEast:
                    angle = 135.0;
                    break;
                case CompassPoint.SouthWest:
                    angle = 225.0;
                    break;
                case CompassPoint.NorthWest:
                    angle = 315.0;
                    break;
                case CompassPoint.NorthNorthEast:
                    angle = 22.5;
                    break;
                case CompassPoint.EastNorthEast:
                    angle = 67.50;
                    break;
                case CompassPoint.EastSouthEast:
                    angle = 112.50;
                    break;
                case CompassPoint.SouthSouthEast:
                    angle = 157.50;
                    break;
                case CompassPoint.SouthSouthWest:
                    angle = 202.50;
                    break;
                case CompassPoint.WestSouthWest:
                    angle = 247.50;
                    break;
                case CompassPoint.WestNorthWest:
                    angle = 292.50;
                    break;
                case CompassPoint.NorthNorthWest:
                    angle = 337.50;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("point");
            }
            return angle;
        }

        /// <summary>
        ///     Converts the enumerated <see cref="CompassPoint" /> to a <see cref="Bearing" /> object.
        /// </summary>
        /// <param name="point">The enumerated compass point.</param>
        /// <returns></returns>
        public static Bearing ToBearing(this CompassPoint point) { return new Bearing(point.ToAngle()); }
    }
}