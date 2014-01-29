// This file is part of the TA.Orbits project
// 
// Copyright © 2014 Tigra Astronomy, all rights reserved.
// 
// File: Vsop87Term.cs  Last modified: 2014-01-29@03:45 by Tim Long

namespace TA.OrbitEngine.Vsop87
    {
    /// <summary>
    ///   Represents a single term for a VSOP87 computation. Each instance of this class corresponds to one record
    ///   in the raw VSOP87 data file.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The VSOP87 model of planetary motion (Jean Meeus, 1991, Astronomical Algorithms, Willmann-Bell, Richmond VA,
    ///     chapter 31 and appendix II;
    ///     P. Bretagnon, G. Francou, 1988, Planetary theories in rectangular and spherical variables, VSOP87 solutions,
    ///     Astronomy and Astrophysics, 202, p.309ff)
    ///     gives us polynomials in time for each planet Mercury to Uranus that are independent of the other planets. There are
    ///     three polynomials,
    ///     one each for geometric heliocentric longitude, latitude and distance. Presumably they are heliocentric and not
    ///     barycentric and the vectors point to
    ///     the planets' centres and not the centres of their planet-moon systems.
    ///   </para>
    ///   <para>
    ///     VSOP87 raw data and further information can be obtained from
    ///     <definitionTable>
    ///       <definedTerm>Centre de Données astronomiques de Strasbourg</definedTerm>
    ///       <defintion>http://cdsarc.u-strasbg.fr/cgi-bin/Cat?VI/81</defintion>
    ///       <definedTerm>AstroSurf</definedTerm>
    ///       <definition>http://www.astrosurf.com/jephem/astro/ephemeris/et300VSOP_en.htm#Presentation</definition>
    ///     </definitionTable>
    ///   </para>
    /// </remarks>
    public sealed class Vsop87Term
        {
        /// <summary>
        ///   Initializes a new instance of the <see cref="Vsop87Term" /> class.
        ///   This constructor is appropriate when only the Amplitude A, Phase B and Frequency C
        ///   values are required.
        /// </summary>
        /// <param name="amplitude">The amplitude.</param>
        /// <param name="phase">The phase.</param>
        /// <param name="frequency">The frequency.</param>
        public Vsop87Term(double amplitude, double phase, double frequency)
            {
            AmplitudeA = amplitude;
            PhaseB = phase;
            FrequencyC = frequency;
            }

        /// <summary>
        ///   Gets the Amplitude (A) coefficient.
        /// </summary>
        /// <value>
        ///   The Amplitude (A) coefficient expressed in au/(tjy**alpha) for the distances
        ///   and in rad/(tjy**alpha) for angular variables.
        /// </value>
        public double AmplitudeA { get; private set; }

        /// <summary>
        ///   Gets the Phase (B) coefficient.
        /// </summary>
        /// <value>The Phase (B) coefficient expressed in radians.</value>
        public double PhaseB { get; private set; }

        /// <summary>
        ///   Gets the Frequency (C) coefficient.
        /// </summary>
        /// <value>The Frequency (C) coefficient expressed in rad/tjy.</value>
        public double FrequencyC { get; private set; }
        };
    }
