// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: Bearing.cs  Last modified: 2016-10-15@04:14 by Tim Long

using System;
using System.Diagnostics.Contracts;
using System.Xml.Serialization;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
    {
    /// <summary>
    ///     Implements a Compass bearing (i.e. an angle greater than or equal to zero but less than 360 degrees). Angles
    ///     are stored internally as double precision floating point, values are always positive, in the range 0 to 360
    ///     degrees. The angle can be initialised or set to a negative value, but it is always reduced to an equivalent
    ///     positive angle before it is stored. Thus, the retrieved value must always be positive and in the range 0 ≤ x
    ///     &lt; 360
    /// </summary>
    public class Bearing : AngleBase
        {
        private const int FullCircle = 360;

        /// <summary>
        ///     Construct a bearing from decimal degrees.
        ///     Any valid double may be supplied, and it will be normalized to the
        ///     equivalent positive angle such that
        ///     <see cref="MinValue" /> ≤ <paramref name="angle" /> ≤  <see cref="MaxValue" />.
        /// </summary>
        /// <param name="angle">The angle value in decimal degrees.</param>
        public Bearing(double angle)
            {
            Value = angle;
            }

        /// <summary>
        ///     Construct a bearing from degrees, minutes and seconds.
        ///     The sign of the angle is contained in the degrees. All components are assumed
        ///     to have the same sign as the degrees.
        /// </summary>
        /// <param name="degrees">
        ///     signed integer number of degrees. Any integer value,
        ///     the value will be converted to an equivalent positive angle in the range [0..359]
        /// </param>
        /// <param name="minutes">unsigned integer number of minutes [0..59]</param>
        /// <param name="seconds">unsigned integer number of seconds [0..59]</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="minutes" /> or <paramref name="seconds" /> or not positive integers in the range
        ///     [0..59].
        /// </exception>
        public Bearing(int degrees, uint minutes, uint seconds)
            {
            if (minutes > 59)
                throw new ArgumentOutOfRangeException("minutes", minutes, "Value out of range (0..59)");
            if (seconds > 59)
                throw new ArgumentOutOfRangeException("seconds", seconds, "Value out of range (0..59)");
            Value = Normalize(FromDms(degrees, minutes, seconds));
            }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bearing" /> class.
        /// </summary>
        protected Bearing() : this(default(double)) {}

        /// <summary>
        ///     Gets the minimum allowed value. <see cref="AngleBase.Value" /> must be greater than or equal to MinValue.
        /// </summary>
        /// <value>The minimum value allowed by this angular measurement.</value>
        public override double MinValue => 0.0;

        /// <summary>
        ///     Gets the maximum allowed value that can be internally stored as a bearing.
        ///     <see cref="MinValue" /> ≤ <see cref="AngleBase.Value" /> ≤  <see cref="MaxValue" />.
        /// </summary>
        /// <value>The maximum allowed value.</value>
        /// <remarks>
        ///     Note that <c>MaxValue</c> is slightly less than 360 degrees, because 360 degrees is actually an invalid value for a
        ///     bearing.
        /// </remarks>
        public override double MaxValue
            {
            get { return 360.0 - double.Epsilon; }
            }

        /// <summary>
        ///     Gets the minutes component of the angle.
        ///     The value returned is the absolute value truncated to the nearest integer towards zero.
        ///     Minutes are always reported as positive integers, but conceptually their sign is the same
        ///     as the sign of <see cref="AngleBase.Value" />.
        /// </summary>
        /// <value>Whole minutes as an integer, truncated towards zero.</value>
        /// <remarks>
        ///     A minute is defined as one sixtieth of a whole angular unit.
        ///     A Bearing with Value=10.525 would return 15 seconds.
        /// </remarks>
        public override uint Minutes
            {
            get
                {
                var positiveSeconds = Math.Abs(TotalSeconds);
                return (uint) (Truncate(positiveSeconds / 60) % 60);
                }
            }

        /// <summary>
        ///     Gets the fractional part of the angle expressed as whole seconds.
        ///     The value returned is the absolute value truncated the the nearest integer towards zero.
        ///     Seconds are always reported as a positive integer but conceptually their sign is the same
        ///     as that of <see cref="AngleBase.Value" />.
        /// </summary>
        /// <value>Whole seconds as a positive integer, truncated towards zero.</value>
        /// <remarks>
        ///     A second is defined as one sixtieth of a minute, or 1/3600th of a whole angular unit.
        ///     An Bearing with Value=10.55 would return 30 minutes.
        /// </remarks>
        public override uint Seconds
            {
            get
                {
                var positiveSeconds = Math.Abs(TotalSeconds);
                return (uint) (positiveSeconds % 60);
                }
            }

        /// <summary>
        ///     Gets the absolute number of whole degrees as an integer in the range 0..360.
        /// </summary>
        [XmlIgnore]
        public override uint Degrees
            {
            get
                {
                // Return the degrees, truncated towards zero.
                // Bearings cannot have negative values, but the functionality is provided
                // here for classes that inherit from Bearing.
                return (uint) Truncate(Math.Abs(Value));
                }
            }

        /// <summary>
        ///     Takes an angle (which can be any double number) and returns the
        ///     equivalent angle in the range +0.0 to +360.0
        /// </summary>
        /// <param name="degrees">Any double-precision value representing an angle or bearing.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="degrees" /> is Infinity or NaN.</exception>
        /// <returns>Equivalent angle in the range +0.0 to +360.0</returns>
        public override double Normalize(double degrees)
            {
            if (double.IsInfinity(degrees) || double.IsNaN(degrees))
                throw new ArgumentException("Value must not be Infinity or NaN", "degrees");
            degrees %= FullCircle;
            if (degrees < MinValue)
                degrees = FullCircle + degrees;
            return degrees;
            }

        /// <summary>
        ///     Takes an angle expressed as a signed integer in degrees, and returns the
        ///     equivalent positive angle in the range [0..360].
        /// </summary>
        /// <param name="degrees">An angle in degrees</param>
        /// <returns>Equivalent positive angle in the range [0..360]</returns>
        public override int Normalize(int degrees)
            {
            degrees %= FullCircle;
            if (degrees < 0)
                degrees = FullCircle + degrees;
            return degrees; // All angles are modulo 360 degrees.
            }

        /// <summary>
        ///     Output the compass bearing as a string ddd°mm'ss"
        /// </summary>
        /// <returns>string containing formatted result</returns>
        public override string ToString()
            {
            var strBearing = Degrees.ToString("D3") + "°" + Minutes.ToString("D2") + "'" + Seconds.ToString("D2") + "\"";
            return strBearing;
            }

        #region Operators
        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="first">The first operand.</param>
        /// <param name="second">The second operand.</param>
        /// <returns>The result of the addition of the two operands, as a new <see cref="Bearing" />.</returns>
        public static Bearing operator +(Bearing first, Bearing second)
            {
            Contract.Requires(first != null);
            Contract.Requires(second != null);
            Contract.Ensures(Contract.Result<Bearing>() != null);
            return new Bearing(first.Value + second.Value);
            }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="first">The first operand.</param>
        /// <param name="second">The second operand.</param>
        /// <returns>The result of the subtraction of the second operand from the first, as a new <see cref="Bearing" />.</returns>
        public static Bearing operator -(Bearing first, Bearing second)
            {
            Contract.Requires(first != null);
            Contract.Requires(second != null);
            Contract.Ensures(Contract.Result<Bearing>() != null);
            return new Bearing(first.Value - second.Value);
            }
        #endregion
        }
    }