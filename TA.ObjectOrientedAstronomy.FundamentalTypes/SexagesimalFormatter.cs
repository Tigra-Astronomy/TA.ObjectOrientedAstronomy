// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: SexagesimalFormatter.cs  Last modified: 2015-11-21@16:44 by Tim Long

using System;
using System.Diagnostics.Contracts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Class SexagesimalFormatter. Provides custom formatting for objects that implement the
    ///     <see cref="ISexagesimal" /> interface.
    /// </summary>
    public class SexagesimalFormatter : IFormatProvider, ICustomFormatter
    {
        internal string formatString = "{0:D}°{1:00}'{2:00}\"";
        bool truncateMinutes = true;
        bool truncateSeconds = true;

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
            if (arg is ISexagesimal)
                return FormatSexagesimal(arg as ISexagesimal);
            if (arg is double || arg is float || arg is decimal)
                return FormatSexagesimal((double) arg);

            // The object being formatted is a type that we don't know how to format. Hand off the formatting job.
            var formattable = arg as IFormattable;
            return formattable == null ? arg.ToString() : formattable.ToString(format, formatProvider);
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
            return Thread.CurrentThread.CurrentCulture.GetFormat(formatType);
        }

        string FormatSexagesimal(ISexagesimal sexagesimal)
        {
            Contract.Requires(sexagesimal != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return FormatSexagesimal(sexagesimal.Value);
        }

        string FormatSexagesimal(double value)
        {
            var wholePart = (int) value;
            var positiveValue = Math.Abs(value);
            var minutesPart = positiveValue * 60.0 % 60.0;
            var secondsPart = positiveValue * 3600.0 % 60.0;
            if (truncateMinutes)
                minutesPart = Math.Truncate(minutesPart);
            if (truncateSeconds)
                secondsPart = Math.Truncate(secondsPart);
            return string.Format(formatString, wholePart, minutesPart, secondsPart);
        }

        /// <summary>
        ///     Creates a sexagesimal formatter and infers the required formatting from a specimen string.
        /// </summary>
        /// <param name="specimen">The specimen.</param>
        /// <param name="forceSign">Forces the sign to be displayed even for positive values.</param>
        /// <returns>TA.MeadeUnifiedDriver.Server.SexagesimalFormatter.</returns>
        public static SexagesimalFormatter InferFromSpecimenString(string specimen, bool forceSign = false)
        {
            Contract.Requires(!string.IsNullOrEmpty(specimen));
            Contract.Ensures(Contract.Result<SexagesimalFormatter>() != null);
            const string valueParser = @"(?<D>\d+)(?<DS>.{1})((?<M>\d*\.\d+)|(?<M>\d+))((?<MS>.{1})(?<S>\d+))?";
            var regex = new Regex(valueParser);
            var matches = regex.Match(specimen);
            if (!matches.Success)
                throw new Exception(); // ToDo - be more specific
            // Assumption: the value must have two or three numeric components, and one or two separators.
            // Assumption: Only the last numeric component may contain a decimal point, the others must be integers.
            var numericComponents = matches.Groups["S"].Success ? 3 : 2;
            var lastNumericGroup = numericComponents == 3 ? "S" : "M";
            var lastNumericValue = matches.Groups[lastNumericGroup].Value;
            var lastNumericIsDecimal = lastNumericValue.Contains(".");
            var formatBuilder = new StringBuilder();

            // Degrees or hours component {0:+000;-000}
            var minimumdigits = matches.Groups["D"].Value.Length;
            var pattern = new string('0', minimumdigits);
            formatBuilder.Append("{0:");
            if (forceSign || specimen.StartsWith("+"))
            {
                formatBuilder.Append('+');
                formatBuilder.Append(pattern);
                formatBuilder.Append(";-");
            }
            formatBuilder.Append(pattern);
            formatBuilder.Append('}');
            formatBuilder.Append(matches.Groups["DS"].Value); // Degrees separator

            // Minutes component
            if (numericComponents <= 2 && lastNumericIsDecimal)
                formatBuilder.Append("{1:00.0}");
            else
                formatBuilder.Append("{1:00}");
            if (numericComponents > 2)
            {
                formatBuilder.Append(matches.Groups["MS"].Value); // Minutes separator

                // Seconds component
                formatBuilder.Append(lastNumericIsDecimal ? "{2:00.0}" : "{2:00}");
            }

            // Create and configure the custom formatter instance.
            var formatter = new SexagesimalFormatter();
            formatter.formatString = formatBuilder.ToString();
            formatter.truncateMinutes = numericComponents != 2 || !lastNumericIsDecimal;
            formatter.truncateSeconds = numericComponents == 3 && !lastNumericIsDecimal;
            return formatter;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() { return string.Format("{0}", formatString); }
    }
}