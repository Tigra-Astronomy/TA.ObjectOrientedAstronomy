// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: SexagesimalParser.cs  Last modified: 2016-10-15@04:14 by Tim Long

using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
    {
    /// <summary>
    ///     Provides static methods and properties for converting and parsing sexagesimal values.
    ///     <para>
    ///         <c>Sexagesimal</c> literally means "base 60" and is how we express clock time, in hours, minutes and
    ///         seconds. The term is often also used to refer to angles expressed in degrees, minutes and seconds. In
    ///         astronomy, 'hour angles' are expressed identically to clock time so we can handle hour angles easily. A
    ///         strict definition of "base 60" would exclude both times and angles because hours are constrained to be
    ///         less than 24 while degrees can be greater than 59. In practice, the accepted definition of sexagesimal
    ///         is:
    ///         <c>"of, relating to, or reckoning by sixtieths"</c> (Oxford English Dictionary).   This lets us treat
    ///         both times and angles as sexagesimal quantities for most practical purposes, including parsing and
    ///         formatting purposes.
    ///     </para>
    ///     <para>
    ///         For the purposes of this software, "sexagesimal" means any number having a whole part, (for example, 0 -
    ///         23 hours, 0 - 359 degrees) plus a number of minutes and seconds. The whole part of the number can be
    ///         positive or negative and of any magnitude. Minutes and seconds express magnitude only (they are unsigned
    ///         and implicitly have the same sign as the whole part). Minutes and seconds are in the range 0 &lt;= x
    ///         &lt; 60, i.e. base 60.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     This helper class is designed to work in a number of different situations for strings containing clock time,
    ///     angles, right ascension, declination, altitude and azimuth so it is necessary to accept a variety of
    ///     delimiters between the fields. Some of these delimiters may not be obvious but may be required to support
    ///     machine-readable communications protocols.
    /// </remarks>
    public sealed class SexagesimalParser
        {
        private const string Pattern =
            @"^(?<Sign>[+-]?)(?<Whole>\d+)(?:[^0-9]+)(?<Minutes>\d{1,2})((?:[^0-9]+)(?<Seconds>\d{1,2})(?:[^0-9]*))?.*$";

        private static readonly RegexOptions matchOptions = RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
                                                            | RegexOptions.CultureInvariant |
                                                            RegexOptions.ExplicitCapture
                                                            | RegexOptions.Singleline;

        private static readonly Regex RegexSexagesimal = new Regex(Pattern, matchOptions);

        /// <summary>
        ///     Parses a sexagesimal string and converts it to an equivalent floating point value in decimal.
        /// </summary>
        /// <param name="sexagesimal">A string containing a sexagesimal quantity.</param>
        /// <returns>The equivalent value expressed as floating point.</returns>
        /// <remarks>
        ///     The following sexagesimal formats are explicitly supported.
        ///     <list type="table">
        ///         <listheader>
        ///             <item>Format</item>
        ///             <item>Description</item>
        ///         </listheader>
        ///         <item>
        ///             <term>sHH:MM:SS</term>
        ///             <description>
        ///                 Time in hours (24-hour format), minutes and seconds with any single character non-numeric
        ///                 separator and optional leading sign.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>sDDD:MM:SS</term>
        ///             <description>
        ///                 Degrees minutes and seconds, with any single non-numeric separator character and optional
        ///                 leading sign.
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Other formats may also work. The parser accepts almost anything as a field separator provided it does
        ///     not interfere with the recognition of numbers. The sign is assumed to be positive if omitted. For more
        ///     information about sexagesimal see http://en.wikipedia.org/wiki/Sexagesimal
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if the supplied string could not be parsed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the supplied sexagesimal string is null.</exception>
        public static double Parse(string sexagesimal)
            {
            if (sexagesimal == null)
                throw new ArgumentNullException(nameof(sexagesimal));
            var sgParsed = RegexSexagesimal.Match(sexagesimal);
            if (!sgParsed.Success)
                {
                var message = $"Unable to parse sexagesimal string [{sexagesimal}]";
                throw new ArgumentException(message, nameof(sexagesimal));
                }
            var sign = sgParsed.Groups["Sign"].Value == "-" ? -1.0 : +1.0;
            var whole = Convert.ToDouble(sgParsed.Groups["Whole"].Value);
            var minutes = 0.0;
            if (sgParsed.Groups["Minutes"].Success)
                minutes = Convert.ToDouble(sgParsed.Groups["Minutes"].Value);
            var seconds = 0.0;
            if (sgParsed.Groups["Seconds"].Success)
                seconds = Convert.ToDouble(sgParsed.Groups["Seconds"].Value);
            return (whole + minutes / 60.0 + seconds / 3600.0) * sign;
            }

        /// <summary>
        ///     Checks whether the supplied string is in a valid sexagesimal format.
        ///     This check validates only the syntax of the string, it does not perform
        ///     any validity or range checking on the values.
        /// </summary>
        /// <param name="sg">The candidate string to be validated.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="sg" /> is null.</exception>
        public static bool IsValid(string sg)
            {
            Contract.Requires(sg != null);
            return RegexSexagesimal.IsMatch(sg);
            }
        }
    }