// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: MathExtensions.cs  Last modified: 2016-10-07@05:19 by Tim Long

using System;
using System.Diagnostics.Contracts;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     Extension methods for numerical values.
    /// </summary>
    public static class MathExtensions
        {
        /// <summary>
        ///     Performs a histogram stretch.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="lowerThreshold">The lower threshold.</param>
        /// <param name="upperThreshold">The upper threshold.</param>
        /// <returns>System.Double.</returns>
        public static double HistogramStretch(this double value, double lowerThreshold, double upperThreshold)
            {
            return value.Constrain(lowerThreshold, upperThreshold)
                .MapToRange(lowerThreshold, upperThreshold, 0, short.MaxValue);
            }

        /// <summary>
        ///     Clips (truncates) the input value so that it is within the specified range.
        /// </summary>
        /// <typeparam name="T">Any type that implements <see cref="IComparable" />.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="lowerBound">The lower bound.</param>
        /// <returns>
        ///     An instance of <typeparamref name="T" /> that is in the specified range. If <typeparamref name="T" /> is a
        ///     reference type, then the result will be a reference to the original input value, the
        ///     <paramref name="upperBound" /> or <paramref name="lowerBound" />.
        /// </returns>
        public static T Constrain<T>(this T input, T lowerBound, T upperBound) where T : IComparable
            {
            Contract.Requires(upperBound.CompareTo(lowerBound) >= 0);
            Contract.Ensures(Contract.Result<T>() != null);
            var result = input.CompareTo(upperBound) > 0 ? upperBound : input;
            result = lowerBound.CompareTo(result) > 0 ? lowerBound : result;
            return result;
            }

        /// <summary>
        ///     Maps (i.e. scales and translates) a value from a source range to a destination range.
        /// </summary>
        /// <param name="input">The input, which must lie within the source range.</param>
        /// <param name="sourceMinimum">The source range minimum.</param>
        /// <param name="sourceMaximum">The source range maximum.</param>
        /// <param name="destMinimum">The destination range minimum.</param>
        /// <param name="destMaximum">The destination range maximum.</param>
        /// <returns>The mapped value.</returns>
        public static double MapToRange(
            this double input, double sourceMinimum, double sourceMaximum, double destMinimum, double destMaximum)
            {
            var sourceRange = sourceMaximum - sourceMinimum;
            var destRange = destMaximum - destMinimum;
            var sourceAbsoluteOffset = input - sourceMinimum;
            var sourceFraction = sourceAbsoluteOffset / sourceRange;
            var destAbsoluteOffset = destRange * sourceFraction;
            var destResult = destMinimum + destAbsoluteOffset;
            return destResult;
            }

        /// <summary>
        ///     Determines whether an object is within a specified range.
        /// </summary>
        /// <typeparam name="T">Any object that implements <see cref="IComparable" />.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="lowerBound">The inclusive lower bound of the range.</param>
        /// <param name="upperBound">The inclusive upper bound of the range.</param>
        /// <returns><c>true</c> if the input is within the specified range; otherwise, <c>false</c>.</returns>
        [Pure]
        public static bool IsInRange<T>(this T input, T lowerBound, T upperBound) where T : IComparable
            {
            Contract.Requires(lowerBound.CompareTo(upperBound) < 1);
            return lowerBound.CompareTo(input) <= 0 && upperBound.CompareTo(input) >= 0;
            }
        }
    }