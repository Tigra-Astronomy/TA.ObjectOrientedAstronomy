// This file is part of the TA.Orbits project
// 
// Copyright © 2014 Tigra Astronomy, all rights reserved.
// 
// File: FirstSpecs.cs  Last modified: 2014-01-26@06:07 by Tim Long

using Machine.Specifications;
using TA.Orbits.ReferenceData;

namespace TA.Orbits.Specifications
    {
    /*
     * Calculate the position in space of the Earth relative to the Sun for a given date, time
     * Give the answer in both cartesian coordinates (X,Y,Z)
     * and sperical coordinates (Latitude, Longitude and Radius).
     * 
     * Use a reference implementation to verify the results.
     */

    [Subject(typeof(VSOP87B_EarthPositionSpherical), "reference data")]
    public class when_computing_vsop87_term_l0_for_earth
        {
        Establish context = () =>
            {
            var rho = (J2000 - 2451545.0)/365250.0;
            ReferenceEarthL0 = VSOP87B_EarthPositionSpherical.Earth_L0(rho);
            OrbitEngine = new OrbitEngine.Vsop87.OrbitEngine();
            };

        Because of = () => ComputedEarthL0 = OrbitEngine.ComputeL0(J2000);

        It should_compute_l0_to_match_the_reference_implementation =
            () => ComputedEarthL0.ShouldBeCloseTo(ReferenceEarthL0, Tolerance);

        static double ReferenceEarthL0;
        static double ComputedEarthL0;
        static OrbitEngine.Vsop87.OrbitEngine OrbitEngine;
        static double Tolerance = 0.00000000000005; // 10E-14
        const double J2000 = 2451545.0; // Julian date JD 2000.0
        }
    }
