// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: HourAngle.cs  Last modified: 2015-11-21@16:44 by Tim Long

using System;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     HourAngle - angular measurement expressed in hours minutes and seconds. 24 hours is equivalent to 360 degrees.
    ///     Instances are immutable.
    /// </summary>
    public class HourAngle : AngleBase
    {
        /// <summary>
        ///     Construct and HourAngle object from integer hours, minutes and seconds.
        /// </summary>
        /// <param name="hours">Number of whole degrees, range 0..359.</param>
        /// <param name="minutes">Number of whole minutes, range 0..59.</param>
        /// <param name="seconds">Number of whole seconds, range 0..59.</param>
        public HourAngle(int hours, uint minutes, uint seconds)
        {
            Value = FromDms(hours, minutes, seconds);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HourAngle" /> class.
        /// </summary>
        /// <param name="hours">The hour angle.</param>
        public HourAngle(double hours)
        {
            Value = hours;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HourAngle" /> class.
        /// </summary>
        protected HourAngle() {}

        /// <summary>
        ///     Gets the number of whole hours, ignoring any minutes and seconds.
        /// </summary>
        /// <value>The number of whole hours, rounded towards zero.</value>
        public int Hours { get { return (int) Truncate(Value); } }

        /// <summary>
        ///     Gets the minimum allowed value. <see cref="AngleBase.Value" /> will always be greater than or equal to MinValue.
        /// </summary>
        /// <value>The minimum value allowed by this angular measurement.</value>
        public override double MinValue { get { return 0.0; } }

        /// <summary>
        ///     Gets the maximum allowed value.
        ///     <see cref="MinValue" /> ? <see cref="AngleBase.Value" /> ? <see cref="MaxValue" />.
        /// </summary>
        /// <value>The maximum allowed value.</value>
        public override double MaxValue { get { return 24.0 - double.Epsilon; } }

        /// <summary>
        ///     Gets the whole degrees. The value is truncated to the nearest integer towards zero.
        /// </summary>
        /// <value>The whole degrees.</value>
        public override uint Degrees { get { return (uint) Truncate(Math.Abs(Value) * 15); } }

        /// <summary>
        ///     Gets the minutes component of the angle.
        ///     The value returned is truncated to the nearest integer towards zero.
        /// </summary>
        /// <value>The minutes.</value>
        /// <remarks>A minute is defined as one sixtieth of a whole angular unit.</remarks>
        public override uint Minutes
        {
            get
            {
                var positiveSeconds = Math.Abs(TotalSeconds);
                return (uint) (Truncate(positiveSeconds / 60.0) % 60);
            }
        }

        /// <summary>
        ///     Gets the seconds component of the angle.
        ///     The value returned is rounded to the nearest integer.
        /// </summary>
        /// <value>The seconds.</value>
        /// <remarks>A second is defined as one sixtieth of a minute, or 1/3600th of a whole angular unit.</remarks>
        public override uint Seconds
        {
            get
            {
                var positiveSeconds = Math.Abs(TotalSeconds);
                return (uint) (positiveSeconds % 60);
            }
        }

        /// <summary>
        ///     Normalizes the specified value into the 24-hour range.
        /// </summary>
        /// <param name="hours">The value to be normalized.</param>
        /// <returns>Normalized value in the range 0.0 .. 24.0 - <see cref="double.Epsilon" />.</returns>
        public override double Normalize(double hours)
        {
            hours %= MaxValue; // All angles are modulo 24 hours
            if (hours < MinValue) // -ve angles are subtracted from 24.
                hours = MaxValue + hours;
            return hours;
        }

        /// <summary>
        ///     Normalizes the specified value into the 24-hour range.
        /// </summary>
        /// <param name="hours">The value to be normalized.</param>
        /// <returns>Normalized value in the range 0 .. 23.</returns>
        public override int Normalize(int hours)
        {
            hours %= (int) MaxValue; // All hour-angles are modulo 24 hours.
            if (hours < (int) MinValue) // -ve values are subtracted from the maximum.
                hours = (int) MaxValue + hours;
            return hours;
        }

        /// <summary>
        ///     Convert an hour angle to a string representation HHh MMm SSs
        ///     where HH, MM and SS are the hour, minute and second values respectively
        ///     and h, m and s are literal characters.
        /// </summary>
        /// <returns>String representation of an hour angle</returns>
        public override string ToString()
        {
            var strHrAng = Hours.ToString("D2") + "h " + Minutes.ToString("D2") + "m " + Seconds.ToString("D2") + "s";
            return strHrAng;
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="first">The first hour angle.</param>
        /// <param name="second">The second hour angle.</param>
        /// <returns>The result of the addition of the two hour angles, as a new HourAngle instance.</returns>
        public static HourAngle operator +(HourAngle first, HourAngle second)
        {
            return new HourAngle(first.Value + second.Value);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="first">The first hour angle.</param>
        /// <param name="second">The second hour angle.</param>
        /// <returns>The result of the second operand from the first, as a new HourAngle instance.</returns>
        public static HourAngle operator -(HourAngle first, HourAngle second)
        {
            return new HourAngle(first.Value - second.Value);
        }
    }
}