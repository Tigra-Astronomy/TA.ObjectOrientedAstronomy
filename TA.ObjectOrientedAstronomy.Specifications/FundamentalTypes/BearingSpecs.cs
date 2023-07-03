// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2018 Tigra Astronomy, all rights reserved.
// 
// File: BearingSpecs.cs  Last modified: 2018-07-12@05:45 by Tim Long

using JetBrains.Annotations;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FundamentalTypes;

namespace TA.ObjectOrientedAstronomy.Specifications.FundamentalTypes
    {
    [Subject(typeof(Bearing), "coersion")]
    [UsedImplicitly]
    class when_creating_a_bearing_of_360
        {
        Because of = () => UUT = new Bearing(360.0);

        It should_coerce_to_zero = () => UUT.Value.ShouldBeCloseTo(0.0);
        static Bearing UUT;
        }

    [Subject(typeof(Bearing), "coersion")]
    class when_creating_a_bearing_of_minus_one
        {
        Because of = () => UUT = new Bearing(-1);

        It should_coerce_to_359 = () => UUT.Value.ShouldBeCloseTo(359.0);
        static Bearing UUT;
        }

    [Subject(typeof(Bearing), "coersion")]
    class when_creating_a_bearing_of_720
        {
        Because of = () => UUT = new Bearing(360.0 * 2);

        It should_coerce_to_zero = () => UUT.Value.ShouldBeCloseTo(0.0);
        static Bearing UUT;
        }

    [Subject(typeof(Bearing), "coersion")]
    class when_creating_a_bearing_of_slightly_less_than_360
        {
        Because of = () =>
            {
            var angle = 360.0 - 0.1 / 3600.0; // tenth of an arc second
            UUT = new Bearing(angle);
            };

        It should_not_coerce_to_zero = () => UUT.Value.ShouldBeCloseTo(360.0, 1.0 / 3600.0);
        static Bearing UUT;
        }

    /// <summary>
    ///     Test assumption about using modulus on a negative.
    /// </summary>
    [Subject(typeof(double), "modulus of negative")]
    class when_taking_the_modulus_of_a_negative
        {
        Because of = () => Result = -361.0 % 360.0;
        It should_be_minus_1 = () => Result.ShouldEqual(-1);
        static double Result;
        }

    [Subject(typeof(Bearing), "sexagesimal")]
    [UsedImplicitly]
    class when_converting_to_sexagesimal
        {
        Because of = () =>
            {
            RaDeneb = 20.6999491773451; // This value caused AWR-88
            UUT = new Bearing(RaDeneb);
            };
        protected static Bearing UUT;
        static double RaDeneb;

        Behaves_like<DenebRightAscension> deneb;
        }
    }