// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright � 2015 Tigra Astronomy, all rights reserved.
// 
// File: AngleBase.cs  Last modified: 2015-11-21@16:44 by Tim Long

using System;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Base class for angular measurements. Defines common characteristics of all angles.
    /// </summary>
    public abstract class AngleBase : IEquatable<AngleBase>, ISexagesimal
    {
        /// <summary>
        ///     The total number of seconds in the value, rounded to the nearest integer.
        /// </summary>
        protected int TotalSeconds; // The total number of whole seconds in the value.

        /// <summary>
        ///     An angular measurement
        /// </summary>
        double value;

        /// <summary>
        ///     Gets the minimum allowed value. <see cref="Value" /> must be greater than or equal to MinValue.
        /// </summary>
        /// <value>The minimum value allowed by this angular measurement.</value>
        public abstract double MinValue { get; }
        /// <summary>
        ///     Gets the maximum allowed value. <see cref="Value" /> must be <b>less than</b> (but not equal to) MaxValue.
        /// </summary>
        /// <value>The maximum allowed value.</value>
        /// <remarks>
        ///     MaxValue actually represents the smallest invalid value. Setting an angular measurement to MaxValue will cause an
        ///     exception to be thrown. <c>MaxValue - <see cref="double.MinValue" /></c> is actually the highest acceptable value.
        /// </remarks>
        public abstract double MaxValue { get; }
        /// <summary>
        ///     Gets the whole degrees. The value is truncated to the nearest integer towards zero.
        /// </summary>
        /// <value>The whole degrees.</value>
        public abstract uint Degrees { get; }

        /// <summary>
        ///     Gets the sign.
        /// </summary>
        /// <value>The sign.</value>
        public int Sign { get { return Math.Sign(Value); } }
        /// <summary>
        ///     Gets or Sets the decimal angle value.
        ///     The value is normalized before being stored so that it is within
        ///     the range <see cref="MinValue" /> to <see cref="MaxValue" />.
        /// </summary>
        public double Value
        {
            get
            {
                //Contract.Ensures(Contract.Result<double>() >= MinValue && Contract.Result<double>() <= MaxValue);
                return value;
            }
            protected set
            {
                //Contract.Ensures(Value >= MinValue && Value <= MaxValue);
                this.value = Normalize(value);
                TotalSeconds = ComputeTotalSeconds(value);
            }
        }
        /// <summary>
        ///     Gets the minutes component of the angle.
        ///     The value returned is truncated to the nearest integer towards zero.
        /// </summary>
        /// <remarks>
        ///     A minute is defined as one sixtieth of a whole angular unit.
        /// </remarks>
        public abstract uint Minutes { get; }
        /// <summary>
        ///     Gets the seconds component of the angle.
        ///     The value returned is rounded to the nearest integer.
        /// </summary>
        /// <remarks>
        ///     A second is defined as one sixtieth of a minute, or 1/3600th of a whole angular unit.
        /// </remarks>
        public abstract uint Seconds { get; }

        /// <summary>
        ///     Takes an angle (which can be any real number) and returns the
        ///     equivalent angle in the range <see cref="MinValue" /> &lt;= angle &lt; <see cref="MaxValue" />.
        /// </summary>
        /// <param name="degrees">Any double-precision value representing an angle or bearing.</param>
        /// <returns>The equivalent normalized angle.</returns>
        public abstract double Normalize(double degrees);

        /// <summary>
        ///     Takes an angle expressed as a signed integer, and returns the
        ///     equivalent positive angle in the range <see cref="MinValue" /> &lt;= angle &lt; <see cref="MaxValue" />.
        /// </summary>
        /// <param name="angle">Any integer value representing an angle or bearing.</param>
        /// <returns>The equivalent normalized angle.</returns>
        public abstract int Normalize(int angle);

        /// <summary>
        ///     Truncate a decimal number to the nearest integer towards zero (i.e. remove the fractional part)
        /// </summary>
        protected static double Truncate(double d)
        {
            if (d < 0.0)
                return Math.Ceiling(d);
            return Math.Floor(d);
        }

        /// <summary>
        ///     Converts an angle in whole degrees, minutes and seconds to a double.
        ///     The minutes and seconds values should be positive integers and they are
        ///     assumed to have the same sign as the degrees.
        /// </summary>
        /// <param name="degrees">The degrees, positive or negative value.</param>
        /// <param name="minutes">The minutes, positive value.</param>
        /// <param name="seconds">The seconds, positive value.</param>
        /// <returns>Returns the equivalent double.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if the degrees or minutes are not positive integers in the rang 0
        ///     to 59.
        /// </exception>
        /// <remarks>
        ///     FromDms(10,30,30) returns 10.55
        ///     FromDms(-10,30,30) returns -10.55
        ///     FromDms(-10,-30,-30) throws ArgumentInvalidException.
        /// </remarks>
        protected static double FromDms(int degrees, uint minutes, uint seconds)
        {
            if (minutes > 59)
                throw new ArgumentOutOfRangeException("minutes", minutes, "must be a positive integer in range 0..59");
            if (seconds > 59)
                throw new ArgumentOutOfRangeException("seconds", seconds, "must be a positive integer in range 0..59");
            double dd = degrees;
            var dm = minutes / 60.0;
            var ds = seconds / 3600.0;
            if (Math.Sign(degrees) < 0)
            {
                dm = -dm;
                ds = -ds;
            }
            return dd + dm + ds;
        }

        /// <summary>
        ///     Computes the total number of seconds in an angle, rounding to the nearest whole number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        int ComputeTotalSeconds(double value)
        {
            var fractionalSeconds = value * 60 * 60;
            var truncatedSeconds = Truncate(fractionalSeconds);
            return (int) truncatedSeconds;
        }

        #region Equality

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(AngleBase other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return value.Equals(other.value);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            var other = obj as AngleBase;
            return other != null && Equals(other);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() { return value.GetHashCode(); }

        /// <summary>
        ///     Compares to <see cref="AngleBase" /> instances for equality.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><c>true</c> if the instances are equal in value; <c>false</c> otherwise.</returns>
        public static bool operator ==(AngleBase left, AngleBase right) { return Equals(left, right); }

        /// <summary>
        ///     Compares two <see cref="AngleBase" /> instances for inequality.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Returns the logical inverse of the == operator.</returns>
        public static bool operator !=(AngleBase left, AngleBase right) { return !Equals(left, right); }

        #endregion Equality
    }
}