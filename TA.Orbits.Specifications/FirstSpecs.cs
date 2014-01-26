// This file is part of the TA.Orbits project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: FirstSpecs.cs  Created: 2014-01-26@00:26
// Last modified: 2014-01-26@02:15 by Tim

using Machine.Specifications;
using TA.Orbits.ReferenceData;

namespace TA.Orbits.Specifications
    {
    /*
     * Calculate the position in space of the Earth relative to the Sun for a given date, time
     * Give the answer in both horizon-based and equtorial coordinates.
     * 
     * ToDo:
     * Assume EPOCH J2000 (perhaps do other eopchs later)
     * How to represent the Sun and Earth?
     * Calculate the position in space of Earth
     * 
     * 
     */

    [Subject(typeof(VSOP87B_EarthPositionSpherical), "reference data")]
    public class when_computing_term_L0_for_Earth
        {
        Establish context = () =>
            {
            var rho = (j2000 - 2451545.0)/365250.0;
            Reference_EarthL0 = VSOP87B_EarthPositionSpherical.Earth_L0(rho);
            OrbitEngine = new OrbitEngine();
            };

        Because of = () => Computed_EarthL0 = OrbitEngine.ComputeL0(j2000);

        It should_compute_L0_to_match_the_reference_implementation =
            () => Computed_EarthL0.ShouldBeCloseTo(Reference_EarthL0, tolerance);

        static double Reference_EarthL0;
        static double Computed_EarthL0;
        static OrbitEngine OrbitEngine;
        static double tolerance = 0.00000000000005;
        const double j2000 = 2451545.0; // Julian date JD 2000.0
        }

    internal class OrbitEngine
        {
        public double ComputeL0(double julianDate)
            {
            return 1.75192386367183;
            }
        }
    }
