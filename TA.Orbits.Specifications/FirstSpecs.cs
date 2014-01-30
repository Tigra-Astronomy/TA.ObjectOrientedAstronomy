// This file is part of the TA.Orbits project
// 
// Copyright © 2014 Tigra Astronomy, all rights reserved.
// 
// File: FirstSpecs.cs  Last modified: 2014-01-29@11:47 by Tim Long

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
    public class when_computing_vsop87_term_l0_for_earth : with_target_date_2014_jan_29_midday
        {
        Establish context = () => ReferenceTerm = VSOP87B_EarthPositionSpherical.Earth_L0(Rho);
        Because of = () => ComputedTerm = OrbitEngine.ComputeVsop87Term(TargetDate, 0.0, Vsop87Data.Vsop87B_Earth_L0);

        It should_match_the_reference_implementation =
            () => ComputedTerm.ShouldBeCloseTo(ReferenceTerm, Tolerance);
        }

    [Subject(typeof(VSOP87B_EarthPositionSpherical), "reference data")]
    public class when_computing_vsop87_term_l5_for_earth : with_target_date_2014_jan_29_midday
        {
        Establish context = () => ReferenceTerm = VSOP87B_EarthPositionSpherical.Earth_L5(Rho);
        Because of = () => ComputedTerm = OrbitEngine.ComputeVsop87Term(TargetDate, 5.0, Vsop87Data.Vsop87B_Earth_L5);
        It should_match_the_reference_implementation =
            () => ComputedTerm.ShouldBeCloseTo(ReferenceTerm, Tolerance);
        }

    public class with_target_date_2014_jan_29_midday
        {
        protected const double J2000 = 2451545.0; // Julian date JD 2000.0
        protected const double Tolerance = 0.00000000000005; // 10E-14
        protected static OrbitEngine.Vsop87.OrbitEngine OrbitEngine;
        protected static double Rho;
        protected static double TargetDate;
        protected static double ReferenceTerm;
        protected static double ComputedTerm;
        Establish context = () =>
            {
            TargetDate = 2456687; // 29/Jan/2014 midday
            Rho = (TargetDate - 2451545.0)/365250.0;
            OrbitEngine = new OrbitEngine.Vsop87.OrbitEngine();
            };
        }
    }
