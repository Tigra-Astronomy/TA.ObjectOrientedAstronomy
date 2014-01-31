// This file is part of the TA.Orbits project
// 
// Copyright © 2014 Tigra Astronomy, all rights reserved.
// 
// File: Vsop87DataReader.cs  Last modified: 2014-01-31@10:46 by Tim Long

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using TiGra;

namespace TA.OrbitEngine.Vsop87
    {
    public class Vsop87DataReader
        {
        const string Vsop87HeaderRegexPattern =
            @"^\s*VSOP87 VERSION (?<VsopVersion>\w+)\s+(?<Body>\w+)\s+VARIABLE (?<Variable>\d+)\s+\((?<Variables>\w+)\)\s+\*T\*\*(?<Power>\d+)\s+(?<Terms>\d+)\s+TERMS\s+(?<Description>.*)";
        const string Vsop87DataRowRegexPattern = @"^(\s*[+-]*[0-9.]+){16}\s+(?<A>[0-9.]+)\s+(?<B>[0-9.]+)\s+(?<C>[0-9.]+).*$";
        internal static Regex Vsop87HeaderRegex = new Regex(Vsop87HeaderRegexPattern, RegexOptions.Compiled);
        internal static Regex Vsop87DataRowRegex = new Regex(Vsop87DataRowRegexPattern, RegexOptions.Compiled);

        public static Vsop87Solution LoadVsop87DataFromFile(string filename)
            {
            Diagnostics.TraceInfo("Loading VSOP87 solution from {0}", filename);
            var dataFile = Path.Combine(@".\Vsop87_data", filename);
            var inputStream = File.OpenText(dataFile);
            var header = inputStream.ReadLine();
            var currentLine = header;
            var solution = Vsop87Solution.FromHeaderString(header);
            foreach (char variable in solution.Variables)
                {
                Diagnostics.TraceVerbose("Loading variable {0}", variable);
                var variableIndex = solution.Variables.IndexOf(variable);
                var series = LoadVariableSeries(solution, variableIndex, inputStream);
                solution.VariableData[variable] = series;
                }
            return solution;
            }

        /// <summary>
        /// Loads the variable series.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="variableIndex">Index of the variable.</param>
        /// <returns>IEnumerable{IEnumerable{Vsop87Term}}.</returns>
        static IEnumerable<IEnumerable<Vsop87Term>> LoadVariableSeries(Vsop87Solution solution, int variableIndex, StreamReader inputStream)
            {
            var majorSeries = new List<List<Vsop87Term>>();
            int power = 0;
            string currentVariable = (++variableIndex).ToString(CultureInfo.InvariantCulture);
            string headerVariable = currentVariable;
            do
                {
                var minorSeries = new List<Vsop87Term>();
                var header = LoadPowerSeries(minorSeries, inputStream);
                majorSeries.Add(minorSeries);
                ++power;

                if (string.IsNullOrEmpty(header))
                    break;  // End of stream

                var match = Vsop87HeaderRegex.Match(header);
                headerVariable = match.Groups["Variable"].Value;
                }
            while (headerVariable == currentVariable);
            return majorSeries;
            }

        /// <summary>
        /// Loads the terms in a power series for one power of one coordinate variable,
        /// up to the next header row or end-of-stream. If a header row is found,
        /// it is returned.
        /// </summary>
        /// <param name="minorSeries">The minor series.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>System.String containing the next header, or String.Empty if at end-of-stream.</returns>
        /// <exception cref="System.InvalidOperationException">Data file was not in the expected format (could not recognize a data row or a header row)</exception>
        static string LoadPowerSeries(List<Vsop87Term> minorSeries, StreamReader inputStream)
            {
            while (!inputStream.EndOfStream)
                {
                var line = inputStream.ReadLine();
                Diagnostics.TraceVerbose("Read line: {0}", line);
                var rowMatch = Vsop87DataRowRegex.Match(line);
                if (rowMatch.Success)
                    {
                    var termA = double.Parse(rowMatch.Groups["A"].Value, CultureInfo.InvariantCulture);
                    var termB = double.Parse(rowMatch.Groups["B"].Value, CultureInfo.InvariantCulture);
                    var termC = double.Parse(rowMatch.Groups["C"].Value, CultureInfo.InvariantCulture);
                    var term = new Vsop87Term(termA, termB, termC);
                    Diagnostics.TraceVerbose("Recognized data row, A={0} B={1} C={2}", termA, termB, termC);
                    minorSeries.Add(term);
                    continue;
                    }
                var headerMatch = Vsop87HeaderRegex.Match(line);
                if (headerMatch.Success)
                    {
                    Diagnostics.TraceVerbose("Recognized header row");
                    return line;
                    }
                throw new InvalidOperationException("Data file was not in the expected format (could not recognize a data row or a header row)");
                }
            Diagnostics.TraceVerbose("Encountered end-of-stream while reading power series terms");
            return string.Empty;    // At end of stream, no header.
            }
        }

    /// <summary>
    /// Class Vsop87Solution. Holds all of the data for one body and one VSOP87 version
    /// (coordinate system and reference frame). VSOP87 has 6 distinct variants so there
    /// may be up to 6 separate solutions for each body.
    /// </summary>
    public class Vsop87Solution
        {
            /// <summary>
            /// Gets the name of the body to which this solution applies.
            /// </summary>
            /// <value>The body name as an upper-case string.</value>
        public string Body { get; private set; }
        /// <summary>
        /// Gets the VSOP87 version (variant) represented by this data.
        /// A VSOP87 version defines the coordinate system and reference frame,
        /// e.g. HELIOCENTRIC DYNAMICAL ECLIPTIC and EQUINOX J2000
        /// </summary>
        /// <value>The version, a string containing a single upper-case letter and a single decimal digit.</value>
        public string Version { get; private set; }
        /// <summary>
        /// Gets the coordinate variables that can be computed with this data.
        /// </summary>
        /// <value>The variables, one character for each variable, concatenated together in a string.</value>
        public string Variables { get; private set; }
        /// <summary>
        /// Gets the description of the coordinate system and reference frame represented by this data.
        /// </summary>
        /// <value>The description, as obtained from the VSOP87 data file.</value>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the variable data for this VSOP87 solution.
        /// The variables (coordinates) are contained in a dictionary, where the key
        /// is the variable name and the value is the series data for that variable.
        /// </summary>
        /// <value>The variable data.</value>
        public IDictionary<char, IEnumerable<IEnumerable<Vsop87Term>>> VariableData { get; private set; }

        public Vsop87Solution()
            {
            VariableData = new Dictionary<char, IEnumerable<IEnumerable<Vsop87Term>>>();
            }
        internal static Vsop87Solution FromHeaderString(string header)
            {
            var matches = Vsop87DataReader.Vsop87HeaderRegex.Match(header);
            if (!matches.Success)
                throw new ArgumentException("The supplied header was invalid", "header");
            var body = matches.Groups["Body"].Value;
            var version = matches.Groups["VsopVersion"].Value;
            var variables = matches.Groups["Variables"].Value;
            var description = matches.Groups["Description"].Value;
            Diagnostics.TraceInfo("Creating VSOP87 solution with Body={0} Version={1} Variables={2} Description={3}",
                body,
                version,
                variables,
                description);
           return new Vsop87Solution {Body = body, Version = version, Variables = variables, Description = description};
            }
        }
    }
