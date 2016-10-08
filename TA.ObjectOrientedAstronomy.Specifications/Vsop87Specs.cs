// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: Vsop87Specs.cs  Last modified: 2016-10-08@23:31 by Tim Long

using System;
using System.Collections;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using TA.ObjectOrientedAstronomy.FundamentalTypes;
using TA.ObjectOrientedAstronomy.OrbitEngines.VSOP87;
using TA.Orbits.ReferenceData;

namespace TA.ObjectOrientedAstronomy.Specifications
    {

    #region  Context base classes
    public class with_target_date_2014_jan_29_midday
        {
        [UsedImplicitly] Establish context = () =>
            {
            TargetDate = 2456687; // 29/Jan/2014 midday
            Rho = (TargetDate - J2000) / 365250.0;
            };
        protected static double ComputedRadius;
        protected static double ReferenceRadius;
        protected static double Rho;
        protected static double TargetDate;
        protected const double J2000 = 2451545.0; // Julian date JD 2000.0
        protected const double Tolerance = 0.0000000000001; // 10E-14
        }
    #endregion

    /*
    * Calculate the position in space of the Earth relative to the Sun for a given date, time
    * Give the answer in both cartesian coordinates (X,Y,Z)
    * and spherical coordinates (Latitude, Longitude and Radius).
    * 
    * Use a reference implementation to verify the results.
    */

    [Subject(typeof(Vsop87OrbitEngine), "compute coordinates")]
    public class when_computing_spherical_j2000_coordinates_for_earth : with_target_date_2014_jan_29_midday
        {
        Establish context = () =>
            {
            var latitude = VSOP87B_EarthPositionSphericalJ2000.Earth_L0(Rho)
                           + VSOP87B_EarthPositionSphericalJ2000.Earth_L1(Rho)
                           + VSOP87B_EarthPositionSphericalJ2000.Earth_L2(Rho)
                           + VSOP87B_EarthPositionSphericalJ2000.Earth_L3(Rho)
                           + VSOP87B_EarthPositionSphericalJ2000.Earth_L4(Rho)
                           + VSOP87B_EarthPositionSphericalJ2000.Earth_L5(Rho);
            var longitude = VSOP87B_EarthPositionSphericalJ2000.Earth_B0(Rho)
                            + VSOP87B_EarthPositionSphericalJ2000.Earth_B1(Rho)
                            + VSOP87B_EarthPositionSphericalJ2000.Earth_B2(Rho)
                            + VSOP87B_EarthPositionSphericalJ2000.Earth_B3(Rho)
                            + VSOP87B_EarthPositionSphericalJ2000.Earth_B4(Rho)
                            + VSOP87B_EarthPositionSphericalJ2000.Earth_B5(Rho);
            var radius = VSOP87B_EarthPositionSphericalJ2000.Earth_R0(Rho)
                         + VSOP87B_EarthPositionSphericalJ2000.Earth_R1(Rho)
                         + VSOP87B_EarthPositionSphericalJ2000.Earth_R2(Rho)
                         + VSOP87B_EarthPositionSphericalJ2000.Earth_R3(Rho)
                         + VSOP87B_EarthPositionSphericalJ2000.Earth_R4(Rho)
                         + VSOP87B_EarthPositionSphericalJ2000.Earth_R5(Rho);
            ReferenceCoordinates = new SphericalCoordinates(latitude, longitude, radius);
            };
        Because of =
            () =>
                ComputedCoordinates =
                    Vsop87OrbitEngine.ComputeSphericalCoordinates(
                        TargetDate, SolarSystemBody.Earth, ReferenceFrame.EquinoxJ2000);
        It should_match_the_reference_latitude =
            () => ComputedCoordinates.Latitude.ShouldBeCloseTo(ReferenceCoordinates.Latitude, Tolerance);
        It should_match_the_reference_longitude =
            () => ComputedCoordinates.Longitude.ShouldBeCloseTo(ReferenceCoordinates.Longitude, Tolerance);
        It should_match_the_reference_radius =
            () => ComputedCoordinates.Radius.ShouldBeCloseTo(ReferenceCoordinates.Radius, Tolerance);
        static SphericalCoordinates ComputedCoordinates;
        static SphericalCoordinates ReferenceCoordinates;
        }

    [Subject(typeof(Vsop87OrbitEngine), "compute coordinates")]
    public class when_computing_rectangular_j2000_coordinates_for_earth : with_target_date_2014_jan_29_midday
        {
        Establish context = () =>
            {
            var x = VSOP87A_EarthPositionRectangularJ2000.Earth_X0(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_X1(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_X2(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_X3(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_X4(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_X5(Rho);
            var y = VSOP87A_EarthPositionRectangularJ2000.Earth_Y0(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_Y1(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_Y2(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_Y3(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_Y4(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_Y5(Rho);
            var z = VSOP87A_EarthPositionRectangularJ2000.Earth_Z0(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_Z1(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_Z2(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_Z3(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_Z4(Rho)
                    + VSOP87A_EarthPositionRectangularJ2000.Earth_Z5(Rho);
            ReferenceCoordinates = new RectangularCoordinates(x, y, z);
            };
        Because of =
            () =>
                ComputedCoordinates =
                    Vsop87OrbitEngine.ComputeRectangularCoordinates(
                        TargetDate, SolarSystemBody.Earth, ReferenceFrame.EquinoxJ2000);
        It should_match_the_reference_x = () => ComputedCoordinates.X.ShouldBeCloseTo(ReferenceCoordinates.X, Tolerance);
        It should_match_the_reference_y = () => ComputedCoordinates.Y.ShouldBeCloseTo(ReferenceCoordinates.Y, Tolerance);
        It should_match_the_reference_z = () => ComputedCoordinates.Z.ShouldBeCloseTo(ReferenceCoordinates.Z, Tolerance);
        static RectangularCoordinates ComputedCoordinates;
        static RectangularCoordinates ReferenceCoordinates;
        }

    [Subject(typeof(Vsop87OrbitEngine), "data loaded from file")]
    public class when_computing_vsop87_radius_variable_for_earth_with_data_loaded_from_file
        : with_target_date_2014_jan_29_midday
        {
        Establish context = () =>
            {
            ReferenceRadius = VSOP87B_EarthPositionSphericalJ2000.Earth_R0(Rho)
                              + VSOP87B_EarthPositionSphericalJ2000.Earth_R1(Rho)
                              + VSOP87B_EarthPositionSphericalJ2000.Earth_R2(Rho)
                              + VSOP87B_EarthPositionSphericalJ2000.Earth_R3(Rho)
                              + VSOP87B_EarthPositionSphericalJ2000.Earth_R4(Rho)
                              + VSOP87B_EarthPositionSphericalJ2000.Earth_R5(Rho);
            EarthData = Vsop87DataReader.LoadVsop87DataFromFile("VSOP87B.ear");
            };
        Because of =
            () =>
                ComputedRadius =
                    Vsop87OrbitEngine.ComputeVsop87Series(TargetDate, EarthData.CoordinateVariableSeriesData['R']);
        It should_match_the_reference_implementation = () => ComputedRadius.ShouldBeCloseTo(ReferenceRadius, Tolerance);
        static Vsop87Solution EarthData;
        }

    [Subject(typeof(Vsop87DataReader), "Data file selection")]
    public class when_selecting_a_data_file
        {
        It should_be_variant_base_emb_for_earth_moon_barycentre_j2000_elliptic_elements =
            () =>
                Vsop87DataReader.SelectDataFile(
                    SolarSystemBody.EarthMoonBarycentre, CoordinateSystem.HeliocentricEllipticElements,
                    ReferenceFrame.EquinoxJ2000).ShouldEqual("VSOP87.emb");
        It should_be_variant_a_mer_for_mercury_j2000_rectangular =
            () =>
                Vsop87DataReader.SelectDataFile(
                    SolarSystemBody.Mercury, CoordinateSystem.HeliocentricRectangularCoordinates,
                    ReferenceFrame.EquinoxJ2000).ShouldEqual("VSOP87A.mer");
        It should_be_variant_b_ven_for_venus_j2000_spherical =
            () =>
                Vsop87DataReader.SelectDataFile(
                    SolarSystemBody.Venus, CoordinateSystem.HeliocentricSphericalCoordinates,
                    ReferenceFrame.EquinoxJ2000).ShouldEqual("VSOP87B.ven");
        It should_be_variant_c_nep_for_neptune_jnow_rectangular =
            () =>
                Vsop87DataReader.SelectDataFile(
                    SolarSystemBody.Neptune, CoordinateSystem.HeliocentricRectangularCoordinates,
                    ReferenceFrame.EquinoxJNow).ShouldEqual("VSOP87C.nep");
        It should_be_variant_d_ear_for_earth_jnow_spherical =
            () =>
                Vsop87DataReader.SelectDataFile(
                    SolarSystemBody.Earth, CoordinateSystem.HeliocentricSphericalCoordinates,
                    ReferenceFrame.EquinoxJNow).ShouldEqual("VSOP87D.ear");
        It should_be_variant_e_sat_for_saturn_j2000_barycentric =
            () =>
                Vsop87DataReader.SelectDataFile(
                    SolarSystemBody.Saturn, CoordinateSystem.BarycentricRectangularCoordinates,
                    ReferenceFrame.EquinoxJ2000).ShouldEqual("VSOP87E.sat");
        }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_barycentric_coordinates_and_jnow
        {
        Because of =
            () =>
                Thrown =
                    Catch.Exception(
                        () =>
                            Vsop87DataReader.SelectDataFile(
                                SolarSystemBody.Mars, CoordinateSystem.BarycentricRectangularCoordinates,
                                ReferenceFrame.EquinoxJNow));
        It should_throw_not_supported_exception = () => Thrown.ShouldBeOfExactType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.Mars));
        It should_contain_the_coordinate_system =
            () =>
                Thrown.Data.ShouldContain(
                    new DictionaryEntry("CoordinateSystem", CoordinateSystem.BarycentricRectangularCoordinates));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJNow));
        static Exception Thrown;
        }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_elliptic_elements_and_jnow
        {
        Because of =
            () =>
                Thrown =
                    Catch.Exception(
                        () =>
                            Vsop87DataReader.SelectDataFile(
                                SolarSystemBody.Mars, CoordinateSystem.HeliocentricEllipticElements,
                                ReferenceFrame.EquinoxJNow));
        It should_throw_not_supported_exception = () => Thrown.ShouldBeOfExactType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.Mars));
        It should_contain_the_coordinate_system =
            () =>
                Thrown.Data.ShouldContain(
                    new DictionaryEntry("CoordinateSystem", CoordinateSystem.HeliocentricEllipticElements));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJNow));
        static Exception Thrown;
        }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_the_sun_and_non_barycentric_coordinates
        {
        Because of =
            () =>
                Thrown =
                    Catch.Exception(
                        () =>
                            Vsop87DataReader.SelectDataFile(
                                SolarSystemBody.Sun, CoordinateSystem.HeliocentricRectangularCoordinates,
                                ReferenceFrame.EquinoxJ2000));
        It should_throw_not_supported_exception = () => Thrown.ShouldBeOfExactType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.Sun));
        It should_contain_the_coordinate_system =
            () =>
                Thrown.Data.ShouldContain(
                    new DictionaryEntry("CoordinateSystem", CoordinateSystem.HeliocentricRectangularCoordinates));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJ2000));
        static Exception Thrown;
        }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_earth_moon_barycentre_and_jnow
        {
        Because of =
            () =>
                Thrown =
                    Catch.Exception(
                        () =>
                            Vsop87DataReader.SelectDataFile(
                                SolarSystemBody.EarthMoonBarycentre, CoordinateSystem.HeliocentricRectangularCoordinates,
                                ReferenceFrame.EquinoxJNow));
        It should_throw_not_supported_exception = () => Thrown.ShouldBeOfExactType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.EarthMoonBarycentre));
        It should_contain_the_coordinate_system =
            () =>
                Thrown.Data.ShouldContain(
                    new DictionaryEntry("CoordinateSystem", CoordinateSystem.HeliocentricRectangularCoordinates));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJNow));
        static Exception Thrown;
        }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_earth_moon_barycentre_and_spherical_coordinates
        {
        Because of =
            () =>
                Thrown =
                    Catch.Exception(
                        () =>
                            Vsop87DataReader.SelectDataFile(
                                SolarSystemBody.EarthMoonBarycentre, CoordinateSystem.HeliocentricSphericalCoordinates,
                                ReferenceFrame.EquinoxJ2000));
        It should_throw_not_supported_exception = () => Thrown.ShouldBeOfExactType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.EarthMoonBarycentre));
        It should_contain_the_coordinate_system =
            () =>
                Thrown.Data.ShouldContain(
                    new DictionaryEntry("CoordinateSystem", CoordinateSystem.HeliocentricSphericalCoordinates));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJ2000));
        static Exception Thrown;
        }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_earth_moon_barycentre_and_barycentric_coordinates
        {
        Because of =
            () =>
                Thrown =
                    Catch.Exception(
                        () =>
                            Vsop87DataReader.SelectDataFile(
                                SolarSystemBody.EarthMoonBarycentre, CoordinateSystem.BarycentricRectangularCoordinates,
                                ReferenceFrame.EquinoxJ2000));
        It should_throw_not_supported_exception = () => Thrown.ShouldBeOfExactType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.EarthMoonBarycentre));
        It should_contain_the_coordinate_system =
            () =>
                Thrown.Data.ShouldContain(
                    new DictionaryEntry("CoordinateSystem", CoordinateSystem.BarycentricRectangularCoordinates));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJ2000));
        static Exception Thrown;
        }

    [Subject(typeof(Vsop87DataReader), "data file selection")]
    public class when_selecting_an_unsupported_configuration_of_earth_and_elliptic_elements
        {
        Because of =
            () =>
                Thrown =
                    Catch.Exception(
                        () =>
                            Vsop87DataReader.SelectDataFile(
                                SolarSystemBody.Earth, CoordinateSystem.HeliocentricEllipticElements,
                                ReferenceFrame.EquinoxJ2000));
        It should_throw_not_supported_exception = () => Thrown.ShouldBeOfExactType<NotSupportedException>();
        It should_contain_the_body =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("SolarSystemBody", SolarSystemBody.Earth));
        It should_contain_the_coordinate_system =
            () =>
                Thrown.Data.ShouldContain(
                    new DictionaryEntry("CoordinateSystem", CoordinateSystem.HeliocentricEllipticElements));
        It should_contain_the_reference_frame =
            () => Thrown.Data.ShouldContain(new DictionaryEntry("ReferenceFrame", ReferenceFrame.EquinoxJ2000));
        static Exception Thrown;
        }
    }