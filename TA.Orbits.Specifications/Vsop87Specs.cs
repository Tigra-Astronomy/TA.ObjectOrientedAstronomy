// This file is part of the TA.Orbits project
// 
// Copyright © 2014 Tigra Astronomy, all rights reserved.
// 
// File: Vsop87Specs.cs  Last modified: 2014-02-01@13:48 by Tim Long

using System;
using System.Collections;
using Machine.Specifications;
using TA.OrbitEngine;
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
        Establish context = () => ReferenceRadius = VSOP87B_EarthPositionSpherical.Earth_L0(Rho);
        Because of =
            () => ComputedRadius = Vsop87OrbitEngine.ComputeVsop87Term(TargetDate, 0.0, Vsop87Data.Vsop87B_Earth_L0);

        It should_match_the_reference_implementation =
            () => ComputedRadius.ShouldBeCloseTo(ReferenceRadius, Tolerance);
        }

    [Subject(typeof(VSOP87B_EarthPositionSpherical), "reference data")]
    public class when_computing_vsop87_term_l5_for_earth : with_target_date_2014_jan_29_midday
        {
        Establish context = () => ReferenceRadius = VSOP87B_EarthPositionSpherical.Earth_L5(Rho);
        Because of =
            () => ComputedRadius = Vsop87OrbitEngine.ComputeVsop87Term(TargetDate, 5.0, Vsop87Data.Vsop87B_Earth_L5);
        It should_match_the_reference_implementation =
            () => ComputedRadius.ShouldBeCloseTo(ReferenceRadius, Tolerance);
        }

    [Subject(typeof(VSOP87B_EarthPositionSpherical), "reference data")]
    public class when_computing_vsop87_latitude_for_earth : with_target_date_2014_jan_29_midday
        {
        Establish context = () =>
            ReferenceRadius =
                VSOP87B_EarthPositionSpherical.Earth_L0(Rho) + VSOP87B_EarthPositionSpherical.Earth_L1(Rho) +
                VSOP87B_EarthPositionSpherical.Earth_L2(Rho) + VSOP87B_EarthPositionSpherical.Earth_L3(Rho) +
                VSOP87B_EarthPositionSpherical.Earth_L4(Rho) + VSOP87B_EarthPositionSpherical.Earth_L5(Rho);
        Because of =
            () => ComputedRadius = Vsop87OrbitEngine.ComputeVsop87Series(TargetDate, Vsop87Data.Vsop87B_Earth_Latitude);
        It should_match_the_reference_implementation =
            () => ComputedRadius.ShouldBeCloseTo(ReferenceRadius, Tolerance);
        }

    [Subject(typeof(Vsop87OrbitEngine), "data loaded from file")]
    public class when_computing_vsop87_radius_variable_for_earth_with_data_loaded_from_file :
        with_target_date_2014_jan_29_midday
        {
        Establish context = () =>
            {
            ReferenceRadius =
                VSOP87B_EarthPositionSpherical.Earth_R0(Rho) + VSOP87B_EarthPositionSpherical.Earth_R1(Rho) +
                VSOP87B_EarthPositionSpherical.Earth_R2(Rho) + VSOP87B_EarthPositionSpherical.Earth_R3(Rho) +
                VSOP87B_EarthPositionSpherical.Earth_R4(Rho) + VSOP87B_EarthPositionSpherical.Earth_R5(Rho);
            EarthData = Vsop87DataReader.LoadVsop87DataFromFile("VSOP87B.ear");
            };
        Because of =
            () => ComputedRadius = Vsop87OrbitEngine.ComputeVsop87Series(TargetDate, EarthData.VariableData['R']);
        It should_match_the_reference_implementation =
            () => ComputedRadius.ShouldBeCloseTo(ReferenceRadius, Tolerance);
        static Vsop87Solution EarthData;
        }

    [Subject(typeof(Vsop87DataReader))]
    public class when_reading_earth_vsop87_data_from_a_text_file
        {
        Because of = () => EarthData = Vsop87DataReader.LoadVsop87DataFromFile("VSOP87B.ear");
        It should_be_the_same_as_the_hard_coded_data =
            () => EarthData.VariableData['L'].ShouldBeLikeObjectGraph(Vsop87Data.Vsop87B_Earth_Latitude, Tolerance);
        static Vsop87Solution EarthData;
        protected const double Tolerance = 0.00000000001;
        }

    [Subject(typeof(Vsop87DataReader), "Data file selection")]
    public class when_selecting_a_data_file
        {
        It should_be_variant_base_emb_for_earth_moon_barycentre_j2000_elliptic_elements = () =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.EarthMoonBarycentre,
                CoordinateSystem.HeliocentricEllipticElements,
                ReferenceFrame.EquinoxJ2000).ShouldEqual("VSOP87.emb");
        It should_be_variant_a_mer_for_mercury_j2000_rectangular = () =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.Mercury,
                CoordinateSystem.HeliocentricRectangularCoordinates,
                ReferenceFrame.EquinoxJ2000).ShouldEqual("VSOP87A.mer");
        It should_be_variant_b_ven_for_venus_j2000_spherical = () =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.Venus,
                CoordinateSystem.HeliocentricSphericalCoordinates,
                ReferenceFrame.EquinoxJ2000).ShouldEqual("VSOP87B.ven");
        It should_be_variant_c_nep_for_neptune_jnow_rectangular = () =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.Neptune,
                CoordinateSystem.HeliocentricRectangularCoordinates,
                ReferenceFrame.EquinoxJNow).ShouldEqual("VSOP87C.nep");
        It should_be_variant_d_ear_for_earth_jnow_spherical = () =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.Earth,
                CoordinateSystem.HeliocentricSphericalCoordinates,
                ReferenceFrame.EquinoxJNow).ShouldEqual("VSOP87D.ear");
        It should_be_variant_e_sat_for_saturn_j2000_barycentric = () =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.Saturn,
                CoordinateSystem.BarycentricRectangularCoordinates,
                ReferenceFrame.EquinoxJ2000).ShouldEqual("VSOP87E.sat");
        }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_barycentric_coordinates_and_jnow
    {
        Because of = () => Thrown = Catch.Exception(() =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.Mars,
                CoordinateSystem.BarycentricRectangularCoordinates,
                ReferenceFrame.EquinoxJNow));
        It should_throw_not_supported_exception = () =>
            Thrown.ShouldBeOfType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.Mars));
        It should_contain_the_coordinate_system =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("CoordinateSystem", CoordinateSystem.BarycentricRectangularCoordinates));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJNow));
        static Exception Thrown;
    }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_elliptic_elements_and_jnow
    {
        Because of = () => Thrown = Catch.Exception(() =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.Mars,
                CoordinateSystem.HeliocentricEllipticElements,
                ReferenceFrame.EquinoxJNow));
        It should_throw_not_supported_exception = () =>
            Thrown.ShouldBeOfType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.Mars));
        It should_contain_the_coordinate_system =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("CoordinateSystem", CoordinateSystem.HeliocentricEllipticElements));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJNow));
        static Exception Thrown;
    }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_the_sun_and_non_barycentric_coordinates
    {
        Because of = () => Thrown = Catch.Exception(() =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.Sun,
                CoordinateSystem.HeliocentricRectangularCoordinates,
                ReferenceFrame.EquinoxJ2000));
        It should_throw_not_supported_exception = () =>
            Thrown.ShouldBeOfType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.Sun));
        It should_contain_the_coordinate_system =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("CoordinateSystem", CoordinateSystem.HeliocentricRectangularCoordinates));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJ2000));
        static Exception Thrown;
    }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_earth_moon_barycentre_and_jnow
    {
        Because of = () => Thrown = Catch.Exception(() =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.EarthMoonBarycentre,
                CoordinateSystem.HeliocentricRectangularCoordinates,
                ReferenceFrame.EquinoxJNow));
        It should_throw_not_supported_exception = () =>
            Thrown.ShouldBeOfType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.EarthMoonBarycentre));
        It should_contain_the_coordinate_system =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("CoordinateSystem", CoordinateSystem.HeliocentricRectangularCoordinates));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJNow));
        static Exception Thrown;
    }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_earth_moon_barycentre_and_spherical_coordinates
    {
        Because of = () => Thrown = Catch.Exception(() =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.EarthMoonBarycentre,
                CoordinateSystem.HeliocentricSphericalCoordinates,
                ReferenceFrame.EquinoxJ2000));
        It should_throw_not_supported_exception = () =>
            Thrown.ShouldBeOfType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.EarthMoonBarycentre));
        It should_contain_the_coordinate_system =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("CoordinateSystem", CoordinateSystem.HeliocentricSphericalCoordinates));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJ2000));
        static Exception Thrown;
    }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_earth_moon_barycentre_and_barycentric_coordinates
    {
        Because of = () => Thrown = Catch.Exception(() =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.EarthMoonBarycentre,
                CoordinateSystem.BarycentricRectangularCoordinates,
                ReferenceFrame.EquinoxJ2000));
        It should_throw_not_supported_exception = () =>
            Thrown.ShouldBeOfType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.EarthMoonBarycentre));
        It should_contain_the_coordinate_system =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("CoordinateSystem", CoordinateSystem.BarycentricRectangularCoordinates));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJ2000));
        static Exception Thrown;
    }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_earth_and_elliptic_elements
    {
        Because of = () => Thrown = Catch.Exception(() =>
            Vsop87DataReader.SelectDataFile(SolarSystemBody.Earth,
                CoordinateSystem.HeliocentricEllipticElements,
                ReferenceFrame.EquinoxJ2000));
        It should_throw_not_supported_exception = () =>
            Thrown.ShouldBeOfType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.Earth));
        It should_contain_the_coordinate_system =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("CoordinateSystem", CoordinateSystem.HeliocentricEllipticElements));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJ2000));
        static Exception Thrown;
    }


    #region Context Base Classes
    public class with_target_date_2014_jan_29_midday
        {
        protected const double J2000 = 2451545.0; // Julian date JD 2000.0
        protected const double Tolerance = 0.0000000000001; // 10E-14
        protected static double Rho;
        protected static double TargetDate;
        protected static double ReferenceRadius;
        protected static double ComputedRadius;
        Establish context = () =>
            {
            TargetDate = 2456687; // 29/Jan/2014 midday
            Rho = (TargetDate - 2451545.0)/365250.0;
            };
        }
    #endregion Context Base Classes
    }
