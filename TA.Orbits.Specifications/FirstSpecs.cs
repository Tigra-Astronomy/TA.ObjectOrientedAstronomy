// This file is part of the TA.Orbits project
// 
// Copyright © 2014 Tigra Astronomy, all rights reserved.
// 
// File: FirstSpecs.cs  Last modified: 2014-01-29@11:47 by Tim Long

using Machine.Specifications;
using TA.OrbitEngine.Vsop87;
using TA.Orbits.ReferenceData;

namespace TA.Orbits.Specifications
    {
    /*
     * Calculate the position in space of the Earth relative to the Sun for a given date, time
     * Give the answer in both cartesian coordinates (X,Y,Z)
     * and spherical coordinates (Latitude, Longitude and Radius).
     * 
     * Use a reference implementation to verify the results.
     */

    [Subject(typeof(VSOP87B_EarthPositionSpherical), "reference data")]
    public class when_computing_vsop87_term_l0_for_earth : with_target_date_2014_jan_29_midday
        {
        Establish context = () => ReferenceLatitude = VSOP87B_EarthPositionSpherical.Earth_L0(Rho);
        Because of = () => ComputedLatitude = Vsop87OrbitEngine.ComputeVsop87Term(TargetDate, 0.0, Vsop87Data.Vsop87B_Earth_L0);

        It should_match_the_reference_implementation =
            () => ComputedLatitude.ShouldBeCloseTo(ReferenceLatitude, Tolerance);
        }

    [Subject(typeof(VSOP87B_EarthPositionSpherical), "reference data")]
    public class when_computing_vsop87_term_l5_for_earth : with_target_date_2014_jan_29_midday
    {
        Establish context = () => ReferenceLatitude = VSOP87B_EarthPositionSpherical.Earth_L5(Rho);
        Because of = () => ComputedLatitude = Vsop87OrbitEngine.ComputeVsop87Term(TargetDate, 5.0, Vsop87Data.Vsop87B_Earth_L5);
        It should_match_the_reference_implementation =
            () => ComputedLatitude.ShouldBeCloseTo(ReferenceLatitude, Tolerance);
    }

    [Subject(typeof(VSOP87B_EarthPositionSpherical), "reference data")]
    public class when_computing_vsop87_latitude_for_earth : with_target_date_2014_jan_29_midday
        {
        Establish context = () =>
                ReferenceLatitude =
                    VSOP87B_EarthPositionSpherical.Earth_L0(Rho) + VSOP87B_EarthPositionSpherical.Earth_L1(Rho) +
                    VSOP87B_EarthPositionSpherical.Earth_L2(Rho) + VSOP87B_EarthPositionSpherical.Earth_L3(Rho) +
                    VSOP87B_EarthPositionSpherical.Earth_L4(Rho) + VSOP87B_EarthPositionSpherical.Earth_L5(Rho);
        Because of = () => ComputedLatitude = Vsop87OrbitEngine.ComputeVsop87Series(TargetDate, Vsop87Data.Vsop87B_Earth_Latitude);
        It should_match_the_reference_implementation =
            () => ComputedLatitude.ShouldBeCloseTo(ReferenceLatitude, Tolerance);
    }


    [Subject(typeof(VSOP87B_EarthPositionSpherical), "reference data")]
    public class when_reading_earth_vsop87_data_from_a_text_file
    {
        protected const double Tolerance = 0.00000000001;
        Because of = () => EarthData = Vsop87DataReader.LoadVsop87DataFromFile("VSOP87B.ear");
        It should_be_the_same_as_the_hard_coded_data = () => EarthData.VariableData['L'].ShouldBeLikeObjectGraph(Vsop87Data.Vsop87B_Earth_Latitude, Tolerance) ;
        static Vsop87Solution EarthData;
    }

    public class with_target_date_2014_jan_29_midday
        {
        protected const double J2000 = 2451545.0; // Julian date JD 2000.0
        protected const double Tolerance = 0.0000000000001; // 10E-14
        protected static double Rho;
        protected static double TargetDate;
        protected static double ReferenceLatitude;
        protected static double ComputedLatitude;
        Establish context = () =>
            {
            TargetDate = 2456687; // 29/Jan/2014 midday
            Rho = (TargetDate - 2451545.0)/365250.0;
            };
        }
    }
