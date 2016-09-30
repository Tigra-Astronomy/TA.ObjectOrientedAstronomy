// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: LatitudeSpecs.cs  Last modified: 2015-11-21@16:46 by Tim Long

using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FundamentalTypes;

namespace TA.ObjectOrientedAstronomy.Specifications
{

    #region Value coersion

    [Subject(typeof(Latitude), "coersion")]
    class when_creating_a_latitude_of_91
    {
        Because of = () => UUT = new Latitude(91.0);
        It should_coerce_to_89 = () => UUT.Value.ShouldBeCloseTo(89.0);
        static Latitude UUT;
    }

    [Subject(typeof(Latitude), "coersion")]
    class when_creating_a_Latitude_of_minus_91
    {
        Because of = () => UUT = new Latitude(-91.0);
        It should_coerce_to_minus_89 = () => UUT.Value.ShouldBeCloseTo(-89.0);
        static Latitude UUT;
    }

    [Subject(typeof(Latitude), "coersion")]
    class when_creating_a_Latitude_of_180
    {
        Because of = () => UUT = new Latitude(180.0);
        It should_coerce_to_0 = () => UUT.Value.ShouldBeCloseTo(0.0);
        static Latitude UUT;
    }

    [Subject(typeof(Latitude), "coersion")]
    class when_creating_a_Latitude_of_270
    {
        Because of = () => UUT = new Latitude(270.0);
        It should_coerce_to_minus_90 = () => UUT.Value.ShouldBeCloseTo(-90.0);
        static Latitude UUT;
    }

    [Subject(typeof(Latitude), "coersion")]
    class when_creating_a_Latitude_of_slightly_less_than_90
    {
        Because of = () =>
        {
            var angle = 90.0 - 0.1 / 3600.0; // tenth of an arc second
            UUT = new Latitude(angle);
        };

        It should_not_coerce_to_zero = () => UUT.Value.ShouldBeCloseTo(90.0, 1.0 / 3600.0);
        static Latitude UUT;
    }

    [Subject(typeof(Latitude), "coersion")]
    class when_creating_a_Latitude_of_359
    {
        Because of = () => { UUT = new Latitude(359.0); };
        It should_coerce_to_minus_1 = () => UUT.Value.ShouldBeCloseTo(-1.0);
        static Latitude UUT;
    }

    [Subject(typeof(Latitude), "coersion")]
    class when_creating_equivalent_angles_in_various_quadrants
    {
        Because of = () => { };
        It should_coerce_135_to_45 = () => new Latitude(135).ShouldEqual(new Latitude(45));
        It should_coerce_minus_135_to_minus_45 = () => new Latitude(-135).ShouldEqual(new Latitude(-45));
        static Latitude UUT;
    }

    #endregion Value coersion

    #region Conversion to DMS

    [Subject(typeof(Latitude), "sexagesimal")]
    class when_converting_latitude_to_sexagesimal
    {
        Because of = () =>
        {
            RaDeneb = 20.6999491773451; // This value caused AWR-88
            UUT = new Latitude(RaDeneb);
        };
        Behaves_like<DenebRightAscension> deneb;
        protected static Latitude UUT;
        static double RaDeneb;
    }

    #endregion Conversion to DMS

    #region Hemisphere

    [Subject(typeof(Latitude), "Hemisphere")]
    class when_testing_the_hemisphere_of_a_latitude
    {
        Because of = () => { };
        It should_be_north_when_pos_0 = () => new Latitude(0).IsNorth.ShouldBeTrue();
        It should_be_north_when_pos_90 = () => new Latitude(90).IsNorth.ShouldBeTrue();
        It should_be_south_when_neg_1 = () => new Latitude(-1).IsSouth.ShouldBeTrue();
        It should_be_south_when_neg_90 = () => new Latitude(-90).IsSouth.ShouldBeTrue();
        It should_not_be_north_when_neg_1 = () => new Latitude(-1).IsNorth.ShouldBeFalse();
        It should_not_be_north_when_neg_90 = () => new Latitude(-90).IsNorth.ShouldBeFalse();
        It should_not_be_south_when_pos_0 = () => new Latitude(0).IsSouth.ShouldBeFalse();
        It should_not_be_south_when_pos_90 = () => new Latitude(90).IsSouth.ShouldBeFalse();
    }

    #endregion Hemisphere

    #region Formatting

    [Subject(typeof(Latitude), "Formatting")]
    class when_formatting_a_positive_latitude
    {
        Establish context;
        Because of = () => Latitude = new Latitude(+89.5);
        It should_format_correctly = () => Latitude.ToString().ShouldEqual("N 89°30'00\"");
        static Latitude Latitude;
    }

    [Subject(typeof(Latitude), "Formatting")]
    class when_formatting_a_negative_latitude
    {
        Establish context;
        Because of = () => Latitude = new Latitude(-89.5);
        It should_format_correctly = () => Latitude.ToString().ShouldEqual("S 89°30'00\"");
        It should_have_a_negative_sign = () => Latitude.Sign.ShouldEqual(-1);
        static Latitude Latitude;
    }

    #endregion Formatting
}