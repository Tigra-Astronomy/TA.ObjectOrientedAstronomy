// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: ISexagesimal.cs  Last modified: 2015-11-21@16:44 by Tim Long

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Interface ISexagesimal - implementors allow their underlying values to be broken up into a whole part (normally
    ///     representing degrees or hours) and fractional parts consisting of Minutes and Seconds.
    /// </summary>
    public interface ISexagesimal
    {
        /// <summary>
        ///     Gets the value. This is the whole value, not the integer part and includes any minutes and seconds.
        ///     The whole part can be obtained by simply casting to an integer.
        /// </summary>
        /// <value>The value.</value>
        double Value { get; }
        /// <summary>
        ///     Gets the minutes component of <see cref="Value" />.
        /// </summary>
        /// <value>The minutes.</value>
        uint Minutes { get; }
        /// <summary>
        ///     Gets the minutes component of <see cref="Value" />.
        /// </summary>
        /// <value>The seconds.</value>
        uint Seconds { get; }
    }
}