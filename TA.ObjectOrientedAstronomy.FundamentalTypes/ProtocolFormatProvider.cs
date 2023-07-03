// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright � 2015 Tigra Astronomy, all rights reserved.
// 
// File: ProtocolFormatProvider.cs  Last modified: 2015-11-21@16:44 by Tim Long

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Class ProtocolFormatProvider - holds a collection of custom formatters to be used for formatting the various
    ///     values needed by the device command protocol.
    /// </summary>
    public class ProtocolFormatProvider : Dictionary<Type, ICustomFormatter>, IFormatProvider, ICustomFormatter
    {
        /// <summary>
        ///     Converts the value of a specified object to an equivalent string representation using specified format and
        ///     culture-specific formatting information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">An object that supplies format information about the current instance.</param>
        /// <returns>
        ///     The string representation of the value of <paramref name="arg" />, formatted as specified by
        ///     <paramref name="format" /> and <paramref name="formatProvider" />.
        /// </returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var typeToFormat = arg.GetType();
            if (!Keys.Contains(typeToFormat))
                return HandleOtherFormats(format, arg);

            var formatter = this[typeToFormat];
            return formatter.Format(format, arg, formatProvider);
        }

        /// <summary>
        ///     Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>
        ///     An instance of the object specified by <paramref name="formatType" />, if the
        ///     <see cref="T:System.IFormatProvider" /> implementation can supply that type of object; otherwise, null.
        /// </returns>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            return null;
        }

        /// <summary>
        ///     Handles formatting for types that are not catered for by this custom formatter.
        ///     Hands off formatting to the type itself.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        /// <returns>System.String.</returns>
        string HandleOtherFormats(string format, object arg)
        {
            if (arg == null)
                return string.Empty;
            if (arg is IFormattable)
                return ((IFormattable) arg).ToString(format, CultureInfo.CurrentCulture);
            return arg.ToString();
        }
    }
}