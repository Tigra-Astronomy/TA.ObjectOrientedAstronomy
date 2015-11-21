// This file is part of the TA.Orbits project
// 
// Copyright © 2014 Tigra Astronomy, all rights reserved.
// 
// File: Vsop87DataReader.cs  Last modified: 2014-01-31@10:46 by Tim Long

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using NLog;

namespace TA.ObjectOrientedAstronomy.OrbitEngines
    {
    internal sealed class Vsop87DataReader
        {
        const string Vsop87HeaderRegexPattern =
            @"^\s*VSOP87 VERSION (?<VsopVersion>\w+)\s+(?<Body>\w+)\s+VARIABLE (?<Variable>\d+)\s+\((?<Variables>\w+)\)\s+\*T\*\*(?<Power>\d+)\s+(?<Terms>\d+)\s+TERMS\s+(?<Description>.*)";
        const string Vsop87DataRowRegexPattern = @"^(\s*[+-]*[0-9.]+){16}\s+(?<A>[0-9.]+)\s+(?<B>[0-9.]+)\s+(?<C>[0-9.]+).*$";
        internal static readonly Regex Vsop87HeaderRegex = new Regex(Vsop87HeaderRegexPattern, RegexOptions.Compiled);
        static readonly Regex Vsop87DataRowRegex = new Regex(Vsop87DataRowRegexPattern, RegexOptions.Compiled);
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static Vsop87Solution LoadVsop87DataFromFile(string filename)
            {
            Log.Info("Loading VSOP87 solution from {0}", filename);
            var dataFile = Path.Combine(@".\Vsop87_data", filename);
            var inputStream = File.OpenText(dataFile);
            var header = inputStream.ReadLine();
            var solution = Vsop87Solution.FromHeaderString(header);
            foreach (char variable in solution.CoordinateVariables)
                {
                Log.Debug("Loading variable {0}", variable);
                var variableIndex = solution.CoordinateVariables.IndexOf(variable);
                var series = LoadVariableSeries(variableIndex, inputStream);
                solution.CoordinateVariableSeriesData[variable] = series;
                }
            return solution;
            }

        /// <summary>
        /// Loads the variable series.
        /// </summary>
        /// <param name="variableIndex">Index of the variable.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>IEnumerable{IEnumerable{Vsop87Term}}.</returns>
        static IEnumerable<IEnumerable<Vsop87Term>> LoadVariableSeries(int variableIndex, StreamReader inputStream)
            {
            var majorSeries = new List<List<Vsop87Term>>();
            int power = 0;  // Used purely for diagnostics
            string currentVariable = (++variableIndex).ToString(CultureInfo.InvariantCulture);
            string headerVariable;
            do
                {
                Log.Debug("Loading terms for series with power={0}", power++);
                var minorSeries = new List<Vsop87Term>();
                var header = LoadPowerSeries(minorSeries, inputStream);
                majorSeries.Add(minorSeries);

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
                if (string.IsNullOrEmpty(line))
                    continue;
                Log.Debug("Read line: {0}", line);
                var rowMatch = Vsop87DataRowRegex.Match(line);
                if (rowMatch.Success)
                    {
                    var termA = double.Parse(rowMatch.Groups["A"].Value, CultureInfo.InvariantCulture);
                    var termB = double.Parse(rowMatch.Groups["B"].Value, CultureInfo.InvariantCulture);
                    var termC = double.Parse(rowMatch.Groups["C"].Value, CultureInfo.InvariantCulture);
                    var term = new Vsop87Term(termA, termB, termC);
                    Log.Debug("Recognized data row, A={0} B={1} C={2}", termA, termB, termC);
                    minorSeries.Add(term);
                    continue;
                    }
                var headerMatch = Vsop87HeaderRegex.Match(line);
                if (headerMatch.Success)
                    {
                    Log.Debug("Recognized header row");
                    return line;
                    }
                throw new InvalidOperationException("Data file was not in the expected format (could not recognize a data row or a header row)");
                }
            Log.Debug("Encountered end-of-stream while reading power series terms");
            return string.Empty;    // At end of stream, no header.
            }

        /// <summary>
        /// Selects the VSOP87 data file based on the body required and the type of coordinates required.
        /// </summary>
        /// <param name="solarSystemBody">The solar system body.</param>
        /// <param name="coordinateSystem">The coordinate system.</param>
        /// <param name="referenceFrame">The reference frame.</param>
        /// <returns>A string containing the filename and extension (no path) of the VSOP87 data file to use.</returns>
        internal static string SelectDataFile(SolarSystemBody solarSystemBody, CoordinateSystem coordinateSystem, ReferenceFrame referenceFrame)
            {
            string body;
            string variant;
            const string fileNameFormat = "VSOP87{0}.{1}";
            switch (solarSystemBody)
                {
                case SolarSystemBody.Sun:
                    // when the Sun is the target body only barycentric coordinates and equinox J2000 are valid.
                    if (coordinateSystem != CoordinateSystem.BarycentricRectangularCoordinates ||
                        referenceFrame != ReferenceFrame.EquinoxJ2000)
                        throw NotSupportedExceptionBuilder(
                            "The Sun may only be used with barycentric coordinates and equinox J2000",
                            solarSystemBody,
                            coordinateSystem,
                            referenceFrame);
                    body = "sun";
                    break;
                case SolarSystemBody.Mercury:
                    body = "mer";
                    break;
                case SolarSystemBody.Venus:
                    body = "ven";
                    break;
                case SolarSystemBody.Earth:
                    // When Earth is the target body, elliptic elements are not valid.
                    if (coordinateSystem == CoordinateSystem.HeliocentricEllipticElements)
                        throw NotSupportedExceptionBuilder(
                            "Earth cannot be used with elliptic elements, please use rectangular or spherical coordinates instead.",
                            solarSystemBody,
                            coordinateSystem,
                            referenceFrame);
                    body = "ear";
                    break;
                case SolarSystemBody.Mars:
                    body = "mar";
                    break;
                case SolarSystemBody.Jupiter:
                    body = "jup";
                    break;
                case SolarSystemBody.Saturn:
                    body = "sat";
                    break;
                case SolarSystemBody.Uranus:
                    body = "ura";
                    break;
                case SolarSystemBody.Neptune:
                    body = "nep";
                    break;
                case SolarSystemBody.EarthMoonBarycentre:
                    // When Earth-Moon Barycenter is the target body, only elliptical elements or rectangular J2000 coordinates are valid.
                    if (coordinateSystem == CoordinateSystem.HeliocentricSphericalCoordinates
                        || referenceFrame == ReferenceFrame.EquinoxJNow ||
                        coordinateSystem == CoordinateSystem.BarycentricRectangularCoordinates)
                        throw NotSupportedExceptionBuilder(
                            "Earth-Moon Barycentre is only valid with equinox J2000 and either elliptic or rectangular coordinates",
                            solarSystemBody,
                            coordinateSystem,
                            referenceFrame);
                    body = "emb";
                    break;
                default:
                    throw NotSupportedExceptionBuilder("The specified body is not supported by VSOP87",
                        solarSystemBody,
                        coordinateSystem,
                        referenceFrame);
                }

            switch (coordinateSystem)
                {
                    case CoordinateSystem.HeliocentricEllipticElements:
                    if (referenceFrame == ReferenceFrame.EquinoxJ2000)
                        {
                        variant = string.Empty;
                        }
                    else
                        {
                        throw NotSupportedExceptionBuilder(
                            "Equinox of date (JNow) is not supported for elliptic elements, please use J2000, rectangular or spherical coordinates instead.",
                            solarSystemBody,
                            coordinateSystem,
                            referenceFrame);
                        }
                    break;
                case CoordinateSystem.HeliocentricRectangularCoordinates:
                    variant = referenceFrame == ReferenceFrame.EquinoxJ2000 ? "A" : "C";
                    break;
                    case CoordinateSystem.HeliocentricSphericalCoordinates:
                    variant = referenceFrame == ReferenceFrame.EquinoxJ2000 ? "B" : "D";
                    break;
                    case CoordinateSystem.BarycentricRectangularCoordinates:
                    if (referenceFrame==ReferenceFrame.EquinoxJ2000)
                        {
                        variant = "E";
                        }
                    else
                        {
                        throw NotSupportedExceptionBuilder(
                            "Equinox of date (JNow) is not supported for barycentric coordinates, please use J2000 or heliocentric coordinates instead.",
                            solarSystemBody,
                            coordinateSystem,
                            referenceFrame);
                        }
                    break;
                default:
                    throw NotSupportedExceptionBuilder("The slected coordinate system is not supported by VSOP87",
                        solarSystemBody,
                        coordinateSystem,
                        referenceFrame);
                }

            var filename = string.Format(fileNameFormat, variant, body);
            return filename;
            }

        static Exception NotSupportedExceptionBuilder(string message, SolarSystemBody solarSystemBody, CoordinateSystem coordinateSystem, ReferenceFrame referenceFrame)
            {
            var ex = new NotSupportedException(message);
            ex.Data.Add("SolarSystemBody", solarSystemBody);
            ex.Data.Add("CoordinateSystem", coordinateSystem);
            ex.Data.Add("ReferenceFrame", referenceFrame);
            return ex;
            }
        }

    /// <summary>
    /// Class Vsop87Solution. Holds all of the data for one body and one VSOP87 version
    /// (coordinate system and reference frame). VSOP87 has 6 distinct variants so there
    /// may be up to 6 separate solutions for each body.
    /// </summary>
    public sealed class Vsop87Solution
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
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
        public string CoordinateVariables { get; private set; }
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
        public IDictionary<char, IEnumerable<IEnumerable<Vsop87Term>>> CoordinateVariableSeriesData { get; private set; }

        Vsop87Solution()
            {
            CoordinateVariableSeriesData = new Dictionary<char, IEnumerable<IEnumerable<Vsop87Term>>>();
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
            Log.Info("Creating VSOP87 solution with Body={0} Version={1} Variables={2} Description={3}",
                body,
                version,
                variables,
                description);
           return new Vsop87Solution {Body = body, Version = version, CoordinateVariables = variables, Description = description};
            }
        }
    }
