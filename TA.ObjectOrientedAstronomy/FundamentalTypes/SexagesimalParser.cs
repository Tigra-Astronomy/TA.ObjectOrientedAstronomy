// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: SexagesimalParser.cs  Last modified: 2015-11-21@16:44 by Tim Long

using System;
using System.Text.RegularExpressions;
using NLog;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Provides static methods and properties for converting and parsing sexagesimal values.
    ///     <para>
    ///         Fot the purposes of this software, "sexagesimal" means any number having a whole part, (for example, 0 -
    ///         23 hours, 0 - 359 degrees) plus a number of minutes and seconds. The whole part of the number can be
    ///         positive or negative and of any magnitude. Minutes and seconds are unsigned (rather, they take their
    ///         sign from the whole part) and are in the range 0 &lt;= x &lt; 60.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     This helper function is designed to work in a number of different situations for strings containing angles;
    ///     right ascension; declination; altitude and azimuth so it is necessary to accept a variety of delimiters
    ///     after the first numeric field. Some of these delimiters may not be obvious but are required to support the
    ///     LX-200 computer-assisted telescope protocol, for example.
    /// </remarks>
    public sealed class SexagesimalParser
    {
        const string Pattern =
            @"^(?<Sign>[+-]?)(?<Whole>\d+)(?:[^0-9]+)(?<Minutes>\d{1,2})((?:[^0-9]+)(?<Seconds>\d{1,2})(?:[^0-9]*))?.*$";

        static readonly RegexOptions matchOptions = RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
                                                    | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture
                                                    | RegexOptions.Singleline;

        static readonly Logger log = LogManager.GetCurrentClassLogger();

        static readonly Regex RegexSexagesimal = new Regex(Pattern, matchOptions);

        /// <summary>
        ///     Parses a sexagesimal string and converts it to an equivalent floating point value in decimal.
        ///     <c>Sexagesimal</c> means "base 60" and is how we express clock time, in hours, minutes and seconds. In
        ///     astronomy, 'hour angles' are expressed identically to clock time so we can handle hour angles easily.  A
        ///     strict definition of "base 60" would exclude angles because the degrees (whole) part can be greater than
        ///     59 but in practice, a slightly looser definition is used: "of, relating to, or reckoning by sixtieths"
        ///     (Oxford English Dictionary).  This lets us treat angles as sexagesimal quantities for most practical
        ///     purposes, including parsing and formatting purposes.
        /// </summary>
        /// <param name="sexagesimal">A string containing a sexagesimal quantity.</param>
        /// <returns>The equivalent value expressed as floating point.</returns>
        /// <exception cref="ArgumentException">Thrown if the supplied string could not be parsed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the supplied sexagesimal string is null.</exception>
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
        ///                 Time in hours (24-hour format), minutes and seconds with any single character
        ///                 non-numeric separator and optional leading sign.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>sDDD:MM:SS</term>
        ///             <description>
        ///                 Degrees minutes and seconds, with any single non-numeric separator character and
        ///                 optional leading sign.
        ///             </description>
        ///         </item>
        ///     </list>
        ///     The sign is assumed to be positive if omitted. For more information about sexagesimal see
        ///     http://en.wikipedia.org/wiki/Sexagesimal
        /// </remarks>
        /// <exception>
        ///     A time-out occurred. For more information about time-outs, see the Remarks section.
        /// </exception>
        /// <exception cref="OverflowException">
        ///     The result represents a number that is less than
        ///     <see cref="F:System.Double.MinValue" /> or greater than <see cref="F:System.Double.MaxValue" />.
        /// </exception>
        public static double Parse(string sexagesimal)
        {
            if (sexagesimal == null)
                throw new ArgumentNullException("sexagesimal");
            if (!IsValid(sexagesimal))
            {
                log.Error("SexagesimalParser string [{0}] is not valid - throwing", sexagesimal);
                throw new ArgumentException("Not a valid sexagesimal string: " + sexagesimal, "sexagesimal");
            }
            var sgParsed = RegexSexagesimal.Match(sexagesimal);
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
            return RegexSexagesimal.IsMatch(sg);
        }
    }
}