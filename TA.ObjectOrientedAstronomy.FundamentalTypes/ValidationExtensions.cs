// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: ValidationExtensions.cs  Last modified: 2015-11-21@16:44 by Tim Long

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    static class ValidationExtensions
    {
        /// <summary>
        ///     Bracket test.
        /// </summary>
        /// <param name="value">The value under test.</param>
        /// <param name="lower">The lower bound.</param>
        /// <param name="upper">The upper bound.</param>
        /// <returns><c>true</c> if value is within the specified range, <c>false</c>"/> if it is outside.</returns>
        public static bool InRange(this int value, int lower, int upper)
        {
            return value >= lower && value <= upper;
        }

        /// <summary>
        ///     Bracket test.
        /// </summary>
        /// <param name="value">The value under test.</param>
        /// <param name="lower">The lower bound.</param>
        /// <param name="upper">The upper bound.</param>
        /// <returns><c>true</c> if value is within the specified range, <c>false</c>"/> if it is outside.</returns>
        public static bool InRange(this double value, double lower, double upper)
        {
            return value >= lower && value <= upper;
        }
    }
}