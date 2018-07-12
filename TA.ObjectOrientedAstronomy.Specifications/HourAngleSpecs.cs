// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2018 Tigra Astronomy, all rights reserved.
// 
// File: HourAngleSpecs.cs  Last modified: 2018-07-12@05:44 by Tim Long

using JetBrains.Annotations;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FundamentalTypes;

namespace TA.ObjectOrientedAstronomy.Specifications
    {
    [Subject(typeof(HourAngle), "coersion")]
    class when_creating_an_hour_angle_of_24
        {
        Because of = () => UUT = new HourAngle(24.0);

        It should_coerce_to_zero = () => UUT.Value.ShouldBeCloseTo(0.0);
        static HourAngle UUT;
        }

    [Subject(typeof(HourAngle), "coersion")]
    class when_creating_an_hour_angle_of_minus_one
        {
        Because of = () => UUT = new HourAngle(-1);

        It should_coerce_to_359 = () => UUT.Value.ShouldBeCloseTo(23.0);
        static HourAngle UUT;
        }

    [Subject(typeof(HourAngle), "coersion")]
    class when_creating_an_hour_angle_of_48
        {
        Because of = () => UUT = new HourAngle(48.0);

        It should_coerce_to_zero = () => UUT.Value.ShouldBeCloseTo(0.0);
        static HourAngle UUT;
        }

    [Subject(typeof(HourAngle), "coersion")]
    class when_creating_an_hour_angle_of_slightly_less_than_24
        {
        Because of = () =>
            {
            var angle = 24.0 - 0.1 / 3600.0; // tenth of an arc second
            UUT = new HourAngle(angle);
            };

        It should_not_coerce_to_zero = () => UUT.Value.ShouldBeCloseTo(24.0, 1.0 / 3600.0);
        static HourAngle UUT;
        }

    [Subject(typeof(HourAngle), "sexagesimal")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    class when_converting_hour_angle_to_sexagesimal
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